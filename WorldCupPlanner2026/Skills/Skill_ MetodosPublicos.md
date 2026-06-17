# Skill: Inventario de Métodos Públicos

## Objetivo

Analizar un proyecto de software y crear o actualizar un documento Markdown que contenga un inventario de los métodos públicos definidos en las clases de dominio y de servicios.

---

## Responsabilidades

* Recorrer el código fuente del proyecto.
* Identificar las clases pertenecientes al dominio.
* Identificar las clases pertenecientes a servicios.
* Listar los métodos públicos declarados en cada clase encontrada.
* Agrupar los resultados por clase.
* Crear o actualizar el archivo de inventario en formato Markdown.

---

## Restricciones

### Debe

* Leer el código fuente del proyecto.
* Mantener la estructura de clases encontrada en el proyecto.
* Mostrar únicamente métodos públicos.
* Agrupar los métodos por clase.
* Crear el archivo si no existe.
* Actualizar el archivo si ya existe.

### No debe

* Modificar código fuente.
* Crear, editar o eliminar lógica de negocio.
* Ejecutar la aplicación.
* Realizar refactorizaciones.
* Analizar lógica interna de los métodos.
* Incluir métodos privados, protegidos o internos.
* Incluir propiedades, atributos o campos.
* Incluir constructores salvo que se solicite explícitamente.
* Incluir controladores, repositorios, DTOs, mappers, migraciones, infraestructura, configuración o pruebas, salvo que se solicite explícitamente.

---

## Archivo de Salida

Nombre sugerido:

`metodos-publicos.md`

Si el archivo ya existe, actualizar su contenido manteniendo el formato establecido.

---

## Formato del Documento

# Inventario de Métodos Públicos

## Cliente

* ActualizarDireccion()
* CambiarEstado()
* ObtenerNombreCompleto()

## ReservaService

* CrearReserva()
* CancelarReserva()
* ObtenerReserva()

## FacturaService

* GenerarFactura()
* EnviarFactura()
* ObtenerFactura()

---

## Procedimiento

1. Recorrer la estructura del proyecto.
2. Identificar las clases de dominio.
3. Identificar las clases de servicios.
4. Obtener los métodos públicos de cada clase.
5. Crear o actualizar el documento Markdown.
6. Mantener el inventario sincronizado con el código analizado.

---

## Regla Fundamental

El archivo Markdown debe contener únicamente:

* Nombre de la clase.
* Lista de métodos públicos de la clase.

No agregar explicaciones, análisis, sugerencias, documentación ni comentarios adicionales.