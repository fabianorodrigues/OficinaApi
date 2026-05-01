# Oficina API - Tech Challenge FIAP | Fases 1 e 2

![Coverage](./badges/badge_combined.svg)

API REST para gestão de oficina mecânica, desenvolvida em .NET 9 como parte das **Fases 1 e 2 da pós-graduação em Software Architecture da FIAP**.

O projeto contempla o fluxo operacional de uma oficina, incluindo clientes, veículos, catálogo de serviços, peças, insumos, estoque, ordens de serviço, diagnósticos, orçamentos, autenticação, autorização e perfis de acesso.

Este repositório representa a base funcional entregue e avaliada nas Fases 1 e 2. As próximas fases da pós tem como objetivo evoluir esta solução em novos repositórios, com foco em Cloud, API Gateway, Serverless, Infraestrutura como Código, Observabilidade e Automação de Deploy.

---

## Visão Geral

A aplicação permite:

- cadastro e manutenção de clientes;
- cadastro e manutenção de veículos;
- gestão de serviços, peças e insumos;
- controle de estoque;
- abertura completa de ordens de serviço;
- classificação da OS como preventiva ou corretiva;
- registro de diagnóstico;
- geração e aprovação/recusa de orçamentos;
- execução, finalização e entrega da OS;
- autenticação JWT por perfil;
- consulta de ordens de serviço pelo cliente autenticado;
- aprovação/recusa externa de orçamento via e-mail;
- testes automatizados e validações de segurança.

---

## Destaques Técnicos

- API desenvolvida em **.NET 9**.
- Organização baseada em **Clean Architecture**, **DDD** e **Use Cases**.
- Separação entre `Api`, `Application`, `Domain`, `Infrastructure` e `Tests`.
- Autenticação JWT com perfis:
  - Cliente;
  - Funcionário;
  - Admin.
- Controle de acesso por perfil.
- Validação de ownership para rotas do cliente.
- Fluxo completo de Ordem de Serviço e Orçamento.
- Integração local de e-mail com `smtp4dev`.
- Execução local via:
  - .NET CLI;
  - Docker Compose;
  - Kubernetes local com Minikube.
- Testes automatizados com xUnit, Moq e Coverlet.
- Collection Postman com cenários sequenciais.

---

## Arquitetura

O projeto segue uma organização baseada em **Clean Architecture**, **DDD** e **Use Cases**, separando responsabilidades entre entrada, aplicação, domínio e infraestrutura.

| Projeto | Responsabilidade |
|---|---|
| `Oficina.Api` | Controllers, autenticação, autorização, Swagger e middlewares |
| `Oficina.Application` | Casos de uso, contratos, validações e modelos de aplicação |
| `Oficina.Domain` | Entidades, value objects, enums e regras de negócio |
| `Oficina.Infrastructure` | EF Core, repositórios, persistência e integrações externas |
| `Oficina.Tests` | Testes de domínio, aplicação, API e segurança |

---

## Tecnologias Utilizadas

- .NET 9
- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT Bearer
- FluentValidation
- xUnit
- Moq
- Coverlet
- Docker
- Docker Compose
- Kubernetes local com Minikube
- smtp4dev
- Swagger/OpenAPI
- GitHub Actions

---

## Como Executar o Projeto

### Variáveis principais

| Variável | Uso |
|---|---|
| `ConnectionStrings__SqlServer` | Conexão com SQL Server |
| `Jwt__Secret` | Chave de assinatura JWT |
| `Jwt__Issuer` | Emissor do token |
| `Jwt__Audience` | Audiência do token |
| `Jwt__ExpirationMinutes` | Tempo de expiração do token |
| `RUN_MIGRATION` | Executa migrations na inicialização em ambiente local/demonstração |
| `AdminInicial__Nome` | Nome do admin inicial |
| `AdminInicial__Cpf` | CPF do admin inicial |
| `AdminInicial__Senha` | Senha do admin inicial |

> Os valores locais são apenas para desenvolvimento e demonstração. Para ambientes reais, o valores devem ser configurados por variáveis de ambiente, secrets ou ferramentas equivalentes.

---

## Opção 1 — Executar com .NET

### Pré-requisitos

- .NET SDK 9
- SQL Server LocalDB ou SQL Server acessível

Por padrão, a execução local usa:

```text
src/Oficina.Api/appsettings.Development.json
```

### Aplicar migrations

