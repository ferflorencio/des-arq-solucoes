# Documento de Arquitetura de Solução  
**Controle de Fluxo de Caixa e Consolidação de Dados**

---

## Informações do Documento

| Campo | Valor |
|------|-------|
| Arquiteto Responsável | Fernando Florencio de Oliveira |
| Data | 01/2026 |
| Versão | 1.0 |
| Status | Em elaboração |

---

## Histórico de Versões

| Versão | Data | Autor | Descrição |
|------|------|------|-----------|
| 1.0 | 01/2026 | Fernando Florencio de Oliveira | Versão inicial do documento |

---

## 1. Introdução

### 1.1 Contexto

Este documento descreve a arquitetura da solução proposta para controle de fluxo de caixa diário e consolidação de dados financeiros, atendendo às necessidades operacionais e gerenciais de comerciantes.

### 1.2 Objetivo do Documento

O objetivo deste documento é apresentar:
- A visão geral da solução
- As capacidades de negócio envolvidas
- Os requisitos funcionais e não funcionais
- As diretrizes arquiteturais adotadas

Este documento serve como referência para equipes técnicas, arquitetos, stakeholders de negócio e times de operação.

---

## 2. Visão Geral da Solução

A solução tem como finalidade permitir que comerciantes realizem o controle diário de entradas e saídas financeiras, bem como a geração de relatórios consolidados de saldo diário, garantindo escalabilidade, resiliência e disponibilidade.

---

## 3. Capacidades de Negócio

### 3.1 Interface Channel

Capacidade de fornecer uma interface visual para que os usuários possam acessar a plataforma e executar as operações permitidas de forma simples e intuitiva.

### 3.2 Cash Flow

Capacidade responsável pelo registro e manutenção dos lançamentos financeiros, contemplando:
- Débitos
- Créditos
- Persistência dos dados de caixa

### 3.3 Finance Consolidate

Capacidade responsável por:
- Consolidação diária das operações de caixa
- Cálculo de saldo diário
- Geração de relatórios financeiros para consumo gerencial

---

## 4. Arquitetura da Solução

### 4.1 Visão Arquitetural

A solução será composta por serviços independentes, permitindo desacoplamento entre as operações de lançamento financeiro e o processo de consolidação diária, garantindo maior resiliência e escalabilidade.

### 4.1.1 Diagrama de Contexto (C4 – Context)

```mermaid
C4Context
title Diagrama de Contexto - Sistema de Fluxo de Caixa

Person(caixa, "Caixa", "Usuário que realiza lançamentos de Crédito e Débito.")
Person(gestor, "Gestor / Analista", "Usuário que consulta os relatórios de saldo diário.")

Enterprise_Boundary(c1, "Ecossistema Financeiro") {
    System(interface, "Interface Channel", "Permite o acesso visual às funcionalidades via Web.")
    System(cashflow_sys, "Cash Flow System", "Responsável pelo registro e controle das entradas e saídas.")
    System(consolidate_sys, "Financial Consolidate", "Responsável pelo processamento e consulta do saldo consolidado.")
}

Rel(caixa, interface, "Acessa o canal")
Rel(gestor, interface, "Acessa o canal")

Rel(interface, cashflow_sys, "Faz chamadas API", "JSON / HTTP")
Rel(interface, consolidate_sys, "Faz chamadas API", "JSON / HTTP")

Rel(consolidate_sys, cashflow_sys, "Consome dados para consolidado", "Assíncrono")
```

### 4.1.2 Diagramas de Container (C4 – Container)

```mermaid
C4Container
title Diagrama de Contêiner - Cash Flow

System_Ext(interface, "Interface Channel", "Web App")

Container_Boundary(cf_boundary, "Contexto: Cash Flow") {
    Container(api_cf, "Cash Flow API", ".NET 10 API REST", "Gerencia os lançamentos diários.")
    ContainerDb(db_cf, "Cash Flow DB", "MongoDB", "Armazena as transações brutas.")
    ContainerQueue(queue, "CashFlow-QUEUE", "RabbitMQ", "Fila para processamento assíncrono.")
}

Rel(interface, api_cf, "Faz chamadas API", "JSON / HTTP")
Rel(api_cf, db_cf, "Lê e escreve dados", "MongoDB Driver")
Rel(api_cf, queue, "Envia lançamentos para processamento", "AMQP / JSON")
```

