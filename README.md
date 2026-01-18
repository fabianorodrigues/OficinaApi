# Oficina API (.NET 9) — Monolito (Clean Architecture + DDD)


## Tecnologias Utilizadas
- ** Linguagem escolhida: .NET (C#)
- ** Arquitetura escolhida: Clean Architecture
- ** Banco de Dados: MS Sql Server


Solução monolítica em camadas:
- **Oficina.Api** (Web / Controllers / Swagger / JWT)
- **Oficina.Application** (UseCases, contratos, validações, abstrações)
- **Oficina.Domain** (DDD: Aggregates, Entidades, Value Objects, regras)
- **Oficina.Infrastructure** (EF Core + SQL Server, Repositórios)

Contextos delimitados:
- **Cadastro**: Cliente e Veículo (pré-requisito para abrir OS)
- **Catálogo & Estoque**: Serviço, Peça, Insumo e Estoque
- **Oficina (Core Domain)**: Ordem de Serviço, Diagnóstico, Orçamento, Execução, Finalização, Entrega, Relatórios

---

## Instruções para configurar .Net 9

### 1) Remover a versão atual (caso não seja a versão 9.0.1)
```bash
dotnet-ef --version
dotnet tool uninstall --global dotnet-ef
```

### 2) Instalar a versão 9.0.1
```bash
dotnet tool install --global dotnet-ef --version 9.0.1
```

## Instruções para rodar local

### 1) Subir SQL Server (Docker)
Na raiz:

```bash
docker compose -f docker/docker-compose.yml up -d sqlserver
```

### 2) Aplicar migrations
Na raiz:

```bash
dotnet ef database update -p src/Oficina.Infrastructure/Oficina.Infrastructure.csproj -s src/Oficina.Api/Oficina.Api.csproj
```

### 3) Rodar API
```bash
dotnet run --project src/Oficina.Api
```

### 4) Swagger
Abra:
- `http://localhost:<porta>/swagger`
---

## Rodar via Docker (API + SQL Server)

### 1) Subir no Docker (API + SQL Server)
Na raiz:

```bash
docker compose -f docker/docker-compose.yml up --build
```

### 2) Aplicar migrations
Na raiz:

```bash
dotnet ef database update -p src/Oficina.Infrastructure/Oficina.Infrastructure.csproj -s src/Oficina.Api/Oficina.Api.csproj
```

### 3) Swagger:
- `http://localhost:8080/swagger`

> **migrations não rodam automaticamente** no start do container.
> Você pode rodar migrations apontando para o SQL do compose, localmente, com `dotnet ef database update`.

---

## Autenticação (JWT simples para testes)

O domínio assume usuário interno (quem chama já está autenticado).
Para facilitar, existe um login simples:

`POST /api/auth/login`

```json
{ "usuario": "admin", "senha": "admin" }
```

No Swagger: **Authorize** → `Bearer {token}`

---

## Fluxo e Endpoints

1) Cadastro
- `POST /api/clientes`
- `POST /api/veiculos`

2) Catálogo & estoque
- `POST /api/pecas`
- `POST /api/insumos`
- `POST /api/servicos`
- `POST /api/estoque/pecas/{pecaId}/ajustar`
- `POST /api/estoque/insumos/{insumoId}/ajustar`

3) Oficina
- OS Preventiva: `POST /api/ordens-servico/preventiva` (gera orçamento imediato)
- OS Corretiva: `POST /api/ordens-servico/corretiva` (em diagnóstico)
- Diagnóstico + serviços: `POST /api/ordens-servico/{id}/diagnosticos` (gera orçamento)
- Aprovar orçamento: `POST /api/orcamentos/{id}/aprovar` (baixa estoque e inicia execução)
- Recusar orçamento: `POST /api/orcamentos/{id}/recusar` (finaliza OS sem cobrança de diagnóstico)
- Finalizar OS: `POST /api/ordens-servico/{id}/finalizar` (registra fim execução)
- Entregar OS: `POST /api/ordens-servico/{id}/entregar`
---

## Testes
```bash
dotnet test
```

Cobertura:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Documentações

- [Miro do projeto](https://miro.com/app/board/uXjVGPRzlmM=/)
