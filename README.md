# OficinaApi — Gestão de Ordens de Serviço Automotivas

API em .NET 9 para gestão do ciclo de Ordens de Serviço (OS), com orçamento, aprovação interna/externa, catálogo, estoque e rastreabilidade de status.

## 1) Visão geral

A solução cobre o ciclo completo de operação da oficina, desde cadastro e catálogo até execução de serviços e relatórios.

Principais capacidades:
- gestão de **Clientes** e **Veículos** com validações de domínio (incluindo CPF/CNPJ);
- gestão de **Peças**, **Insumos**, **Serviços** e **Estoque**;
- gestão de **Ordens de Serviço** com abertura completa, classificação, diagnóstico, orçamento, execução e entrega;
- aprovação/recusa interna e externa de orçamento por link de e-mail;
- controle da origem da atualização de status (**Interna** / **Externa**);
- relatórios operacionais (tempo médio de execução).

## 2) Arquitetura

A aplicação segue **Clean Architecture** com organização modular e conceitos de **DDD**, priorizando separação de responsabilidades, desacoplamento e testabilidade.

| Camada | Responsabilidade |
|---|---|
| **Oficina.Api** | Endpoints HTTP, autenticação JWT, Swagger e middleware global de exceções |
| **Oficina.Application** | UseCases, orquestração de fluxos, DTOs, validações e contratos |
| **Oficina.Domain** | Entidades, Value Objects e regras de negócio |
| **Oficina.Infrastructure** | Persistência com EF Core/SQL Server, repositórios e envio de e-mail |

## 3) Tecnologias utilizadas

- .NET 9 / C#
- ASP.NET Core Web API
- Entity Framework Core 9
- SQL Server 2022
- JWT Bearer
- MailKit + smtp4dev
- xUnit + Moq + Coverlet

## 4) Como executar o projeto

### Pré-requisitos
- .NET SDK 9
- Docker + Docker Compose
- `dotnet-ef`

### Docker Compose
```bash
docker compose -f docker/docker-compose.yml up -d sqlserver smtp4dev
```

### Migrations
```bash
dotnet ef database update -p src/Oficina.Infrastructure/Oficina.Infrastructure.csproj -s src/Oficina.Api/Oficina.Api.csproj
```

### Execução da API
```bash
dotnet run --project src/Oficina.Api
```

### Swagger
- Ambiente local: `http://localhost:49324/swagger`
- Ambiente Docker: `http://localhost:8080/swagger`

> Para subir API + SQL Server + smtp4dev no compose:
>
> ```bash
> docker compose -f docker/docker-compose.yml up --build
> ```

## 5) Integração de e-mail (smtp4dev)

A integração externa de aprovação/recusa de orçamento usa token por link de e-mail.

- Interface smtp4dev: `http://localhost:5000`
- SMTP local (API fora de container): `localhost:2525`
- SMTP Docker (API em container): `smtp4dev:25`

Configurações de e-mail:
- `EmailSettings:SmtpHost`
- `EmailSettings:SmtpPort`
- `EmailSettings:EnableSsl`
- `EmailSettings:From`
- `EmailSettings:BaseUrl`

## 6) Fluxos principais

### 6.1 Abertura de OS
- endpoint único: `POST /api/ordens-servico`;
- cria/reaproveita cliente e veículo;
- exige ao menos 1 serviço e aceita peças/insumos;
- calcula o total do orçamento na abertura;
- mantém a OS em **Recebida**.

### 6.2 Classificação
- endpoint: `POST /api/ordens-servico/{id}/classificar`;
- preventiva: direciona para **AguardandoAprovacao**;
- corretiva: direciona para **EmDiagnostico**.

### 6.3 Aprovação externa via e-mail
- `GET /api/orcamentos/acoes-externas/aprovar?token=...`
- `GET /api/orcamentos/acoes-externas/recusar?token=...`
- processamento da ação externa atualiza orçamento e OS;
- origem da atualização registrada como **Externa**.

## 7) Rotas da API (completas)

| Método | Rota | Autenticação |
|---|---|---|
| POST | `/api/auth/login` | Pública |
| POST | `/api/clientes` | JWT |
| GET | `/api/clientes/{id}` | JWT |
| PUT | `/api/clientes/{id}` | JWT |
| POST | `/api/veiculos` | JWT |
| GET | `/api/veiculos/{id}` | JWT |
| PUT | `/api/veiculos/{id}` | JWT |
| GET | `/api/veiculos/por-cliente/{clienteId}` | JWT |
| POST | `/api/pecas` | JWT |
| GET | `/api/pecas/{id}` | JWT |
| PUT | `/api/pecas/{id}` | JWT |
| POST | `/api/insumos` | JWT |
| GET | `/api/insumos/{id}` | JWT |
| PUT | `/api/insumos/{id}` | JWT |
| POST | `/api/servicos` | JWT |
| GET | `/api/estoque/pecas/{pecaId}` | JWT |
| POST | `/api/estoque/pecas/{pecaId}/ajustar` | JWT |
| GET | `/api/estoque/insumos/{insumoId}` | JWT |
| POST | `/api/estoque/insumos/{insumoId}/ajustar` | JWT |
| POST | `/api/ordens-servico` | JWT |
| POST | `/api/ordens-servico/preventiva` | JWT |
| POST | `/api/ordens-servico/corretiva` | JWT |
| POST | `/api/ordens-servico/{id}/classificar` | JWT |
| POST | `/api/ordens-servico/{id}/diagnosticos` | JWT |
| GET | `/api/ordens-servico/{id}/status` | JWT |
| GET | `/api/ordens-servico/{id}` | JWT |
| GET | `/api/ordens-servico` | JWT |
| POST | `/api/ordens-servico/{id}/finalizar` | JWT |
| POST | `/api/ordens-servico/{id}/entregar` | JWT |
| GET | `/api/orcamentos/{id}` | JWT |
| POST | `/api/orcamentos/{id}/aprovar` | JWT |
| POST | `/api/orcamentos/{id}/recusar` | JWT |
| GET | `/api/orcamentos/acoes-externas/aprovar?token=...` | Pública |
| GET | `/api/orcamentos/acoes-externas/recusar?token=...` | Pública |
| GET | `/api/relatorios/tempo-medio-execucao` | JWT |

## 8) Testes e cobertura
![Coverage](./badges/badge_combined.svg)

### Rodar testes
```bash
dotnet test
```

### Gerar cobertura de testes
```bash
dotnet test --collect:"XPlat Code Coverage"
```

Saída de cobertura (formato Cobertura):
- `tests/Oficina.Tests/TestResults/**/coverage.cobertura.xml`

## 9) Segurança

- autenticação JWT para endpoints internos;
- endpoints externos com token e expiração;
- validação de CPF/CNPJ com dígitos verificadores no domínio;
- tratamento global de exceções na API.

## 10) Vídeo de demonstração

- _[inserir link do vídeo aqui]_

## 11) Evolução contínua da solução (Fase 1 → Fase 2)

- abertura completa da OS (cliente, veículo, serviços, peças e insumos);
- status inicial **Recebida** e classificação posterior;
- integração externa de aprovação/recusa via e-mail;
- rastreabilidade de origem de status (Interna/Externa);
- listagem de OS com filtro e ordenação;
- validação de CPF/CNPJ reforçada;
- controllers finos com lógica concentrada em UseCases;
- cobertura de testes atualizada.

## 12) Observações finais

- README focado na solução como um todo, com navegação rápida para avaliação técnica.
- O fluxo completo de demonstração prática está no vídeo.
- Material complementar: [Miro do projeto](https://miro.com/app/board/uXjVGPRzlmM=/).