```mermaid
C4Container
title Diagrama de Contêiner - Financial Consolidate

ContainerQueue(queue, "CashFlow-QUEUE", "RabbitMQ", "Fila com lançamentos pendentes.")
System_Ext(interface, "Interface Channel", "Web App")

Container_Boundary(fc_boundary, "Contexto: Financial Consolidate") {
    Container(worker, "Financial Consolidate Worker", ".NET 10 Worker", "Consome a fila e consolida o saldo.")
    Container(api_fc, "Financial Consolidate API", ".NET 10 API REST", "Disponibiliza os dados de saldo consolidado.")
    ContainerDb(db_fc, "Financial Consolidate DB", "MongoDB", "Armazena o histórico consolidado.")
    ContainerDb(cache, "Redis Cache", "Redis", "Cache de alta performance para saldo diário.")
}

Rel(queue, worker, "Encaminha itens", "AMQP / JSON")
Rel(worker, db_fc, "Lê e escreve dados", "MongoDB Driver")
Rel(worker, cache, "Atualiza saldo diário", "Redis Driver")

Rel(interface, api_fc, "Consulta saldo", "JSON / HTTP")
Rel(api_fc, cache, "Busca rápida de saldo", "Redis Driver")
Rel(api_fc, db_fc, "Consulta histórico", "MongoDB Driver")
```

```mermaid
C4Container
title Diagrama de Contêiner - Interface Channel (Frontend)

    Person(caixa, "Caixa", "Usuário do caixa que faz lançamentos (Crédito e Débito).")
    Person(gestor, "Gestor/Analista", "Usuário que acessa os relatórios consolidados diários.")

    Container_Boundary(frontend_boundary, "System: Interface Channel") {
        Container(web_app, "Front End CashFlow - Financial Consolidate", "HTML, Javascript, .NET", "Fornece interface visual para operações de cash flow e financial consolidate via WebSite.")
    }

    System_Ext(cashflow_sys, "Cash Flow", "Sistema de lançamentos.")
    System_Ext(consolidate_sys, "Financial Consolidate", "Sistema de consolidados.")

    Rel(caixa, web_app, "Acessa o canal")
    Rel(gestor, web_app, "Acessa o canal")

    Rel(web_app, cashflow_sys, "Faz chamadas API", "JSON/HTTP")
    Rel(web_app, consolidate_sys, "Faz chamadas API", "JSON/HTTP")

```
### 4.1.3 Diagramas de Topologia (Arq. Alvo)

![Diagrama de topologia](./ImgDocs/topology-target.png)
---

## 5. Requisitos

### 5.1 Requisitos de Negócio

| ID | Descrição |
|----|-----------|
| RN-01 | O comerciante deve controlar o fluxo de caixa diário por meio de lançamentos de débitos e créditos. |
| RN-02 | O sistema deve disponibilizar um relatório com o saldo diário consolidado. |

### 5.2 Requisitos Funcionais

| ID | Descrição |
|----|-----------|
| RF-01 | O sistema deve disponibilizar um serviço para registro de lançamentos financeiros (débitos e créditos). |
| RF-02 | O sistema deve disponibilizar um serviço responsável pela consolidação diária do fluxo de caixa. |

### 5.3 Requisitos Não Funcionais

| ID | Descrição |
|----|-----------|
| RNF-01 | O serviço de lançamentos não deve ficar indisponível caso o serviço de consolidação diária apresente falhas. |
| RNF-02 | O serviço de consolidação diária deve suportar picos de até **50 requisições por segundo**. |
| RNF-03 | A taxa máxima de perda de requisições durante picos deve ser de **5%**. |
| RNF-04 | A solução deve ser escalável, suportando dimensionamento horizontal e balanceamento de carga. |

---

## 6. Decisões de Arquitetura (ADR)

As principais decisões arquiteturais serão documentadas utilizando o padrão **Architecture Decision Record (ADR)**, garantindo rastreabilidade e histórico de decisões técnicas.

> Os ADRs devem ser versionados e armazenados juntamente com este documento.

---

## 7. Riscos e Considerações

- Possível aumento de carga no processo de consolidação em períodos de fechamento.
- Necessidade de garantir consistência eventual entre lançamentos e consolidados.
- Monitoramento contínuo dos serviços para atender aos SLAs definidos.

---

## 8. ADRs

### ADR 001: Escolha do Redis como Banco de Dados em Cache para as Consultas do Consolidado

#### Status
Aceito

### Data
01/2026

---

### Contexto

Durante o desenho da arquitetura do projeto **CashFlow**, foram definidos requisitos não funcionais relacionados a desempenho e resiliência do serviço responsável pela consolidação diária de dados financeiros. Em especial, destacam-se os seguintes requisitos:

