# Oficina API

![Coverage](./badges/badge_combined.svg)

## Visão Geral

API para gestão de oficina mecânica, cobrindo clientes, veículos, serviços, peças, insumos, estoque, ordens de serviço, orçamentos, autenticação e perfis de acesso.

## Arquitetura

Projeto organizado com Clean Architecture, DDD e Use Cases:

- `Oficina.Api`: controllers, autenticação, autorização, Swagger e middlewares.
- `Oficina.Application`: casos de uso, DTOs, validações e contratos.
- `Oficina.Domain`: entidades, value objects, enums e regras de negócio.
- `Oficina.Infrastructure`: EF Core, repositórios, e-mail e persistência.
- `Oficina.Tests`: testes de domínio, aplicação, API e segurança.

## Tecnologias Utilizadas

- .NET 9
- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT Bearer
- FluentValidation
- xUnit, Moq e Coverlet
- Docker e Docker Compose
- Kubernetes local com Minikube
- smtp4dev
- Swagger/OpenAPI
- GitHub Actions

## Como Executar

Variáveis principais usadas pela aplicação:
| Variável | Uso |
| --- | --- |
| `ConnectionStrings__SqlServer` | Conexão com SQL Server |
| `Jwt__Secret` | Chave JWT com ao menos 32 caracteres |
| `Jwt__Issuer` / `Jwt__Audience` | Emissor e audiência do token |
| `RUN_MIGRATION` | Executa migrations na inicialização |
| `AdminInicial__Nome` / `AdminInicial__Cpf` / `AdminInicial__Senha` | Bootstrap do primeiro admin |

### Opção 1 - Executar Com dotnet

Pré-requisitos:

- .NET SDK 9
- SQL Server LocalDB ou SQL Server acessível

Por padrão, a execução local usa `src/Oficina.Api/appsettings.Development.json`.

Aplicar migrations:

```powershell
dotnet ef database update --project src/Oficina.Infrastructure --startup-project src/Oficina.Api
```

Subir a API:

```powershell
dotnet run --project src/Oficina.Api/Oficina.Api.csproj
```

Acesso:

| Recurso | URL |
| --- | --- |
| Swagger | `https://localhost:5001/swagger` |

### Opção 2 - Executar Com Docker Compose

Pré-requisito:

- Docker Desktop

Essa é a opção recomendada para avaliação, porque sobe API, SQL Server e smtp4dev juntos.

```powershell
Copy-Item docker/.env.example docker/.env
```

Revise `docker/.env` se precisar alterar portas, senha do SQL Server ou credenciais do admin inicial. Depois suba os serviços informando o arquivo de ambiente:

```powershell
docker compose --env-file docker/.env -f docker/docker-compose.yml up --build
```

Esse comando sobe API, SQL Server e smtp4dev. A API usa o host interno `sqlserver` na connection string e executa migrations quando `RUN_MIGRATION=true`.

Acessos:

| Recurso | URL |
| --- | --- |
| Swagger | `http://localhost:8080/swagger` |
| Healthcheck | `http://localhost:8080/health` |
| smtp4dev | `http://localhost:5000` |

### Opção 3 - Executar Com Kubernetes Local (Minikube)

Pré-requisitos:

- Docker Desktop
- kubectl
- Minikube

Os manifests locais ficam em `k8s/local` e usam valores apenas de desenvolvimento. No Minikube, `RUN_MIGRATION=true` aplica migrations na inicialização da API para facilitar a avaliação local.

Subir o ambiente:

```powershell
minikube start
minikube addons enable metrics-server
minikube docker-env | Invoke-Expression
docker build -t oficina-api:local -f docker/Dockerfile .
kubectl apply -f k8s/local
kubectl get pods -n oficina
kubectl get svc -n oficina
```

Importante: `minikube docker-env | Invoke-Expression` deve ser executado antes do `docker build`, para que a imagem seja criada dentro do Docker daemon do Minikube.

Acessar API em um terminal:

```powershell
kubectl port-forward svc/oficina-api 8080:80 -n oficina
```

Swagger: `http://localhost:8080/swagger`

Acessar smtp4dev em outro terminal:

```powershell
kubectl port-forward svc/smtp4dev 5000:80 -n oficina
```

Web UI: `http://localhost:5000`

Remover ambiente:

```powershell
kubectl delete -f k8s/local
```

O HPA em `k8s/local/hpa.yaml` depende do `metrics-server` habilitado no Minikube.

