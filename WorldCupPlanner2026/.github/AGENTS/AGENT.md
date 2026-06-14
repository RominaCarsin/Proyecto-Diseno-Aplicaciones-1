# Instrucciones del Agente de IA — WorldCupPlanner 2026

## Contexto del proyecto

Proyecto académico **Obligatorio 2 — Diseño de Aplicaciones 1 (Universidad ORT Uruguay)**.
Aplicación web para planificación del Mundial de Fútbol 2026.
Stack: C# · .NET 8 · Blazor Server · EF Core · Azure SQL Edge · Docker · MSTest.
Deadline: **17/06/2026 a las 21:00hs**.

---

## Skills disponibles

| Archivo | Nombre | Cuándo usarla |
|---|---|---|
| `tdd-missing-tests.md` | tdd-missing-tests | Completar cobertura de tests de una clase C# siguiendo ciclo RED→GREEN |
| `method-inventory.md` | method-inventory | Generar inventario de métodos públicos de dominio y servicios |
| `readme-generator.md` | readme-generator | Generar o actualizar el README.md del repositorio |
| `project-progress.md` | project-status-reporter | Reportar estado de avance del proyecto vs. requerimientos |
| `design-review.md` | design-review | Revisar diseño de clases contra principios GRASP/SOLID/Demeter |

---

## Principios de trabajo

### Código
- Siempre seguir TDD: escribir el test antes que el código de producción
- Un test a la vez, ciclo RED → commit → GREEN → commit
- Cambios mínimos: usar `str_replace` quirúrgico, nunca reescribir archivos completos
- Respetar la arquitectura en capas: Domain → IRepository/IServices → Repository/Services → Web
- No agregar lógica de negocio en la capa Web (Blazor pages)
- Inyección de dependencias siempre por constructor

### Principios de diseño a respetar
- **SRP**: una sola responsabilidad por clase
- **OCP**: extensible sin modificar (usar Strategy/herencia para nuevos tipos)
- **Bajo acoplamiento / Alta cohesión** (GRASP)
- **Ley de Demeter**: no encadenar más de un nivel de navegación
- **GRASP Controlador**: los servicios coordinan, el dominio decide

### Tests
- Proyecto de tests separado por capa (`Domain.Test`, `Services.Tests`, `Repository.Tests`)
- Usar `FakeRepository<T>` para tests de servicios, nunca mocks de EF
- Nomenclatura: `Sujeto_Condición_ResultadoEsperado`
- Cobertura mínima esperada: ≥90% en nuevas features

### Git
- Flujo GitFlow: `main` → `develop` → `feature/*`
- Commits atómicos con prefijo convencional: `feat`, `fix`, `refactor`, `test`, `docs`
- Commits RED con prefijo `red:` y GREEN con prefijo `green:`
- No commitear directamente a `main` ni a `develop`

---

## Lo que el agente NO debe hacer

- Modificar archivos de migración EF generados automáticamente
- Agregar dependencias NuGet sin consultar al equipo
- Reescribir archivos de configuración (`appsettings.json`, `Program.cs`) sin confirmar
- Saltarse la pausa RED en el ciclo TDD
- Generar código en la capa Web que contenga lógica de negocio
- Commitear a `main` o `develop` directamente

---

## Estructura de ramas activas

| Rama | Propósito |
|---|---|
| `main` | Release estable — solo merge final con tag |
| `develop` | Integración continua |
| `feature/add-skills` | Skills e instrucciones IA |
| `feature/dynamic-ranking-system` | Ranking ELO |
| `feature/notification-system` | Notificaciones |
| `feature/audit-export` | Exportación + auditoría extendida |
| `feature/role-journalist` | Rol Periodista |
