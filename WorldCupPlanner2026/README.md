# WorldCupPlanner 2026

## Descripción

Aplicación web para la planificación y simulación del Mundial de Fútbol 2026.
Permite gestionar equipos, estadios, usuarios y fixture completo (fase de grupos y eliminatoria),
simular resultados con motores intercambiables, calcular rankings dinámicos mediante fórmula ELO,
enviar notificaciones a periodistas y exportar datos en CSV y XLSX.

Desarrollado como Obligatorio 2 de **Diseño de Aplicaciones 1 — Universidad ORT Uruguay**.

---

## Tecnologías utilizadas

| Tecnología | Versión / Detalle |
|---|---|
| C# / .NET | 8.0 |
| Blazor Server | .NET 8 — UI interactiva server-side |
| Entity Framework Core | 8.0.0 — Code First, Azure SQL Edge |
| Azure SQL Edge | Docker — base de datos relacional |
| Docker | docker-compose para levantar la BD |
| MSTest | 3.1.1 — tests unitarios e integración |
| ClosedXML | 0.105.0 — Exportación a XLSX |
| CsvHelper | 33.1.0 — Exportación a CSV |
| Blazorise + Bootstrap 5 | 2.2.0 — Componentes UI |
| Blazored.LocalStorage | 4.5.0 — Persistencia de sesión en cliente |
| Moq | 4.20.72 — Mocking en tests de servicios |

---

## Estructura de la solución

```
WorldCupPlanner2026/
├── Domain/              # Entidades, enums y lógica de dominio pura
│   ├── Enums/           # MatchStatus, TournamentPhase, SimulationEngineType
│   └── *.cs             # Team, Match, Group, Knockout, Notification, AuditLog, ...
├── DTOs/                # Objetos de transferencia de datos (Request/Response)
├── IRepository/         # Contrato genérico IRepository<T>
├── IServices/           # Contratos de todos los servicios
├── Repository/          # Implementación EF Core
│   ├── Config/          # Fluent API — configuración de entidades
│   ├── Migrations/      # Migraciones generadas por EF
│   ├── AppDbContext.cs  # DbContext con todos los DbSet
│   └── Repository.cs    # Implementación genérica de IRepository<T>
├── Services/            # Implementación de servicios
│   ├── Mappers/         # Conversión entidad ↔ DTO
│   └── *.cs             # TournamentService, NotificationService, ExportService, ...
├── Domain.Test/         # Tests unitarios de dominio (492 tests)
├── Repository.Tests/    # Tests de repositorio EF In-Memory (7 tests)
├── Services.Tests/      # Tests unitarios de servicios (293 tests)
│   └── FakeRepository.cs# Fake in-memory para tests de servicios
├── Web/                 # Blazor Server App
│   ├── Components/
│   │   └── Pages/       # Error, Export, Group, Home, Import, Knockout, Login,
│   │                    # Logs, Matches, Profile, Ranking, Stadium, Team, Users
│   ├── Program.cs       # DI, EF, seed de datos
│   └── appsettings.json # Cadena de conexión
├── Skills/              # Skills para el agente de IA
├── .github/
│   └── AGENTS/          # Skills e instrucciones para el agente de IA
├── data/                # Volumen persistido de Azure SQL Edge
└── docker-compose.yml   # Orquestación del contenedor de BD
```

---

## Instrucciones de ejecución

### Requisitos previos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y corriendo
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### 1. Clonar el repositorio

```bash
git clone <url-del-repositorio>
cd WorldCupPlanner2026
```

### 2. Levantar la base de datos

```bash
docker-compose up -d
```

Esto levanta un contenedor con **Azure SQL Edge** en el puerto `1433`.
La cadena de conexión ya está configurada en `Web/appsettings.json`.

### 3. Restaurar dependencias

```bash
dotnet restore
```

### 4. Ejecutar la aplicación

```bash
cd Web
dotnet run
```

La aplicación estará disponible en `https://localhost:5001`.
Las migraciones y el seed inicial se aplican automáticamente al iniciar.

### 5. Ejecutar los tests

```bash
dotnet test
```

Resultado esperado: **792 tests en verde** (492 Domain + 293 Services + 7 Repository).
