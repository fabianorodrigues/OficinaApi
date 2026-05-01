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

Você pode rodar de **2 formas**:
- **Opção A: Docker Compose (API + SQL Server + smtp4dev)**
- **Opção B: Kubernetes local com Minikube**

### Opção A) Docker Compose

#### Pré-requisitos
- Docker + Docker Compose
- .NET SDK 9 e `dotnet-ef`, apenas se você optar por aplicar migrations manualmente fora do container

#### 1. Conferir variáveis locais
O arquivo `docker/.env.example` possui valores fictícios para desenvolvimento local. Ele não contém secrets reais.

Para executar sem criar arquivo local:
```bash
docker compose --env-file docker/.env.example -f docker/docker-compose.yml up --build
```

Para customizar valores:
```bash
cp docker/.env.example docker/.env
docker compose --env-file docker/.env -f docker/docker-compose.yml up --build
```

Principais variáveis:
- `MSSQL_SA_PASSWORD`: senha local forte do usuário `sa`
- `JWT_SECRET`: segredo JWT local com pelo menos 32 caracteres
- `RUN_MIGRATION`: `true` no `.env.example` para facilitar o primeiro boot local; use `false` fora de desenvolvimento
- `ADMIN_INICIAL_*`: credenciais fictícias do admin local de bootstrap

#### 2. Subir API, SQL Server e smtp4dev
```bash
docker compose --env-file docker/.env.example -f docker/docker-compose.yml up --build
```

#### 3. Acessar os serviços
- Swagger: `http://localhost:8080/swagger`
- Healthcheck da API: `http://localhost:8080/health`
- smtp4dev: `http://localhost:5000`
- SQL Server local: `localhost,1433`

#### 4. Aplicar migrations
No fluxo local recomendado, `docker/.env.example` define `RUN_MIGRATION=true`, então as migrations são aplicadas no startup da API para facilitar a primeira execução.

Em produção, Kubernetes ou ambientes compartilhados, mantenha `RUN_MIGRATION=false` e aplique migrations de forma controlada.

Opção manual, via host:
```bash
dotnet ef database update -p src/Oficina.Infrastructure/Oficina.Infrastructure.csproj -s src/Oficina.Api/Oficina.Api.csproj --connection "Server=localhost,1433;Database=OficinaDb;User Id=sa;Password=Oficina_dev_2026!;TrustServerCertificate=True;"
```

Para forçar migrations automáticas sem usar `.env.example`:
```bash
RUN_MIGRATION=true docker compose -f docker/docker-compose.yml up --build
```

No PowerShell:
```powershell
$env:RUN_MIGRATION="true"; docker compose -f docker/docker-compose.yml up --build
```

#### 5. Login local de funcionário/admin
Depois das migrations, o bootstrap cria um admin local fictício se `ADMIN_INICIAL_ENABLED=true`:

```json
{
  "cpf": "39053344705",
  "senha": "Senha@123"
}
```

Endpoint: `POST http://localhost:8080/api/auth/funcionario`

#### 6. Parar e limpar containers
```bash
docker compose --env-file docker/.env.example -f docker/docker-compose.yml down
```

Para remover também os volumes locais do SQL Server e smtp4dev:
```bash
docker compose --env-file docker/.env.example -f docker/docker-compose.yml down -v
```

#### 7. Troubleshooting rápido
- Porta ocupada: altere `API_HTTP_PORT`, `SQLSERVER_PORT`, `SMTP4DEV_WEB_PORT` ou `SMTP4DEV_SMTP_PORT` no arquivo `.env`.
- SQL Server demorando: aguarde o healthcheck; o primeiro boot pode levar alguns minutos.
- Erro de senha SQL: use senha forte e, se trocar depois de criar o volume, remova o volume com `down -v`.
- Migrations: para execução local simples use `RUN_MIGRATION=true`; para produção use `false` e rode migrations de forma controlada.
- Swagger: fica exposto no fluxo local; restrinja por ambiente antes de publicar a API.

> Os valores do compose e de `docker/.env.example` são apenas para desenvolvimento local. Em produção ou Kubernetes, configure secrets e variáveis por ambiente.

### Opção B) Kubernetes local com Minikube

> Fluxo simplificado para subir **infra + API** no Minikube, usando a imagem da API direto do Docker Hub.

#### Pré-requisitos
- Docker
- Minikube
- kubectl

#### 1. Iniciar o cluster local
```bash
minikube start
```

#### 2. Aplicar os manifestos Kubernetes
```bash
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/sqlserver-deployment.yaml
kubectl apply -f k8s/sqlserver-service.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml
```