Resumo dos acessos:

| Opção | Swagger | smtp4dev |
| --- | --- |
| dotnet | `https://localhost:5001/swagger` | depende do SMTP configurado localmente |
| Docker Compose | `http://localhost:8080/swagger` | `http://localhost:5000` |
| Minikube | `http://localhost:8080/swagger` após port-forward | `http://localhost:5000` após port-forward |

## Integração De E-mail

- O projeto usa smtp4dev para visualizar e-mails locais.
- Web UI via Docker Compose: `http://localhost:5000`.
- SMTP no Docker Compose: host `smtp4dev`, porta `25`.
- No Minikube, a API usa o Service `smtp4dev` dentro do cluster.
- Os e-mails são usados no fluxo de aprovação/recusa externa de orçamento.

## Fluxos Principais

- Cliente autentica por CPF e acessa apenas as próprias OS/orçamentos.
- Funcionário/Admin autentica por CPF e senha.
- Funcionário cadastra clientes, veículos, catálogo e estoque.
- OS é aberta com cliente, veículo, serviços, peças e insumos.
- OS é classificada como `Preventiva` ou `Corretiva`.
- Preventiva segue para orçamento; corretiva exige diagnóstico antes do orçamento.
- Aprovação do orçamento inicia execução e baixa estoque.
- OS pode ser finalizada e entregue.
- Admin gerencia funcionários e administradores.

## Rotas Da API

