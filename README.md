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

## 📊 Code Coverage

![Coverage](./badges/badge_combined.svg)

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

### 1) Subir SQL Server e smtp4dev (Docker)
Na raiz:

```bash
docker compose -f docker/docker-compose.yml up -d sqlserver smtp4dev
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
- smtp4dev Web: `http://localhost:5001`
---

## Rodar via Docker (API + SQL Server)

### 1) Subir no Docker (API + SQL Server + smtp4dev)
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
- smtp4dev Web: `http://localhost:5001`

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
- `PUT /api/clientes/{id}`
- `GET api/clientes/{id}`
- `POST /api/veiculos`
- `PUT /api/veiculos/{id}`
- `GET api/veiculos/{id}`
- `GET api/veiculos/por-cliente/{clienteId}`

2) Catálogo & estoque
- `POST /api/pecas`
- `GET api/pecas/{id}`
- `PUT /api/pecas/{id}`
- `POST /api/insumos`
- `GET api/insumos/{id}`
- `PUT /api/insumos/{id}`
- `POST /api/servicos`
- `POST /api/estoque/pecas/{pecaId}/ajustar`
- `GET /api/estoque/pecas/{id}`
- `POST /api/estoque/insumos/{insumoId}/ajustar`
- `GET /api/estoque/insumos/{id}`

3) Oficina
- OS Preventiva: `POST /api/ordens-servico/preventiva` (gera orçamento imediato)
- OS Corretiva: `POST /api/ordens-servico/corretiva` (em diagnóstico)
- Diagnóstico + serviços: `POST /api/ordens-servico/{id}/diagnosticos` (gera orçamento)
- Aprovar orçamento: `POST /api/orcamentos/{id}/aprovar` (baixa estoque e inicia execução)
- Recusar orçamento: `POST /api/orcamentos/{id}/recusar` (finaliza OS sem cobrança de diagnóstico)
- Aprovação externa por link: `GET /api/orcamentos/acoes-externas/aprovar?token=...`
- Recusa externa por link: `GET /api/orcamentos/acoes-externas/recusar?token=...`
- Finalizar OS: `POST /api/ordens-servico/{id}/finalizar` (registra fim execução)
- Entregar OS: `POST /api/ordens-servico/{id}/entregar`
---

## Fluxo de ação externa por e-mail (link)

1. Ao criar um orçamento, a aplicação gera um token externo seguro (expiração padrão de 7 dias).
2. O notificador envia e-mail HTML real via SMTP com os links de aprovação e recusa.
3. O cliente usa um dos links externos.
4. A API processa a ação e reaproveita os mesmos UseCases internos de aprovação/recusa.
5. A OS é atualizada com origem da última atualização como `Externa`.

Configuração SMTP + links:
- `EmailSettings:SmtpHost`
- `EmailSettings:SmtpPort`
- `EmailSettings:EnableSsl`
- `EmailSettings:From`
- `EmailSettings:BaseUrl`

Em Docker:
- `EmailSettings__SmtpHost=smtp4dev`
- `EmailSettings__SmtpPort=25`
- `EmailSettings__BaseUrl=http://localhost:8080`

Validações do endpoint externo:
- `404` para link/token inválido;
- `410` para link expirado;
- `409` para orçamento já processado;
- `200` quando ação concluída com sucesso.

### Passo a passo completo para testar com smtp4dev

1. Suba serviços: SQL + smtp4dev (+ API se quiser Docker completo).
2. Gere token criando orçamento (OS preventiva ou diagnóstico de corretiva).
3. Abra `http://localhost:5001` e localize o e-mail enviado para o cliente.
4. Abra o e-mail e clique em **Aprovar** ou **Recusar**.
5. Valide orçamento:
   - `GET /api/orcamentos/{orcamentoId}`.
6. Valide OS:
   - `GET /api/ordens-servico/{osId}`
   - `GET /api/ordens-servico/{osId}/status`
7. Confirme `OrigemUltimaAtualizacaoStatus = Externa`.

### Checklist rápido de demonstração (ponta a ponta)

- [ ] Criar OS preventiva (`POST /api/ordens-servico/preventiva`) e guardar `orcamentoId` + `osId`.
- [ ] Confirmar recebimento do e-mail no smtp4dev (`http://localhost:5001`).
- [ ] Validar que o HTML do e-mail contém ambos os links:
  - `/api/orcamentos/acoes-externas/aprovar?token=...`
  - `/api/orcamentos/acoes-externas/recusar?token=...`
- [ ] Clicar no link **Aprovar** ou **Recusar** no e-mail.
- [ ] Conferir status do orçamento (`GET /api/orcamentos/{orcamentoId}`).
- [ ] Conferir status da OS (`GET /api/ordens-servico/{osId}/status`) e origem `Externa`.
- [ ] Reabrir o mesmo link para validar idempotência funcional (`409 acao_ja_processada`).

### Local x Docker

- **API fora de Docker**:
  - `EmailSettings:SmtpHost=localhost`
  - `EmailSettings:SmtpPort=2525`
  - `EmailSettings:BaseUrl=http://localhost:5000` (ou porta usada pela API local).
- **API em Docker**:
  - `EmailSettings__SmtpHost=smtp4dev` (nome do serviço no compose)
  - `EmailSettings__SmtpPort=25`
  - `EmailSettings__BaseUrl=http://localhost:8080`

### Troubleshooting

- Se e-mail não chega:
  - verifique `SmtpHost`/`SmtpPort` conforme modo de execução (local vs Docker).
- Se link abre URL errada:
  - ajuste `EmailSettings:BaseUrl`.
- Se API em container não conecta no SMTP:
  - não use `localhost`; use `smtp4dev` no compose.

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