#### 3. Validar se os pods estão prontos
```bash
kubectl get pods
```

#### 4. Obter a URL local da API
```bash
minikube service oficina-api-service --url
```

#### 5. Abrir o Swagger
Com a URL retornada no passo anterior, acesse:

```text
http://<URL_DO_MINIKUBE>/swagger
```

Exemplo:
```text
http://127.0.0.1:30007/swagger
```

#### (Opcional) Limpar ambiente
```bash
kubectl delete -f k8s/service.yaml
kubectl delete -f k8s/deployment.yaml
kubectl delete -f k8s/sqlserver-service.yaml
kubectl delete -f k8s/sqlserver-deployment.yaml
kubectl delete -f k8s/secret.yaml
kubectl delete -f k8s/configmap.yaml
```

## 5) Integração de e-mail (smtp4dev)

A integração externa de aprovação/recusa de orçamento usa token por link de e-mail.

- Interface smtp4dev: `http://localhost:5000`
- SMTP local (API fora de container): `localhost:25`
- SMTP Docker (API em container): `smtp4dev:25`
- Base URL aprovação/recusa (Development): `https://localhost:49323`
- Base URL aprovação/recusa (Docker): `http://localhost:8080`

Configurações para fluxo de aprovação e recusa de orçamento por e-mail:
- `EmailSettings:SmtpHost`
- `EmailSettings:SmtpPort`
- `EmailSettings:EnableSsl`
- `EmailSettings:From`
- `EmailSettings:BaseUrlSmtp`
- `EmailSettings:BaseUrlAprovaRecusaOrcamento`

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

### 7.1 Collection Postman (cenários de teste)

- Link da collection/environment: [Postman](./postman/OficinaApi.postman_collection.json)
- Cenários incluídos:
  - autenticação (login com captura automática de JWT);
  - cadastro base (cliente e veículo);
  - fluxo de OS (abertura, classificação e consulta de status);
  - orçamento (aprovar e recusar);
  - relatório (tempo médio de execução).


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

### 9.1 Segurança por perfis

A API possui três perfis no JWT:

| Perfil | Autenticação | Acesso |
| --- | --- | --- |
| Cliente | `POST /api/auth/cliente` com CPF cadastrado | Somente recursos próprios pelas rotas `api/minhas-*` e `api/meus-*` |
| Funcionário | `POST /api/auth/funcionario` com CPF + senha | Rotas internas da oficina, exceto rotas `AdminOnly` |
| Admin | `POST /api/auth/funcionario` com CPF + senha | Tudo que funcionário acessa e endpoints administrativos |

Claims emitidas:
- `sub`
- `cpf`
- `role`
- `clienteId`, quando o perfil for Cliente
- `funcionarioId`, quando o perfil for Funcionário ou Admin

Configuração JWT:
```json
{
  "Jwt": {
    "Issuer": "Oficina.Api",
    "Audience": "Oficina.Api",
    "Secret": "CHAVE_COM_PELO_MENOS_32_CARACTERES",
    "ExpirationMinutes": 120
  }
}
```

Admin inicial:
```json
{
  "AdminInicial": {
    "Enabled": true,
    "Nome": "Admin Local",
    "Cpf": "39053344705",
    "Senha": "Senha@123"
  }
}
```

O admin inicial é criado apenas se `AdminInicial` estiver habilitado em ambiente de desenvolvimento/homologação ou por configuração explícita, e somente quando não existir funcionário/admin com o CPF informado. A senha é armazenada como hash e não há credencial `admin/admin` fixa no código.

Endpoints administrativos `AdminOnly`:
- `POST /api/admin/funcionarios`
- `GET /api/admin/funcionarios`
- `GET /api/admin/funcionarios/{id}`
- `PUT /api/admin/funcionarios/{id}`
- `PATCH /api/admin/funcionarios/{id}/alterar-senha`
- `PATCH /api/admin/funcionarios/{id}/ativar`
- `PATCH /api/admin/funcionarios/{id}/inativar`

Rotas do cliente autenticado:
- `GET /api/minhas-ordens-servico`
- `GET /api/minhas-ordens-servico/{id}`
- `GET /api/minhas-ordens-servico/{id}/status`
- `GET /api/meus-orcamentos/{id}`
- `POST /api/meus-orcamentos/{id}/aprovar`
- `POST /api/meus-orcamentos/{id}/recusar`

## 10) Vídeo de demonstração

- [Vídeo](https://youtu.be/8kAxO5SFWiY)

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
