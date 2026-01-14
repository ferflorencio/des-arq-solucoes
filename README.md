# SolutionArchitect.CashFlow

## Visão Geral

O **SolutionArchitect.CashFlow** é uma aplicação Front e Backend desenvolvida para **controlar o fluxo de caixa diário** de um comerciante, permitindo o registro de **lançamentos financeiros (débitos e créditos)** e a **consolidação diária do saldo**.

A aplicação centraliza as movimentações financeiras e disponibiliza uma API para consulta do saldo consolidado por dia, servindo como base para relatórios e análises financeiras internas.

- **Uso:** Interno  
- **Domínio:** Financeiro / Fluxo de Caixa  

---

## Objetivo do Projeto

- Registrar lançamentos financeiros de **crédito** e **débito**
- Manter o histórico diário das movimentações
- Gerar e disponibilizar o **saldo diário consolidado**
- Garantir consistência e integridade dos dados financeiros

---

## Visão Geral da Arquitetura

A aplicação foi projetada seguindo princípios de **baixo acoplamento** e **separação de responsabilidades**, utilizando práticas do ecossistema .NET.

A arquitetura favorece a manutenção, testabilidade e evolução do sistema, permitindo a introdução de novos fluxos e integrações sem impacto direto nas regras de negócio.


> **Diagrama de Arquitetura**  
> ![Diagrama de topologia](./Docs/ImgDocs/developed-architecture.png)

### Características Arquiteturais

- APIs e Worker Services em .NET 10
- Organização em camadas:
  - Domain
  - Application
  - Infrastructure
- Persistência orientada a documentos
- Uso de cache para otimização de Consultas
- Mensageria para desacoplamento
- Infraestrutura local orquestrada via **.NET Aspire**
- Monorepo para aproveitar totalmente os recursos do **.NET Aspire**
- Frontend utilizando tecnologia **Microsoft Blazor**

---

## Stack Tecnológica

### Backend
- **.NET 10**

### FrontEnd
- **.NET 10 + Blazor**

### Persistência
- **MongoDB**  
  Armazenamento dos lançamentos financeiros.

- **Redis**  
  Cache para otimizar consultas do consolidado diário.

### Mensageria
- **RabbitMQ**  
  Utilizado para comunicação assíncrona e propagação de eventos de domínio.

### Infraestrutura
- **Docker**
- **.NET Aspire** — orquestração dos serviços no ambiente local

---

## Como Executar o Projeto Localmente

### Pré-requisitos

Certifique-se de que os seguintes itens estejam instalados:

- **.NET SDK 10** ([Download](https://dotnet.microsoft.com/en-us/download/dotnet/10.0))
- **Docker** ([Download](https://docs.docker.com/get-started/get-docker/))
- **.NET Aspire** ([Download](https://aspire.dev/pt-br/get-started/install-cli/))

---

### Executando a aplicação

```sh
# 1. Clone o repositório:  
# Ou faça o download do repositorio em caso de bloqueios em computadores empresariais

git clone https://github.com/ferflorencio/des-arq-solucoes.git

# 2. Acesse a pasta do projeto:

cd SolutionArchitect.CashFlow

# 3. Execute a aplicação utilizando o Aspire:

aspire run

# 4. O Aspire será responsável por subir automaticamente:

> FrontEnd CashFlow
> API CashFlow
> API CashFlow Consolidate
> Worker CashFlow Consolidate
> MongoDB
> Redis
> RabbitMQ
> K6

# 5. Acompanhe o status dos serviços, logs e métricas através do Dashboard do Aspire.

# 6. Execute as operações utilizando o FrontEnd do cashflow

# 7. Caso queira executar o Teste com K6 para validar o requisito de consultas por minuto nos relatórios:
  ## clicar no botão "Play" do K6 no dashboard do aspire 
  ## ao aparecer o link "K6 Dashboard"
  ## clicar para conseguir acompanhar a execução do teste de carga.
```

---

### DAS (Documento de arquitetura de solução)

Para acessar a documentação de arquitetura, visite o link: [DAS](https://github.com/ferflorencio/des-arq-solucoes/blob/main/Docs/Documentacao.md)