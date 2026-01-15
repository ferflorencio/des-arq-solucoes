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

A solução tem como finalidade permitir que um comerciante realize o controle diário de entradas e saídas financeiras, bem como a geração de relatórios consolidados de saldo diário, garantindo escalabilidade, resiliência e disponibilidade.

---

## 3. Capacidades de Negócio

### 3.1 Cash Flow

Capacidade responsável pelo registro e manutenção dos lançamentos financeiros, contemplando:
- Débitos
- Créditos
- Persistência dos dados de caixa

### 3.2 Finance Consolidate

Capacidade responsável por:
- Consolidação diária das operações de caixa
- Cálculo de saldo diário
- Geração de relatórios financeiros para consumo gerencial

---

## 4. Arquitetura da Solução

### 4.1 Visão Arquitetural

A solução será composta por serviços independentes, permitindo desacoplamento entre as operações de lançamento financeiro e o processo de consolidação diária, garantindo maior resiliência e escalabilidade.

Você pode acessar todos os diagramas pela plataforma do Draw IO pelo [LINK](https://drive.google.com/file/d/13uJC5h6orcYGYhFBDFSJuhOsAosX8uQT/view?usp=sharing) ou conferir abaixo cada um deles nas imagens

### 4.1.1 Diagrama de Contexto (C4 – Context)

![Diagrama de Contexto](./ImgDocs/c4-1-context.png)

### 4.2.1 Diagrama de Container - CashFlow (C4 – Container)

![Diagrama de Container Cashflow](./ImgDocs/c4-2-container-cashflow.png)

### 4.2.2 Diagrama de Container - Consolidated (C4 – Container)

![Diagrama de Container Consolidated](./ImgDocs/c4-3-container-consolidated.png)

### 4.3.1 Diagrama de Topologia (Arq. Alvo)

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

O sistema CashFlow adota uma arquitetura baseada em **microserviços**, sendo que o **serviço de consolidados** é responsável por expor consultas frequentes a dados já processados e agregados.

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
Embora bancos relacionais sejam adequados para persistência e consistência transacional, eles não são otimizados para cenários de leitura intensiva com baixa latência e alta concorrência. O uso do banco relacional como mecanismo de cache implicaria maior sobrecarga de I/O, locks e competição por recursos, podendo comprometer o atendimento aos requisitos de desempenho, além do alto custo para uma estrutura muito simples de registros e consultas.

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


## 7. Riscos e Considerações

- Possível aumento de carga no processo de consolidação em períodos de fechamento.
- Necessidade de garantir consistência eventual entre lançamentos e consolidados.
- Monitoramento contínuo dos serviços para atender aos SLAs definidos.

---

## 8. Possíveis Evoluções Arquiteturais

Esta seção descreve possíveis evoluções arquiteturais e sistêmicas da solução proposta. As evoluções aqui apresentadas não foram implementadas na versão atual devido ao escopo e ao tempo do desafio, porém foram consideradas durante o desenho da arquitetura, demonstrando a capacidade da solução de evoluir de forma escalável, flexível e alinhada aos requisitos de negócio futuros.



### 8.1 Escalonamento da Solução: Suporte a Múltiplos Usuários e Múltiplos Comércios

Atualmente, a solução foi desenhada considerando o cenário de **um único comerciante** responsável pelo controle de seus lançamentos de débito e crédito. Uma possível evolução natural do sistema seria a adaptação para um modelo **multiusuário e multicomércio**.

Nesse cenário, seria necessário introduzir um **módulo de controle de usuários e permissões**, permitindo que:

- Um usuário possa estar associado a **um ou mais comércios**;
- Um comércio possa ter **múltiplos usuários com acesso autorizado**;
- Os lançamentos de débito e crédito sejam sempre vinculados a um comércio específico;
- O acesso aos relatórios consolidados seja restrito apenas aos comércios associados ao usuário autenticado.

Essa modelagem permitiria atender cenários mais complexos, como:
- Escritórios contábeis gerenciando múltiplos comércios;
- Empresas com múltiplas filiais;
- Diferentes perfis de usuários (operacional, gestor, auditor).

Do ponto de vista arquitetural, essa evolução exigiria:
- Criação de um domínio de **Identidade e Acesso**;
- Relacionamento explícito entre **Usuários ↔ Comércios ↔ Lançamentos**;
- Garantia de isolamento lógico dos dados entre diferentes comércios (multi-tenancy).

---

### 8.2 Evolução do Cashflow API para um Modelo Orientado a Eventos

Na arquitetura atual, o **Cashflow API** é responsável por:
- Receber lançamentos de débito e crédito;
- Calcular o consolidado diário;
- Persistir os dados no MongoDB;
- Publicar o resultado consolidado no RabbitMQ.

Uma evolução relevante seria transformar o Cashflow API em uma **porta de entrada exclusivamente responsável pela validação e aceitação dos lançamentos**, adotando um modelo ainda mais orientado a eventos.

Nesse novo cenário:
- O Cashflow API validaria o payload do lançamento (débitos/créditos);
- Após validação, o lançamento seria publicado no RabbitMQ;
- Um **worker de consolidação** passaria a ser responsável por:
  - Persistir os lançamentos no MongoDB;
  - Executar o cálculo do consolidado diário;
  - Armazenar o resultado consolidado no Redis.

Essa mudança traria benefícios importantes:
- Redução da responsabilidade do Cashflow API;
- Eliminação da necessidade de controle de concorrência no API síncrono;
- Garantia de integridade dos dados através de processamento sequencial e controlado no worker;
- Maior aderência aos princípios de **Event-Driven Architecture** e **Single Responsibility Principle**.

Além disso, essa abordagem fortalece os requisitos não funcionais do sistema, como resiliência e escalabilidade, pois desacopla completamente o recebimento de lançamentos do processamento e consolidação dos dados.

---

### 8.3 Evoluções em Nível Sistêmico e Reutilização de Componentes

A solução foi desenvolvida utilizando **.NET Aspire**, explorando seus recursos de orquestração local, integração entre componentes e service discovery. No entanto, cada aplicação é responsável por suas próprias integrações com infraestrutura e contratos, o que resulta em algumas duplicações de código, como:

- Configurações de acesso a bancos de dados;
- DTOs e contratos compartilhados entre serviços;
- Modelos de mensagens utilizados no RabbitMQ.

Uma evolução natural da arquitetura seria a criação de **projetos de Class Library compartilhados** que podem virar pacotes publicados e consumidos pelas soluções, contendo:
- Contratos e DTOs comuns;
- Abstrações de integração com infraestrutura;
- Modelos de eventos e mensagens.

Esse modelo reduz a duplicação de código, evita divergências de contratos entre serviços e melhora a manutenibilidade da solução e contribui diretamente para a escalabilidade organizacional do projeto, facilitando a inclusão de novos serviços, equipes e domínios sem comprometer a consistência da solução.

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

- MongoDB Vs Redis
  https://www.bytebase.com/blog/mongodb-vs-redis/

 - Redis vs MongoDb Fight
  https://dev.to/playtomic/redis-vs-mongodb-fight-1481 