| Grupo | Método | Endpoint | Perfil | Descrição |
| --- | --- | --- | --- | --- |
| Auth | POST | `/api/auth/clientes` | Público | Login do cliente por CPF |
| Auth | POST | `/api/auth/funcionarios` | Público | Login interno por CPF e senha |
| Cliente | GET | `/api/minhas-ordens-servico` | Cliente | Lista OS do cliente autenticado |
| Cliente | GET | `/api/minhas-ordens-servico/{id}` | Cliente | Detalha OS própria |
| Cliente | GET | `/api/minhas-ordens-servico/{id}/status` | Cliente | Consulta status de OS própria |
| Cliente | GET | `/api/meus-orcamentos/{id}` | Cliente | Consulta orçamento próprio |
| Cliente | POST | `/api/meus-orcamentos/{id}/aprovar` | Cliente | Aprova orçamento próprio |
| Cliente | POST | `/api/meus-orcamentos/{id}/recusar` | Cliente | Recusa orçamento próprio |
| Admin | GET | `/api/admin/funcionarios` | Admin | Lista funcionários |
| Admin | POST | `/api/admin/funcionarios` | Admin | Cria funcionário/admin |
| Admin | GET | `/api/admin/funcionarios/{id}` | Admin | Consulta funcionário |
| Admin | PUT | `/api/admin/funcionarios/{id}` | Admin | Atualiza funcionário |
| Admin | PATCH | `/api/admin/funcionarios/{id}/alterar-senha` | Admin | Altera senha |
| Admin | PATCH | `/api/admin/funcionarios/{id}/ativar` | Admin | Ativa usuário |
| Admin | PATCH | `/api/admin/funcionarios/{id}/inativar` | Admin | Inativa usuário |
| Cadastro | GET | `/api/clientes` | Funcionário/Admin | Lista clientes |
| Cadastro | POST | `/api/clientes` | Funcionário/Admin | Cria cliente |
| Cadastro | GET | `/api/clientes/{id}` | Funcionário/Admin | Consulta cliente |
| Cadastro | PUT | `/api/clientes/{id}` | Funcionário/Admin | Atualiza cliente |
| Cadastro | GET | `/api/veiculos` | Funcionário/Admin | Lista veículos |
| Cadastro | POST | `/api/veiculos` | Funcionário/Admin | Cria veículo |
| Cadastro | GET | `/api/veiculos/{id}` | Funcionário/Admin | Consulta veículo |
| Cadastro | PUT | `/api/veiculos/{id}` | Funcionário/Admin | Atualiza veículo |
| Catálogo | GET | `/api/servicos` | Funcionário/Admin | Lista serviços |
| Catálogo | POST | `/api/servicos` | Funcionário/Admin | Cria serviço |
| Catálogo | GET | `/api/servicos/{id}` | Funcionário/Admin | Consulta serviço |
| Catálogo | PUT | `/api/servicos/{id}` | Funcionário/Admin | Atualiza serviço |
| Catálogo | GET | `/api/pecas` | Funcionário/Admin | Lista peças |
| Catálogo | POST | `/api/pecas` | Funcionário/Admin | Cria peça |
| Catálogo | GET | `/api/pecas/{id}` | Funcionário/Admin | Consulta peça |
| Catálogo | PUT | `/api/pecas/{id}` | Funcionário/Admin | Atualiza peça |
| Catálogo | GET | `/api/insumos` | Funcionário/Admin | Lista insumos |
| Catálogo | POST | `/api/insumos` | Funcionário/Admin | Cria insumo |
| Catálogo | GET | `/api/insumos/{id}` | Funcionário/Admin | Consulta insumo |
| Catálogo | PUT | `/api/insumos/{id}` | Funcionário/Admin | Atualiza insumo |
| Estoque | GET | `/api/estoque` | Funcionário/Admin | Consulta estoque |
| Estoque | GET | `/api/estoque/pecas/{pecaId}` | Funcionário/Admin | Consulta estoque da peça |
| Estoque | POST | `/api/estoque/pecas/{pecaId}/ajustar` | Funcionário/Admin | Ajusta estoque da peça |
| Estoque | GET | `/api/estoque/insumos/{insumoId}` | Funcionário/Admin | Consulta estoque do insumo |
| Estoque | POST | `/api/estoque/insumos/{insumoId}/ajustar` | Funcionário/Admin | Ajusta estoque do insumo |
| OS | GET | `/api/ordens-servico` | Funcionário/Admin | Lista OS abertas |
| OS | POST | `/api/ordens-servico` | Funcionário/Admin | Abre OS completa |
| OS | GET | `/api/ordens-servico/{id}` | Funcionário/Admin | Detalha OS |
| OS | GET | `/api/ordens-servico/{id}/status` | Funcionário/Admin | Consulta status |
| OS | POST | `/api/ordens-servico/{id}/classificar` | Funcionário/Admin | Classifica OS |
| OS | POST | `/api/ordens-servico/{id}/diagnostico` | Funcionário/Admin | Registra diagnóstico |
| OS | POST | `/api/ordens-servico/{id}/finalizar` | Funcionário/Admin | Finaliza OS |
| OS | POST | `/api/ordens-servico/{id}/entregar` | Funcionário/Admin | Entrega veículo |
| Orçamentos | GET | `/api/orcamentos/{id}` | Funcionário/Admin | Consulta orçamento |
| Orçamentos | POST | `/api/orcamentos/{id}/aprovar` | Funcionário/Admin | Aprova orçamento |
| Orçamentos | POST | `/api/orcamentos/{id}/recusar` | Funcionário/Admin | Recusa orçamento |
| Orçamentos | GET | `/api/orcamentos/acoes-externas/aprovar?token=...` | Público | Aprovação externa |
| Orçamentos | GET | `/api/orcamentos/acoes-externas/recusar?token=...` | Público | Recusa externa |
| Relatórios | GET | `/api/relatorios/tempo-medio-execucao` | Funcionário/Admin | Tempo médio de execução |
| Health | GET | `/health` | Público | Status da API |

## Collection Postman

Arquivos:

- `postman/OficinaAPI-cenarios.postman_collection.json`
- `postman/OficinaAPI-cenarios.postman_environment.json`
- `postman/OficinaAPI-seguranca.postman_collection.json`

Uso:

1. Importe a collection e o environment no Postman.
2. Configure `baseUrl`, `adminCpf` e `adminSenha`.
3. Execute pelo Collection Runner.

A collection gera dados, autentica usuários e preenche tokens/IDs automaticamente.

## Testes E Cobertura

Rodar testes:

```powershell
dotnet test
```

Gerar cobertura:

```powershell
dotnet test --collect:"XPlat Code Coverage"
```

Os testes cobrem regras de domínio, casos de uso, controllers e autorização.

## Links Da Entrega

| Item | Link |
| --- | --- |
| Documentação da API | https://miro.com/app/board/uXjVGPRzlmM=/ |
| Vídeo de Demonstração | https://www.youtube.com/watch?v=8kAxO5SFWiY |


## Observações Finais

- Projeto acadêmico.
- Valores locais não devem ser usados em produção.
- Segredos devem ser configurados por variáveis de ambiente.
- O banco usa migrations do Entity Framework Core.
- O primeiro admin deve ser criado via bootstrap/configuração segura.