```powershell
dotnet ef database update --project src/Oficina.Infrastructure --startup-project src/Oficina.Api
```

### Subir a API

```powershell
dotnet run --project src/Oficina.Api/Oficina.Api.csproj
```

### Acesso

| Recurso | URL |
|---|---|
| Swagger | `https://localhost:5001/swagger` |

---

## Opção 2 — Executar com Docker Compose

### Pré-requisito

- Docker Desktop

Essa é a opção recomendada para execução local, porque sobe API, SQL Server e smtp4dev juntos.

### Configurar ambiente

```powershell
Copy-Item docker/.env.example docker/.env
```

Revise o arquivo `docker/.env` se precisar alterar portas, senha do SQL Server, JWT ou credenciais do admin inicial.

### Subir os containers

```powershell
docker compose --env-file docker/.env -f docker/docker-compose.yml up --build
```

Esse comando sobe:

- API;
- SQL Server;
- smtp4dev.

A API usa o host interno `sqlserver` na connection string e, quando `RUN_MIGRATION=true`, aplica migrations automaticamente na inicialização para facilitar a execução local.

### Acessos

| Recurso | URL |
|---|---|
| Swagger | `http://localhost:8080/swagger` |
| Healthcheck | `http://localhost:8080/health` |
| smtp4dev | `http://localhost:5000` |

---

## Opção 3 — Executar com Kubernetes Local (Minikube)

### Pré-requisitos

- Docker Desktop
- kubectl
- Minikube

Os manifests locais ficam em:

```text
k8s/local
```

### Subir o ambiente

```powershell
minikube start
minikube addons enable metrics-server
minikube docker-env | Invoke-Expression
docker build -t oficina-api:local -f docker/Dockerfile .
kubectl apply -f k8s/local
kubectl get pods -n oficina
kubectl get svc -n oficina
```

> O comando `minikube docker-env | Invoke-Expression` deve ser executado antes do `docker build`, para que a imagem seja criada dentro do Docker daemon do Minikube.

### Acessar a API

```powershell
kubectl port-forward svc/oficina-api 8080:80 -n oficina
```

Swagger:

```text
http://localhost:8080/swagger
```

### Acessar smtp4dev

Em outro terminal:

```powershell
kubectl port-forward svc/smtp4dev 5000:80 -n oficina
```

Web UI:

```text
http://localhost:5000
```

### Remover ambiente

```powershell
kubectl delete -f k8s/local
```

O HPA em `k8s/local/hpa.yaml` depende do `metrics-server` habilitado no Minikube.

---

## Integração de E-mail com smtp4dev

O projeto usa **smtp4dev** para simular envio e recebimento de e-mails em ambiente local.

Ele é utilizado no fluxo de aprovação/recusa externa de orçamento:

1. a aplicação gera o orçamento;
2. o sistema envia um e-mail ao cliente;
3. o cliente recebe o e-mail no smtp4dev;
4. o cliente clica em Aprovar ou Recusar;
5. a API processa a ação externa;
6. o orçamento e a OS são atualizados.

A interface web do smtp4dev fica disponível em:

```text
http://localhost:5000
```

---

## Fluxos de Negócio

### Autenticação

A aplicação possui três perfis principais:

| Perfil | Autenticação | Acesso |
|---|---|---|
| Cliente | CPF | Consulta recursos próprios e aprova/recusa orçamento próprio |
| Funcionário | CPF + senha | Opera os fluxos internos da oficina |
| Admin | CPF + senha | Opera a oficina e gerencia funcionários/admins |

O token JWT define o perfil e as permissões do usuário.

Clientes só acessam recursos vinculados ao próprio cadastro. Funcionários e admins acessam rotas internas da oficina.

---

### Abertura da Ordem de Serviço

A OS pode ser aberta com:

- cliente;
- veículo;
- serviços;
- peças;
- insumos.

Toda OS nasce com status:

```text
Recebida
```

Nesse momento, o tipo da OS ainda não precisa estar definido.

---

### Classificação da OS

A OS pode ser classificada como:

| Tipo | Próximo status | Regra |
|---|---|---|
| Preventiva | `AguardandoAprovacao` | Gera orçamento direto |
| Corretiva | `Diagnostico` | Exige diagnóstico antes do orçamento |

---

### Orçamento

Toda OS deve possuir orçamento antes de seguir para execução.

- Preventiva: orçamento é gerado após a classificação.
- Corretiva: orçamento é gerado após o diagnóstico.