| Código  | Descrição                                                                 |
|--------|---------------------------------------------------------------------------|
| RNF-02 | O serviço de consolidação diária deve suportar picos de até **50 requisições por segundo** |
| RNF-03 | A taxa máxima de perda de requisições durante picos deve ser de **5%**     |

O sistema CashFlow adota uma arquitetura baseada em **microserviços**, sendo que o **serviço de consolidados** é responsável por expor consultas frequentes a dados já processados e agregados. Essas consultas possuem padrão de leitura intensiva e são altamente sensíveis à latência, especialmente durante períodos de pico de acesso.

Para atender a esses requisitos não funcionais, identificou-se a necessidade de introduzir um mecanismo de **cache em memória**, capaz de reduzir a carga sobre os bancos de dados primários e garantir tempos de resposta consistentes sob alta concorrência.

---

### Requisitos Técnicos

Os requisitos técnicos considerados para a solução de cache foram:

- **Baixa latência** como requisito crítico, devido aos limites impostos pelos RNFs.
- **Alta capacidade de concorrência**, suportando múltiplas requisições simultâneas.
- **Escalabilidade horizontal**, permitindo crescimento do sistema sem degradação significativa de performance.
- **Persistência dos dados em cache sem expiração automática**, uma vez que os consolidados diários devem permanecer disponíveis para consulta contínua.
- **Modelo simples de dados**, baseado em chave e valor, sem necessidade de estruturas complexas.

---

### Alternativas Avaliadas

As seguintes alternativas foram consideradas antes da decisão final:

#### Banco de Dados Relacional
Embora bancos relacionais sejam adequados para persistência e consistência transacional, eles não são otimizados para cenários de leitura intensiva com baixa latência e alta concorrência. O uso do banco relacional como mecanismo de cache implicaria maior sobrecarga de I/O, locks e competição por recursos, podendo comprometer o atendimento aos requisitos de desempenho.

#### MongoDB
O MongoDB oferece flexibilidade de esquema e boa performance em determinados cenários, porém continua sendo um banco orientado a persistência em disco. Mesmo com índices, o custo de acesso e a latência são superiores quando comparados a soluções de cache em memória, especialmente sob cargas elevadas e padrões de acesso repetitivos.

Ambas as alternativas não atendiam de forma ideal aos requisitos de latência e throughput exigidos pelo serviço de consolidados.

---

### Decisão

Foi decidido utilizar o **Redis** como banco de dados em cache para as consultas do consolidado diário.

O Redis foi escolhido por ser uma solução de cache em memória amplamente adotada, com excelente desempenho para operações de leitura, suporte nativo a alta concorrência e capacidade de escalar horizontalmente. Sua arquitetura é especialmente adequada para cenários de acesso frequente a dados já processados, como é o caso dos consolidados diários.

A implementação utiliza:
- Redis executando em **container**, integrado ao ambiente de desenvolvimento via **.NET Aspire**.
- Integração com a aplicação através da abstração **IDistributedCache**, que utiliza internamente a biblioteca **StackExchange.Redis**.

---

### Consequências

#### Benefícios

- Atendimento aos requisitos não funcionais definidos para o projeto.
- Redução significativa da latência nas consultas de consolidados diários.
- Capacidade de suportar picos de carga conforme especificado.
- Validação prática da decisão por meio de **testes de carga com K6**, incluídos na solução, que comprovam o atendimento aos cenários propostos.

#### Custos e Riscos

- Introdução de um novo componente de infraestrutura, aumentando a complexidade operacional.
- Necessidade de cuidados com estratégias de invalidação e consistência dos dados em cache, especialmente em cenários futuros de evolução da arquitetura.

---

### Referências

- Testes de carga com K6 disponíveis no repositório da solução.
- Requisitos não funcionais definidos para o projeto CashFlow.
---

## 9. Referências para construção do DAS

- RabbitMQ Documentation  
  https://www.rabbitmq.com/docs

- Guia sobre Architecture Decision Records (ADR)  
  https://medium.com/@jhonywalkeer/guia-completo-sobre-architecture-decision-records-adr-defini%C3%A7%C3%A3o-e-melhores-pr%C3%A1ticas-f63e66d33e6

- Entendendo o C4 Model: Uma Abordagem para Arquitetura de Software  
  https://medium.com/cajudevs/entendendo-o-c4-model-uma-abordagem-para-arquitetura-de-software-3ed0f007ae66

- C4 Model Documentation - Diagrams
  https://c4model.com/diagrams