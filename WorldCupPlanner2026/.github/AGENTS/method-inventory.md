---
name: method-inventory
description: >
  Skill de solo lectura que analiza el proyecto y genera un inventario de métodos públicos
  de las clases de dominio y servicios. Úsala cuando el usuario pida "listá los métodos",
  "generá el inventario de métodos", "qué métodos públicos tiene X clase" o cualquier
  variante. Produce un archivo Metodos.md agrupado por clase. No modifica ningún archivo
  de código fuente.
---

# Skill: Inventario de Métodos Públicos

## Objetivo

Analizar un proyecto de software únicamente en modo lectura y generar un "Metodos.md" de los métodos públicos definidos en las clases de dominio y de servicios.

---

## Responsabilidades

- Recorrer el código fuente del proyecto.
- Identificar las clases pertenecientes al dominio.
- Identificar las clases pertenecientes a servicios.
- Listar los métodos públicos declarados en cada clase encontrada.
- Agrupar los resultados por clase.

---

## Restricciones

### Debe

- Leer exclusivamente el código fuente.
- Mantener la estructura de clases encontrada en el proyecto.
- Mostrar únicamente métodos públicos.
- Agrupar los métodos por clase.

### No debe

- Modificar archivos.
- Crear, editar o eliminar código.
- Ejecutar la aplicación.
- Realizar refactorizaciones.
- Analizar lógica interna de los métodos.
- Incluir métodos privados, protegidos o internos.
- Incluir propiedades, atributos o campos.
- Incluir constructores salvo que se solicite explícitamente.
- Incluir controladores, repositorios, DTOs, mappers, migraciones, infraestructura, configuración o pruebas, salvo que se solicite explícitamente.
- Commitear a git o realizar cualquier acción de control de versiones.

---

## Criterios de Identificación

### Clases de Dominio

Incluye clases que representen entidades o reglas de negocio, por ejemplo:

- Entity
- Model
- Domain Object
- Aggregate
- Value Object

### Clases de Servicios

Incluye clases que representen lógica de negocio o casos de uso, por ejemplo:

- Service
- Application Service
- Domain Service
- Business Service

---

## Formato de Salida

```text
Clase: Usuario

- CrearUsuario()
- ActualizarDatos()
- CambiarPassword()
- ObtenerNombreCompleto()

Clase: ProyectoService

- CrearProyecto()
- ObtenerProyectoPorId()
- ActualizarProyecto()
- EliminarProyecto()
```

---

## Procedimiento

1. Recorrer la estructura del proyecto.
2. Identificar las clases de dominio.
3. Identificar las clases de servicios.
4. Para cada clase encontrada:
    - Obtener el nombre de la clase.
    - Identificar los métodos públicos declarados.
    - Registrar únicamente el nombre del método y su firma básica.
5. Agrupar los métodos por clase.
6. Generar el inventario final.

---

## Formato Final Esperado

```text
Clase: Cliente

- ActualizarDireccion()
- CambiarEstado()
- ObtenerNombreCompleto()

Clase: ReservaService

- CrearReserva()
- CancelarReserva()
- ObtenerReserva()

Clase: FacturaService

- GenerarFactura()
- EnviarFactura()
- ObtenerFactura()
```

---

## Regla Fundamental

La salida debe contener únicamente:

- Nombre de la clase.
- Lista de métodos públicos de la clase.

No agregar explicaciones, análisis, sugerencias, documentación ni comentarios adicionales.