---

### Aprovação do Orçamento

Ao aprovar orçamento:

- orçamento é marcado como aprovado;
- estoque é baixado;
- OS muda para `EmExecucao`;
- data de início da execução é registrada.

Ao recusar orçamento:

- orçamento é marcado como recusado;
- OS é finalizada;
- estoque não é baixado.

---

### Finalização e Entrega

Após execução:

```text
EmExecucao → Finalizada → Entregue
```

---

## Rotas da API

### Auth

| Método | Endpoint | Perfil | Descrição |
|---|---|---|---|
| POST | `/api/auth/clientes` | Público | Login do cliente por CPF |
| POST | `/api/auth/funcionarios` | Público | Login interno por CPF e senha |

---

### Cliente autenticado

| Método | Endpoint | Perfil | Descrição |
|---|---|---|---|
| GET | `/api/minhas-ordens-servico` | Cliente | Lista OS do cliente autenticado |
| GET | `/api/minhas-ordens-servico/{id}` | Cliente | Detalha OS própria |
| GET | `/api/minhas-ordens-servico/{id}/status` | Cliente | Consulta status da OS própria |
| GET | `/api/meus-orcamentos/{id}` | Cliente | Consulta orçamento próprio |
| POST | `/api/meus-orcamentos/{id}/aprovar` | Cliente | Aprova orçamento próprio |
| POST | `/api/meus-orcamentos/{id}/recusar` | Cliente | Recusa orçamento próprio |

---

### Administração

| Método | Endpoint | Perfil | Descrição |
|---|---|---|---|
| GET | `/api/admin/funcionarios` | Admin | Lista funcionários/admins |
| POST | `/api/admin/funcionarios` | Admin | Cria funcionário/admin |
| GET | `/api/admin/funcionarios/{id}` | Admin | Consulta funcionário/admin |
| PUT | `/api/admin/funcionarios/{id}` | Admin | Atualiza funcionário/admin |
| PATCH | `/api/admin/funcionarios/{id}/alterar-senha` | Admin | Altera senha |
| PATCH | `/api/admin/funcionarios/{id}/ativar` | Admin | Ativa usuário |
| PATCH | `/api/admin/funcionarios/{id}/inativar` | Admin | Inativa usuário |

---

### Cadastro

| Método | Endpoint | Perfil | Descrição |
|---|---|---|---|
| GET | `/api/clientes` | Funcionário/Admin | Lista clientes |
| POST | `/api/clientes` | Funcionário/Admin | Cria cliente |
| GET | `/api/clientes/{id}` | Funcionário/Admin | Consulta cliente |
| PUT | `/api/clientes/{id}` | Funcionário/Admin | Atualiza cliente |
| GET | `/api/veiculos` | Funcionário/Admin | Lista veículos |
| POST | `/api/veiculos` | Funcionário/Admin | Cria veículo |
| GET | `/api/veiculos/{id}` | Funcionário/Admin | Consulta veículo |
| PUT | `/api/veiculos/{id}` | Funcionário/Admin | Atualiza veículo |

---

### Catálogo

| Método | Endpoint | Perfil | Descrição |
|---|---|---|---|
| GET | `/api/servicos` | Funcionário/Admin | Lista serviços |
| POST | `/api/servicos` | Funcionário/Admin | Cria serviço |
| GET | `/api/servicos/{id}` | Funcionário/Admin | Consulta serviço |
| PUT | `/api/servicos/{id}` | Funcionário/Admin | Atualiza serviço |
| GET | `/api/pecas` | Funcionário/Admin | Lista peças |
| POST | `/api/pecas` | Funcionário/Admin | Cria peça |
| GET | `/api/pecas/{id}` | Funcionário/Admin | Consulta peça |
| PUT | `/api/pecas/{id}` | Funcionário/Admin | Atualiza peça |
| GET | `/api/insumos` | Funcionário/Admin | Lista insumos |
| POST | `/api/insumos` | Funcionário/Admin | Cria insumo |
| GET | `/api/insumos/{id}` | Funcionário/Admin | Consulta insumo |
| PUT | `/api/insumos/{id}` | Funcionário/Admin | Atualiza insumo |

---

### Estoque

