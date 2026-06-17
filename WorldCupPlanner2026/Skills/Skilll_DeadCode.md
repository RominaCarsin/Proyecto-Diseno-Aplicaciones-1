# Skill: Detección de Código Muerto, Código No Utilizado y Código Duplicado en C#

## Rol

Actuá como un desarrollador de software experto en **C# / .NET**, con foco en análisis estático, limpieza de código, mantenibilidad y refactorización segura.

Cuando se te pida revisar, analizar o refactorizar código C#, debés detectar de forma estricta posibles casos de:

* Código muerto.
* Código no utilizado.
* Código duplicado.
* Elementos sospechosos que parecen no participar en el flujo real de la aplicación.

El objetivo es mejorar la claridad, reducir deuda técnica y evitar mantener código innecesario.

---

## 1. Código muerto

Considerar como **código muerto** todo fragmento que existe en el proyecto pero no puede ejecutarse o no participa en ningún flujo real.

### 1.1 Métodos privados nunca llamados

Detectar métodos privados que no son invocados desde ningún otro método de la clase.

Ejemplo:

```csharp id="2cq29s"
public class PedidoService
{
    public decimal ObtenerTotal(Pedido pedido) => pedido.Subtotal;

    private decimal CalcularImpuesto(decimal baseImponible)
    {
        return baseImponible * 0.21m;
    }
}
```

En este caso, `CalcularImpuesto` es sospechoso de código muerto si no se llama desde ningún lado.

---

### 1.2 Ramas inalcanzables

Detectar bloques que nunca se ejecutan porque antes ya hubo un `return`, `throw`, `break` o `continue`.

Ejemplo:

```csharp id="w8i2y0"
public string ObtenerEstado(int codigo)
{
    if (codigo == 1)
        return "Activo";

    return "Inactivo";

    if (codigo == 2)
        return "Pendiente";
}
```

La última condición nunca se ejecuta porque el método ya retornó antes.

---

### 1.3 Variables asignadas pero nunca leídas

Detectar variables locales que reciben un valor pero después no se usan.

Ejemplo:

```csharp id="g7r99n"
public void ProcesarOrden(Orden orden)
{
    var timestamp = DateTime.UtcNow;
    var resultado = _repository.Guardar(orden);

    _logger.Log(resultado.Id.ToString());
}
```

En este caso, `timestamp` es innecesaria si no participa en ninguna operación posterior.

---

### 1.4 Catch vacío

Detectar bloques `catch` que capturan una excepción pero no hacen nada útil con ella.

Ejemplo:

```csharp id="bpoee3"
try
{
    _service.Ejecutar();
}
catch (Exception ex)
{
}
```

Un `catch` vacío oculta errores y dificulta el diagnóstico. Si se captura una excepción, debe existir una razón clara: loguear, transformar la excepción, recuperar el flujo o relanzarla.

---

### 1.5 Enum con valores no utilizados

Detectar valores de `enum` que no aparecen en:

* `switch`.
* Comparaciones.
* Asignaciones.
* Persistencia.
* Mapeos.
* Tests.
* UI.
* Serialización o deserialización.

Ejemplo:

```csharp id="h0pjk3"
public enum EstadoPedido
{
    Pendiente,
    Procesando,
    Completado,
    Cancelado,
    EnDisputa
}
```

Si `EnDisputa` no se usa en ningún flujo real, debe marcarse como sospechoso.

---

## 2. Código no utilizado

Considerar como **código no utilizado** todo elemento declarado que podría ser válido sintácticamente, pero no tiene integración real con el sistema.

---

### 2.1 Interfaz sin implementación

Detectar interfaces que no tienen ninguna clase concreta que las implemente.

Ejemplo:

```csharp id="ogz6ha"
public interface IReporteExportador
{
    byte[] Exportar(Reporte reporte);
}
```

Si no existe ninguna clase que implemente `IReporteExportador`, la interfaz puede ser innecesaria o estar incompleta.

---

### 2.2 Implementación no registrada en Dependency Injection

Detectar clases que implementan servicios pero no están registradas en el contenedor de dependencias.

Ejemplo:

```csharp id="4byi7l"
public class CacheDeProductos : ICacheDeProductos
{
}
```

Si `CacheDeProductos` no aparece en ningún registro como:

```csharp id="7g2ecm"
services.AddScoped<ICacheDeProductos, CacheDeProductos>();
services.AddTransient<ICacheDeProductos, CacheDeProductos>();
services.AddSingleton<ICacheDeProductos, CacheDeProductos>();
```

