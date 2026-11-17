# MoveSmart — Backend C# (.NET 8)

> Mobilidade Urbana Inteligente · **Global Solution 2026.2 — CitizenTech** · FIAP · C# Software Development

## Integrantes

- Augusto Ferreira — RM 557709
- Heitor Prestes — RM 554823

## Problema e solução

Usuários do transporte público de São Paulo enfrentam atrasos, superlotação e falta de informações confiáveis sobre as viagens, o que dificulta o planejamento dos deslocamentos diários. (ODS 9 e ODS 11)

O **MoveSmart** é a plataforma da CitizenTech que ajuda o passageiro a decidir se vale a pena esperar um ônibus ou procurar outra alternativa. Este repositório contém a **camada de backend em C#/.NET**, responsável por:

- cadastro de **usuários**;
- consulta de **linhas** de ônibus (base inicial com ônibus municipais de SP, SPTrans);
- **favoritos** de cada usuário;
- **relatos de lotação** feitos pelos passageiros e um **indicador de lotação** (Baixa / Média / Alta) calculado a partir deles.

> **Limitação importante:** a API Olho Vivo (SPTrans) não informa quantas pessoas há dentro de cada ônibus. Por isso o indicador de lotação **não é um dado oficial**: é uma estimativa baseada em relatos voluntários da última hora, e a resposta da API deixa isso explícito no campo `fonte`.


## Tecnologias

.NET 8 · ASP.NET Core Web API · Entity Framework Core 8 · SQLite · Swagger (Swashbuckle) · xUnit + Moq

## Arquitetura

```
MoveSmart.Domain/          Entidades, enums e exceções de domínio (regras e validações)
MoveSmart.Application/     DTOs, interfaces (repositórios/serviços) e serviços de aplicação
MoveSmart.Infrastructure/  DbContext, repositórios EF Core, seed e injeção de dependência
MoveSmart.Api/             Controllers, middleware de erros, Swagger e Program.cs
MoveSmart.Tests/           Testes unitários (xUnit + Moq)
```

Dependências entre camadas: `Api → Application, Infrastructure` · `Infrastructure → Application, Domain` · `Application → Domain`.
A camada de aplicação depende apenas de **interfaces**; as implementações (EF Core) são injetadas na inicialização.

## Como executar

**Pré-requisitos:** [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0) e Visual Studio 2022 (ou apenas o terminal).

### Pelo Visual Studio
1. Abra `MoveSmart.sln`.
2. Defina **MoveSmart.Api** como projeto de inicialização e pressione **F5**.
3. O Swagger abre em `http://localhost:5080/swagger`.

### Pelo terminal
```bash
dotnet restore
dotnet run --project MoveSmart.Api
# Swagger: http://localhost:5080/swagger
```

O banco **SQLite** (`movesmart.db`) é criado automaticamente na primeira execução, com algumas linhas de exemplo. Nenhuma instalação de banco é necessária.

### Migrations (EF Core)
Se o repositório ainda não tiver a pasta `MoveSmart.Infrastructure/Persistence/Migrations`, gere a migration inicial (uma única vez):

```bash
dotnet tool install --global dotnet-ef --version 8.*
dotnet ef migrations add Inicial --project MoveSmart.Infrastructure --startup-project MoveSmart.Api --output-dir Persistence/Migrations
```
No Visual Studio (Package Manager Console):
```
Add-Migration Inicial -Project MoveSmart.Infrastructure -StartupProject MoveSmart.Api -OutputDir Persistence/Migrations
```
Ao iniciar, a aplicação aplica as migrations existentes automaticamente.

### Testes
```bash
dotnet test
```

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/usuarios` | Cadastra usuário (`nome`, `email`) |
| GET | `/api/usuarios/{id}` | Consulta usuário |
| GET | `/api/linhas?busca=` | Lista linhas (busca por código, origem ou destino) |
| GET | `/api/linhas/{id}` | Consulta linha |
| POST | `/api/linhas` | Cadastra linha |
| GET | `/api/linhas/mais-favoritadas?top=5` | Ranking de linhas mais favoritadas |
| GET | `/api/usuarios/{usuarioId}/favoritos` | Lista favoritos do usuário |
| POST | `/api/usuarios/{usuarioId}/favoritos` | Adiciona favorito (`linhaId`) |
| DELETE | `/api/usuarios/{usuarioId}/favoritos/{linhaId}` | Remove favorito |
| POST | `/api/linhas/{linhaId}/relatos-lotacao` | Registra relato (`usuarioId`, `nivel`: Baixa/Media/Alta, `observacao`) |
| GET | `/api/linhas/{linhaId}/lotacao` | Lotação estimada pelos relatos da última hora |

Status utilizados: `200`, `201`, `204`, `400` (validação/regra), `404` (não encontrado), `409` (duplicidade), `500`.

### Exemplo de uso
```http
POST /api/usuarios
{ "nome": "Ana Souza", "email": "ana@email.com" }

POST /api/linhas/1/relatos-lotacao
{ "usuarioId": 1, "nivel": "Alta", "observacao": "Lotado na porta traseira" }

GET /api/linhas/1/lotacao
{
  "linhaId": 1, "codigoLinha": "8000-10", "nivelEstimado": "Alta",
  "totalRelatos": 1, "janelaMinutos": 60,
  "distribuicao": { "Alta": 1 },
  "fonte": "Relatos voluntários de passageiros (estimativa do MoveSmart, não é dado oficial da SPTrans)"
}
```

### Regra do indicador de lotação
Considera os relatos dos últimos 60 minutos. Cada relato recebe peso `1 / (1 + minutosAtrás / 15)` (relatos recentes pesam mais). A média ponderada (Baixa=1, Média=2, Alta=3) vira: `< 1,67` Baixa · `< 2,34` Média · demais Alta. Sem relatos, `nivelEstimado` é `null`.

## Evidências de funcionamento
Prints do Swagger, das requisições e da execução dos testes estão em [`docs/evidencias`](docs/evidencias).
![Testes](docs/evidencias/01-testes-passando.png)

![Endpoints](docs/evidencias/02-swagger.png)

![Post Usuario](docs/evidencias/03-post-usuario-201.png)

![Get Usuario](docs/evidencias/04-get-usuario-200.png)

![Get Linhas vazio](docs/evidencias/05-get-linhas-vazio-200.png)

![Get Linhas 8000](docs/evidencias/06-get-linhas-8000-200.png)

![Post Favorito](docs/evidencias/07-post-favorito-1-201.png)

![Get Favorito](docs/evidencias/08-get-favorito-1-200.png)

![Post Relatos](docs/evidencias/09-post-relatos-lotacao-1-201.png)

![Get Lotação](docs/evidencias/10-get-lotacao-1-200.png)

![Get Mais Favoritados](docs/evidencias/11-get-mais-favoritados-200.png)

![Delete](docs/evidencias/12-delete-1-204.png)

![Post Email Duplicado](docs/evidencias/13-post-email-duplicado-409.png)

![Post Validação](docs/evidencias/14-post-validacao-400.png)

![Get Não Encontrado](docs/evidencias/15-get-nao-encontrado-404.png)

![Post Favorito Duplicado](docs/evidencias/16-post-favorito-duplicado-409.png)

![Post Nivel Inválido](docs/evidencias/17-post-nivel-invalido-400.png)

## Próximos passos
Integração assíncrona com a API Olho Vivo (previsão de chegada), autenticação, notificações de atraso.
