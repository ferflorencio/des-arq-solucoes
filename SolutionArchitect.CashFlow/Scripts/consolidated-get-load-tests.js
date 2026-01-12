import http from "k6/http";
import { check } from "k6";
import { Rate } from "k6/metrics";

/**
 * Métrica de erro real (regra de negócio)
 */
export const errorRate = new Rate("errors");

/**
 * Configuração do teste
 */
export const options = {
  scenarios: {
    consolidated_load: {
      executor: "constant-arrival-rate",
      rate: 50,            // 50 RPS
      timeUnit: "1s",
      duration: "60s",
      preAllocatedVUs: 50,
      maxVUs: 150,
    },
  },
  thresholds: {
    errors: ["rate<=0.05"],
    http_req_duration: ["p(95)<500"],
  },
};

/**
 * URLs vindas do Aspire
 */
const CASHFLOW_BASE =
  __ENV.BASE_URL_CASHFLOW;

const CONSOLIDATE_BASE =
  __ENV.BASE_URL_CONSOLIDATE;

if (!CASHFLOW_BASE || !CONSOLIDATE_BASE) {
  throw new Error("Service URLs not injected by Aspire");
}

export function setup() {
  console.log("==== K6 SETUP START ====");
  console.log(`CashFlow API Base: ${CASHFLOW_BASE}`);
  console.log(`Consolidate API Base: ${CONSOLIDATE_BASE}`);

  const today = new Date();

  const year = today.getUTCFullYear();
  const month = today.getUTCMonth() + 1;
  const day = today.getUTCDate();

  console.log(`Using date (UTC): ${year}-${month}-${day}`);

  for (let i = 0; i < 10; i++) {
    const amount = Math.floor(Math.random() * 1000) + 1;

    const payload = JSON.stringify({
      operationType: "Credit",
      amount: amount,
    });

    console.log(
      `[SETUP] POST /cashflow attempt ${i + 1} | amount=${amount}`
    );

    const res = http.post(
      `${CASHFLOW_BASE}/api/v1/cashflow`,
      payload,
      {
        headers: { "Content-Type": "application/json" },
      }
    );

    console.log(
      `[SETUP] Response ${i + 1} | status=${res.status}`
    );

    if (res.status !== 200) {
      console.log(
        `[SETUP][ERROR] Response body: ${res.body}`
      );
    }

    check(res, {
      "credit created": (r) => r.status === 200,
    });
  }

  console.log("==== K6 SETUP END ====");

  return { year, month, day };
}

/**
 * LOAD
 */
export default function (data) {
  const url =
    CONSOLIDATE_BASE +
    "/api/v1/cashflow/consolidated/" +
    data.year +
    "/" +
    data.month +
    "/" +
    data.day;

  const res = http.get(url);

  const ok = check(res, {
    "status 200 or 404": (r) =>
      r.status === 200 || r.status === 404,
  });

  errorRate.add(!ok);
}