entonces puede no estar siendo usada por la aplicación.

---

### 2.3 Using directive no usado

Detectar `using` que no son necesarios dentro del archivo.

Ejemplo:

```csharp id="xekpip"
using System.Xml.Serialization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
```

Si `System.Xml.Serialization` no se usa en el archivo, debe eliminarse.

---

### 2.4 Parámetros no utilizados

Detectar parámetros que forman parte de la firma de un método pero no se usan dentro del cuerpo.

Ejemplo:

```csharp id="htdwpd"
public IEnumerable<Producto> Buscar(string termino, BusquedaOpciones opciones)
{
    return _context.Productos
        .Where(producto => producto.Nombre.Contains(termino))
        .ToList();
}
```

Si `opciones` no se usa, puede indicar una firma incorrecta, una funcionalidad incompleta o código pendiente.

---

### 2.5 DTO con propiedad no mapeada

Detectar propiedades de DTOs que nunca se asignan en mappers, constructores, controladores o servicios.

Ejemplo:

```csharp id="qaw3k2"
public class ClienteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string CodigoInterno { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
```

Si `CodigoInterno` nunca se asigna ni se lee, debe marcarse como propiedad sospechosa.

---

### 2.6 Extension methods no invocados

Detectar métodos de extensión que no se usan en ningún archivo `.cs` del proyecto.

Ejemplo:

```csharp id="k6m8do"
public static class StringExtensions
{
    public static string ToSlug(this string input)
    {
        return input.ToLower().Replace(" ", "-");
    }
}
```

Si no aparece ninguna llamada a `.ToSlug()`, el método puede ser innecesario.

---

## 3. Código duplicado

Considerar como **código duplicado** todo bloque repetido que representa la misma regla, validación, consulta, transformación o lógica de negocio en más de un lugar.

---

### 3.1 Guard clauses duplicadas

Detectar validaciones repetidas entre servicios, controladores o casos de uso.

Ejemplo de duplicación:

```csharp id="byag4n"
public void Procesar(Pedido pedido)
{
    if (pedido is null)
        throw new ArgumentNullException(nameof(pedido));

    if (pedido.Items is null || !pedido.Items.Any())
        throw new InvalidOperationException("El pedido no tiene ítems.");
}
```

```csharp id="g05l9d"
public void Generar(Pedido pedido)
{
    if (pedido is null)
        throw new ArgumentNullException(nameof(pedido));

    if (pedido.Items is null || !pedido.Items.Any())
        throw new InvalidOperationException("El pedido no tiene ítems.");
}
```

Refactor recomendado:

```csharp id="2ieqv8"
public static class PedidoGuards
{
    private const string PedidoSinItemsErrorMessage = "El pedido no tiene ítems.";

    public static void EnsureValido(Pedido pedido)
    {
        if (pedido is null)
            throw new ArgumentNullException(nameof(pedido));

        if (pedido.Items is null || !pedido.Items.Any())
            throw new InvalidOperationException(PedidoSinItemsErrorMessage);
    }
}
```

---

### 3.2 Mapping manual duplicado

Detectar transformaciones repetidas entre entidades y DTOs.

Ejemplo de duplicación:

```csharp id="3k6182"
var dto = new PedidoDto
{
    Id = pedido.Id,
    Total = pedido.Total,
    FechaCreacion = pedido.FechaCreacion,
    Estado = pedido.Estado.ToString()
};
```

Refactor recomendado:

```csharp id="7ciuzq"
public static class PedidoMapper
{
    public static PedidoDto ToDto(this Pedido pedido)
    {
        return new PedidoDto
        {
            Id = pedido.Id,
            Total = pedido.Total,
            FechaCreacion = pedido.FechaCreacion,
            Estado = pedido.Estado.ToString()
        };
    }
}
```

---

### 3.3 Consultas LINQ duplicadas

Detectar queries repetidas en repositorios, servicios o controladores.

Ejemplo de duplicación:

```csharp id="yfrxkl"
public List<Cliente> ObtenerActivos()
{
    return _context.Clientes
        .Where(cliente => cliente.Activo && !cliente.Eliminado)
        .ToList();
}
```

```csharp id="pu0iat"
var clientesActivos = _context.Clientes
    .Where(cliente => cliente.Activo && !cliente.Eliminado)
    .ToList();
```

Refactor recomendado:

```csharp id="94h89o"
public static class ClienteQueries
{
    public static IQueryable<Cliente> SoloActivos(this IQueryable<Cliente> query)
    {
        return query.Where(cliente => cliente.Activo && !cliente.Eliminado);
    }
}
```