| Método | Endpoint | Perfil | Descrição |
|---|---|---|---|
| GET | `/api/estoque` | Funcionário/Admin | Consulta estoque |
| GET | `/api/estoque/pecas/{pecaId}` | Funcionário/Admin | Consulta estoque da peça |
| POST | `/api/estoque/pecas/{pecaId}/ajustar` | Funcionário/Admin | Ajusta estoque da peça |
| GET | `/api/estoque/insumos/{insumoId}` | Funcionário/Admin | Consulta estoque do insumo |
| POST | `/api/estoque/insumos/{insumoId}/ajustar` | Funcionário/Admin | Ajusta estoque do insumo |

---

### Ordens de Serviço

| Método | Endpoint | Perfil | Descrição |
|---|---|---|---|
| GET | `/api/ordens-servico` | Funcionário/Admin | Lista OS abertas |
| POST | `/api/ordens-servico` | Funcionário/Admin | Abre OS completa |
| GET | `/api/ordens-servico/{id}` | Funcionário/Admin | Detalha OS |
| GET | `/api/ordens-servico/{id}/status` | Funcionário/Admin | Consulta status |
| POST | `/api/ordens-servico/{id}/classificar` | Funcionário/Admin | Classifica OS |
| POST | `/api/ordens-servico/{id}/diagnostico` | Funcionário/Admin | Registra diagnóstico |
| POST | `/api/ordens-servico/{id}/finalizar` | Funcionário/Admin | Finaliza OS |
| POST | `/api/ordens-servico/{id}/entregar` | Funcionário/Admin | Entrega veículo |

---

### Orçamentos

| Método | Endpoint | Perfil | Descrição |
|---|---|---|---|
| GET | `/api/orcamentos/{id}` | Funcionário/Admin | Consulta orçamento |
| POST | `/api/orcamentos/{id}/aprovar` | Funcionário/Admin | Aprova orçamento |
| POST | `/api/orcamentos/{id}/recusar` | Funcionário/Admin | Recusa orçamento |
| GET | `/api/orcamentos/acoes-externas/aprovar?token=...` | Público | Aprovação externa via token |
| GET | `/api/orcamentos/acoes-externas/recusar?token=...` | Público | Recusa externa via token |

---

### Relatórios e Health

| Método | Endpoint | Perfil | Descrição |
|---|---|---|---|
| GET | `/api/relatorios/tempo-medio-execucao` | Funcionário/Admin | Tempo médio de execução |
| GET | `/health` | Público | Status da API |

---

## Collection Postman

Arquivos:

```text
postman/OficinaAPI-cenarios.postman_collection.json
postman/OficinaAPI-cenarios.postman_environment.json
postman/OficinaAPI-seguranca.postman_collection.json
```

### Como usar

1. Importe a collection e o environment no Postman.
2. Configure:
   - `baseUrl`;
   - `adminCpf`;
   - `adminSenha`.
3. Execute pelo **Collection Runner**.

A collection gera dados de teste, autentica usuários e preenche tokens/IDs automaticamente.

---

## Testes e Cobertura

### Rodar testes

```powershell
dotnet test
```

### Gerar cobertura

```powershell
dotnet test --collect:"XPlat Code Coverage"
```

Os testes cobrem:

- regras de domínio;
- casos de uso;
- controllers;
- autorização;
- fluxos principais da OS.

---

## Documentação da API

A documentação da API, contendo fluxos, contratos e organização dos endpoints, está disponível no Miro:

```text
https://miro.com/app/board/uXjVGPRzlmM=/
```

---

## Vídeo de Demonstração

```text
https://www.youtube.com/watch?v=8kAxO5SFWiY
```

---

## Evolução do Projeto

Este repositório contém a base funcional construída e avaliada nas **Fases 1 e 2** do Tech Challenge FIAP.

As próximas fases da pós-graduação em Software Architecture evoluem esta solução em repositórios separados, com foco em:

- API Gateway;
- autenticação serverless;
- infraestrutura como código;
- Kubernetes em nuvem;
- banco gerenciado;
- observabilidade;
- pipelines de CI/CD;
- deploy automatizado.

---

## Observações Finais

- Projeto iniciado no contexto do Tech Challenge FIAP e evoluído como aplicação backend de portfólio.
- Este repositório representa a base funcional das Fases 1 e 2.
- Valores locais não devem ser usados em produção.
- Secrets devem ser configuradas por variáveis de ambiente ou ferramentas próprias.
- O banco usa migrations do Entity Framework Core.
- `RUN_MIGRATION=true` é recomendado apenas para ambiente local/demonstração.
- O primeiro admin deve ser criado via bootstrap/configuração segura.
