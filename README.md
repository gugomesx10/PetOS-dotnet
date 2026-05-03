# PetOS API (.NET 8)

API RESTful para gerenciamento de **pets, vacinas, rotinas e alertas**, usando ASP.NET Core Web API com arquitetura em camadas, Entity Framework Core e Oracle.

## Tecnologias

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- Oracle Database (`Oracle.EntityFrameworkCore`)
- Swagger / OpenAPI

## Estrutura do projeto

```text
PetOS/
├── Controllers/
├── Services/
├── Repositories/
├── Models/
├── DTOs/
├── Data/
│   ├── AppDbContext.cs
│   └── AppDbContextFactory.cs
├── Exceptions/
├── Mappings/
├── Migrations/
├── Program.cs
└── appsettings.json
```

## Modelo de dominio

- `Pet` (1:N) `Vacina`
- `Pet` (1:N) `Rotina`
- `Pet` (1:N) `Alerta`
- `Rotina` (1:N) `Alerta` (opcional)

## Endpoints principais

Base URL: `/api`

### Pets

- `GET /api/pets`
- `GET /api/pets/{id}`
- `GET /api/pets/especie/{especie}`
- `GET /api/pets/responsavel/{responsavel}`
- `POST /api/pets`
- `PUT /api/pets/{id}`
- `DELETE /api/pets/{id}`

### Vacinas

- `GET /api/vacinas`
- `GET /api/vacinas/{id}`
- `GET /api/vacinas/pet/{petId}`
- `POST /api/vacinas`
- `PUT /api/vacinas/{id}`
- `DELETE /api/vacinas/{id}`

### Rotinas

- `GET /api/rotinas`
- `GET /api/rotinas/{id}`
- `GET /api/rotinas/pet/{petId}`
- `POST /api/rotinas`
- `PUT /api/rotinas/{id}`
- `DELETE /api/rotinas/{id}`

### Alertas

- `GET /api/alertas`
- `GET /api/alertas/{id}`
- `GET /api/alertas/pet/{petId}`
- `POST /api/alertas`
- `PUT /api/alertas/{id}`
- `DELETE /api/alertas/{id}`

## Retornos HTTP implementados

- `200 OK` em consultas GET
- `201 Created` em criacao de recursos (POST)
- `204 NoContent` em atualizacao/remocao (PUT/DELETE)
- `400 BadRequest` em validacoes e referencias invalidas
- `404 NotFound` para recursos inexistentes

## Configuracao do banco Oracle

A connection string `OracleDb` esta em:

- `PetOS/appsettings.json`
- `PetOS/appsettings.Development.json`

> Recomendacao: mover senha para User Secrets ou variavel de ambiente em ambientes reais.

## Como executar

```powershell
cd C:\Rider\PetOS\PetOS
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```

Swagger (em Development):

- `https://localhost:xxxx/swagger`

## Troubleshooting Oracle

Se ocorrer `ORA-00955` durante migration, significa que ja existem objetos com o mesmo nome.

- Opcao 1: usar outro schema/usuario Oracle limpo.
- Opcao 2: remover manualmente as tabelas `ALERTAS`, `VACINAS`, `ROTINAS`, `PETS` e executar `dotnet ef database update` novamente.

## Git Flow sugerido

Branches:

- `main`
- `develop`
- `feature/pet-crud`
- `feature/vaccine-module`
- `feature/routine-module`
- `feature/alert-module`
- `feature/swagger`
- `release/v1.0.0`

Fluxo:

- `feature/* -> develop -> release/v1.0.0 -> main`

Exemplo de comandos:

```powershell
git checkout -b develop
git checkout -b feature/pet-crud
git checkout develop
git merge --no-ff feature/pet-crud
git checkout -b release/v1.0.0
git checkout main
git merge --no-ff release/v1.0.0
```