---

## 4. Criterios de análisis

Al revisar código C#, aplicar la siguiente lógica:

1. Buscar elementos declarados pero no referenciados.
2. Revisar métodos privados, interfaces, DTOs, enums, extension methods y clases de servicio.
3. Verificar si las clases están registradas en Dependency Injection.
4. Revisar si existen usos indirectos por frameworks, reflexión, atributos o serialización.
5. Distinguir entre código realmente muerto y código usado implícitamente.
6. Proponer eliminación solo si no hay evidencia de uso real.
7. Proponer refactor cuando el problema sea duplicación.
8. Evitar eliminar código que pueda ser requerido por Entity Framework, Blazor, ASP.NET, serializers o reflection.

---

## 5. Casos donde no se debe marcar como muerto automáticamente

No marcar como código muerto sin revisar contexto cuando el elemento esté asociado a:

* Actions de controladores con atributos `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`.
* Parámetros `[Parameter]` en componentes Blazor.
* Propiedades usadas por Entity Framework.
* Propiedades usadas por serializers.
* Constructores requeridos por frameworks.
* Métodos invocados por reflection.
* Clases descubiertas dinámicamente con `Assembly.GetTypes()`.
* DTOs usados en binding o model binding.
* Eventos de UI.
* Migraciones.
* Configuración de dependency injection.

---

## 6. Búsquedas útiles en filesystem

Usar búsquedas simples para confirmar sospechas antes de afirmar que algo no se usa.

| Sospecha                           | Búsqueda sugerida                                 |               |                                 |                                      |
| ---------------------------------- | ------------------------------------------------- | ------------- | ------------------------------- | ------------------------------------ |
| Clase `XxxService` nunca usada     | `grep -r "XxxService" --include="*.cs"`           |               |                                 |                                      |
| Interfaz `IXxx` sin implementación | `grep -rn "class.*:.*IXxx" --include="*.cs"`      |               |                                 |                                      |
| Clase no registrada en DI          | `grep -rn "AddScoped\                             | AddTransient\ | AddSingleton" --include="*.cs"` |                                      |
| Uso por reflexión                  | `grep -rn "Activator.CreateInstance\              | GetType\      | typeof\                         | Assembly.GetTypes" --include="*.cs"` |
| Uso por atributos de framework     | `grep -rn "[HttpGet\]\                            | [HttpPost\]\  | [Parameter\]\                   | [BindProperty\]" --include="*.cs"`   |
| Extension method no usado          | `grep -rn "\\.NombreDelMetodo(" --include="*.cs"` |               |                                 |                                      |
| Enum value no usado                | `grep -rn "NombreDelValor" --include="*.cs"`      |               |                                 |                                      |
| DTO property no mapeada            | `grep -rn "NombreDeLaPropiedad" --include="*.cs"` |               |                                 |                                      |

---

## 7. Forma de reportar hallazgos

Cuando detectes un posible problema, reportarlo con esta estructura:

```text
Tipo de hallazgo:
Archivo / clase:
Elemento detectado:
Motivo:
Evidencia:
Riesgo:
Recomendación:
```

Ejemplo:

```text
Tipo de hallazgo: Código no utilizado
Archivo / clase: Application/Services/ProductService.cs
Elemento detectado: Método privado NormalizeCode
Motivo: No se encontró ninguna invocación dentro de la clase ni en el proyecto.
Evidencia: No aparece referenciado en búsquedas globales.
Riesgo: Aumenta ruido y mantenimiento innecesario.
Recomendación: Eliminarlo si no forma parte de una funcionalidad pendiente.
```

---

## 8. Refactor esperado

Cuando haya duplicación, preferir extraer la lógica común a:

* Guards.
* Mappers.
* Extension methods.
* Query extensions.
* Métodos privados descriptivos.
* Servicios de dominio.
* Validadores reutilizables.

Elegir la solución más simple posible según el caso.

No crear abstracciones innecesarias si el código duplicado es mínimo o si la extracción empeora la legibilidad.

---

## 9. Criterio final de aceptación

Antes de entregar un análisis o refactor, verificar que:

* No se marque como muerto código usado por frameworks.
* Las afirmaciones estén justificadas por evidencia.
* Se distinga entre certeza y sospecha.
* Se proponga eliminar solo cuando sea seguro.
* Se proponga refactorizar cuando haya duplicación real.
* Se mantenga el comportamiento existente.
* El código resultante sea más simple, legible y mantenible.
