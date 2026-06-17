# README Generator Skill

Generates and maintains the `README.md` file for the WorldCupPlanner 2026 project.

## When to use

When the user asks: "generá el README", "escribí el README", "actualizá el README", or any equivalent. Produces a complete README.md with project description, tech stack, solution structure, and execution instructions.

## Process

### 1 — Discovery

Before writing, explore the actual project structure:
- List projects inside the `.sln` file
- Identify the tech stack from `.csproj` files and `docker-compose.yml`
- Review `Web/Program.cs` for entry point and registered services
- Review `Web/appsettings.json` for connection string
- List directories under `Web/Components/Pages/` for existing pages
- Run `dotnet test` and capture the test count

### 2 — Generate README.md

Write `README.md` at the repo root with this structure:

```markdown
# WorldCupPlanner 2026

## Description

[Brief description: what the app does, who it's for, project context]

## Technologies Used

| Technology | Version / Detail |
|---|---|
| ... | ... |

## Solution Structure

[ASCII tree of the solution]

## Execution Instructions

### Prerequisites

- Docker Desktop installed and running
- .NET 8 SDK

### 1. Start the database

docker-compose up -d

### 2. Restore dependencies

dotnet restore

### 3. Run the app

cd Web
dotnet run

App available at https://localhost:5001.

### 4. Test users

| Email | Password | Role |
|---|---|---|
| admin@test.com | Password1! | Admin + Editor |
| periodista@test.com | Password1! | Periodista |

### 5. Run tests

dotnet test
```

## Rules

1. Always write the file, never just display content
2. Replace placeholders with real data discovered in step 1
3. If README already exists, replace it completely
4. Do not add extra sections beyond what's specified
5. Use neutral Spanish for all text
