# Patrones de Código C# — Referencia de Detección

## 1. Código Muerto — Ejemplos

### 1.1 Método privado nunca llamado
```csharp
// ❌ DEAD CODE — CalcularImpuesto() no es invocado desde ningún lado
public class PedidoService
{
    public decimal ObtenerTotal(Pedido pedido) => pedido.Subtotal;

    private decimal CalcularImpuesto(decimal base_) // <- muerto
    {
        return base_ * 0.21m;
    }
}
```

### 1.2 Rama inalcanzable
```csharp
// ❌ DEAD CODE — el return hace que todo lo siguiente sea inalcanzable
public string ObtenerEstado(int codigo)
{
    if (codigo == 1) return "Activo";
    return "Inactivo";

    // Nunca se ejecuta:
    if (codigo == 2) return "Pendiente"; // <- muerto
}
```

### 1.3 Variable local asignada pero nunca leída
```csharp
public void ProcesarOrden(Orden orden)
{
    var timestamp = DateTime.UtcNow; // <- nunca se usa
    var resultado = repositorio.Guardar(orden);
    logger.Log(resultado.Id.ToString());
}
```

### 1.4 Catch vacío
```csharp
try
{
    servicio.Ejecutar();
}
catch (Exception ex) // <- ex nunca se usa, catch está vacío
{
    // TODO
}
```

### 1.5 Enum con valores no usados
```csharp
public enum EstadoPedido
{
    Pendiente,
    Procesando,
    Completado,
    Cancelado,
    EnDisputa // <- nunca aparece en switch, comparaciones ni asignaciones
}
```

---

## 2. Código No Utilizado — Ejemplos

### 2.1 Interfaz sin implementación
```csharp
// ❌ UNUSED — IReporteExportador declarada pero sin ninguna clase que la implemente
public interface IReporteExportador
{
    byte[] Exportar(Reporte reporte);
}
```

### 2.2 Implementación no registrada en DI
```csharp
// ❌ UNUSED — CacheDeProductos implementa ICacheDeProductos
// pero no aparece en ningún AddScoped/AddTransient/AddSingleton
public class CacheDeProductos : ICacheDeProductos
{
    // ...
}
```

### 2.3 Using directive no usado
```csharp
using System.Xml.Serialization;  // <- nunca se usa en este archivo
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Dominio.Entidades { ... }
```

### 2.4 Parámetro no utilizado en método
```csharp
// ❌ UNUSED PARAM — 'opciones' nunca se usa en el cuerpo
public IEnumerable<Producto> Buscar(string termino, BusquedaOpciones opciones)
{
    return context.Productos
        .Where(p => p.Nombre.Contains(termino))
        .ToList();
}
```

### 2.5 DTO con propiedad no mapeada
```csharp
public class ClienteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string CodigoInterno { get; set; } // <- nunca se asigna en ningún mapper
    public string Email { get; set; }
}
```

### 2.6 Extension method no invocado
```csharp
// ❌ UNUSED — ToSlug() no aparece invocado en ningún .cs del proyecto
public static class StringExtensions
{
    public static string ToSlug(this string input)
    {
        return input.ToLower().Replace(" ", "-");
    }
}
```

---

## 3. Código Duplicado — Ejemplos

### 3.1 Guard clauses duplicadas entre servicios
```csharp
// En PedidoService.cs:
public void Procesar(Pedido pedido)
{
    if (pedido == null) throw new ArgumentNullException(nameof(pedido));
    if (pedido.Items == null || !pedido.Items.Any())
        throw new InvalidOperationException("El pedido no tiene ítems.");
    // ...
}

// En FacturaService.cs: (DUPLICADO)
public void Generar(Pedido pedido)
{
    if (pedido == null) throw new ArgumentNullException(nameof(pedido));
    if (pedido.Items == null || !pedido.Items.Any())
        throw new InvalidOperationException("El pedido no tiene ítems.");
    // ...
}
```

**Refactor propuesto:**
```csharp
// En Dominio/Guards/PedidoGuards.cs:
public static class PedidoGuards
{
    public static void EnsureValido(Pedido pedido)
    {
        if (pedido == null) throw new ArgumentNullException(nameof(pedido));
        if (pedido.Items == null || !pedido.Items.Any())
            throw new InvalidOperationException("El pedido no tiene ítems.");
    }
}
```

### 3.2 Mapping manual duplicado
```csharp
// En PedidoController.cs:
var dto = new PedidoDto
{
    Id = pedido.Id,
    Total = pedido.Total,
    FechaCreacion = pedido.FechaCreacion,
    Estado = pedido.Estado.ToString()
};

// En PedidoService.cs: (DUPLICADO)
var dto = new PedidoDto
{
    Id = pedido.Id,
    Total = pedido.Total,
    FechaCreacion = pedido.FechaCreacion,
    Estado = pedido.Estado.ToString()
};
```

**Refactor propuesto:**
```csharp
// En Application/Mappers/PedidoMapper.cs:
public static class PedidoMapper
{
    public static PedidoDto ToDto(this Pedido pedido) => new()
    {
        Id = pedido.Id,
        Total = pedido.Total,
        FechaCreacion = pedido.FechaCreacion,
        Estado = pedido.Estado.ToString()
    };
}
```

### 3.3 Consultas LINQ duplicadas en repositorios
```csharp
// En ClienteRepository.cs:
public List<Cliente> ObtenerActivos() =>
    context.Clientes.Where(c => c.Activo && !c.Eliminado).ToList();

// En ReporteRepository.cs: (DUPLICADO)
var clientesActivos = context.Clientes
    .Where(c => c.Activo && !c.Eliminado)
    .ToList();
```

**Refactor propuesto:**
```csharp
// En Infraestructura/Queries/ClienteQueries.cs:
public static class ClienteQueries
{
    public static IQueryable<Cliente> SoloActivos(this IQueryable<Cliente> query) =>
        query.Where(c => c.Activo && !c.Eliminado);
}
```

---

## 4. Tabla de búsqueda rápida

| Lo que ves | Cómo buscarlo en el filesystem |
|---|---|
| Clase `XxxService` nunca usada | `grep -r "XxxService" --include="*.cs"` |
| Interfaz `IXxx` sin implementación | `grep -rn "class.*:.*IXxx" --include="*.cs"` |
| Registro en DI de una clase | `grep -rn "AddScoped\|AddTransient\|AddSingleton" --include="*.cs"` |
| Uso de reflexión que podría activar una clase | `grep -rn "Activator.CreateInstance\|GetType\|typeof\|Assembly.GetTypes" --include="*.cs"` |
| Atributos que indican uso por framework | `grep -rn "\[HttpGet\]\|\[HttpPost\]\|\[Parameter\]\|\[BindProperty\]" --include="*.cs"` |