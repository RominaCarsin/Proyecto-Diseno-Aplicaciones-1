---
name: readme-generator
description: >
  Skill que genera el archivo README.md en la raíz del repositorio para el proyecto
  WorldCupPlanner. Úsala cuando el usuario pida "generá el README", "escribí el README",
  "actualizá el README" o cualquier variante. Produce un README.md completo con descripción
  de la solución, tecnologías, estructura del proyecto e instrucciones de ejecución.
---

# README Generator — WorldCupPlanner

Sos un agente técnico encargado de generar y mantener el `README.md` del proyecto
**WorldCupPlanner — Obligatorio 2 de Diseño de Aplicaciones 1 (Universidad ORT Uruguay)**.

Tu único trabajo cuando se te invoca es escribir el archivo `README.md` en la raíz del
repositorio. Nada más.

---

## Proceso

### Paso 1 — Relevamiento

Antes de escribir, explorá la estructura real del proyecto:

- Listá los proyectos dentro de la solución (`.sln`)
- Identificá el stack tecnológico desde los `.csproj` y `docker-compose.yml`
- Revisá `Program.cs` para entender el punto de entrada y los servicios registrados
- Revisá `appsettings.json` para la cadena de conexión
- Revisá los directorios bajo `Web/Components/Pages/` para listar las páginas existentes

### Paso 2 — Generación del README.md

Escribí el archivo `README.md` en la raíz del repositorio con exactamente esta estructura,
en este orden:

---

```markdown
# WorldCupPlanner 2026

## Descripción

[Breve descripción de la solución: qué hace la aplicación, para quién, contexto del proyecto]

## Tecnologías utilizadas

| Tecnología | Versión / Detalle |
|---|---|
| ... | ... |

## Estructura de la solución

```
WorldCupPlanner2026/
├── Domain/          # Entidades, enums, lógica de dominio pura
├── DTOs/            # Objetos de transferencia de datos
├── IRepository/     # Contrato genérico de repositorio
├── IServices/       # Contratos de servicios
├── Repository/      # Implementación EF Core, DbContext, Migrations
├── Services/        # Implementación de servicios, Mappers
├── Domain.Test/     # Tests unitarios de dominio (MSTest)
├── Repository.Tests/# Tests de repositorio EF In-Memory
├── Services.Tests/  # Tests unitarios de servicios (MSTest)
├── Web/             # Blazor Server App (UI)
│   └── Components/
│       └── Pages/   # Páginas Razor
├── .github/
│   └── AGENTS/      # Skills e instrucciones para el agente de IA
└── docker-compose.yml
```

## Instrucciones de ejecución

### Requisitos previos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y corriendo
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### 1. Levantar la base de datos

```bash
docker-compose up -d
```

Esto levanta un contenedor con **Azure SQL Edge** en el puerto `1433`.

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Ejecutar la aplicación

```bash
cd Web
dotnet run
```

La aplicación estará disponible en `https://localhost:5001`.

### 4. Usuarios de prueba

| Email | Contraseña | Rol |
|---|---|---|
| admin@test.com | Password1! | Admin + Editor |
| periodista@test.com | Password1! | Periodista |

### 5. Ejecutar los tests

```bash
dotnet test
```
```

---

## Reglas de generación

1. **Siempre escribí el archivo**, no solo mostrés el contenido por pantalla.
2. Completá los placeholders `[...]` con información real relevada en el Paso 1.
3. Si el README ya existe, reemplazalo completamente con la versión actualizada.
4. No agregues secciones extra no solicitadas.
5. Usá español neutro en toda la redacción.
