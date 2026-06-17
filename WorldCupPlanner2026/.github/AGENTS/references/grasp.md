# Referencia: Patrones GRASP

Principios Generales de Software para la Asignación de Responsabilidades (Craig Larman).
Usados en DA1 para guiar el diseño orientado a objetos.

---

## 1. Creador

**Problema**: ¿Quién debe ser responsable de crear una nueva instancia de una clase A?

**Solución**: Asignar a la clase B la responsabilidad de crear A si:
- B agrega, contiene o registra objetos A
- B tiene los datos de inicialización necesarios para A

**Ejemplo correcto**:
```csharp
// Factura contiene LineaFactura → Factura crea LineaFactura
public class Factura {
    public LineaFactura AgregarLinea(Producto p, int cantidad) {
        return new LineaFactura(p, cantidad); // ✅ Factura es creadora
    }
}
```

**Señal de problema**: Una clase crea objetos de otra sin tener relación de contenido ni los datos necesarios.

---

## 2. Experto en Información

**Problema**: ¿A qué clase asignar una responsabilidad?

**Solución**: Asignar la responsabilidad a la clase que tiene la información necesaria para cumplirla.

**Ejemplo correcto**:
```csharp
// LineaFactura conoce cantidad y precio → ella calcula el subtotal
public class LineaFactura {
    public decimal CalcularSubtotal() => Cantidad * Producto.PrecioUnitario; // ✅
}
// Factura conoce todas sus líneas → ella calcula el total
public class Factura {
    public decimal CalcularTotal() => Lineas.Sum(l => l.CalcularSubtotal()); // ✅
}
```

**Señal de problema**: Una clase A extrae datos de B para hacer un cálculo que B podría hacer sola.

---

## 3. Bajo Acoplamiento

**Problema**: ¿Cómo reducir el impacto del cambio?

**Solución**: Asignar responsabilidades de modo que el acoplamiento sea bajo. Una clase con bajo acoplamiento depende de pocas clases, especialmente de pocas clases inestables.

**Señales de problema**:
- Una clase importa/usa docenas de otras clases
- Un cambio en una clase obliga cambios en muchas otras
- Clases de dominio que dependen de clases de infraestructura (BD, UI)

**Métricas orientativas**: una clase de dominio idealmente depende de < 5 otras clases.

---

## 4. Alta Cohesión

**Problema**: ¿Cómo mantener la complejidad dentro de límites manejables?

**Solución**: Asignar responsabilidades de modo que la cohesión sea alta. Una clase con alta cohesión tiene responsabilidades fuertemente relacionadas.

**Señales de problema**:
- Clase que hace cálculos de negocio, persistencia, logging y formatting a la vez
- Clase con métodos que usan subconjuntos disjuntos de los campos
- "God class" que lo hace todo

**Relación con SRP**: una clase de baja cohesión generalmente viola SRP.

---

## 5. Controlador

**Problema**: ¿Quién debe encargarse de atender los eventos del sistema (acciones del usuario)?

**Solución**: Asignar la responsabilidad a una clase que represente:
- **Controlador de Fachada**: representa el sistema global (ej: `TorneoController`, `SistemaFutbol`)
- **Controlador de Caso de Uso**: maneja los eventos de un caso de uso específico (ej: `GestionPartidosService`)

**Regla clave**: El controlador **delega** a otros objetos; no hace el trabajo él mismo.

**Señal de problema**:
```csharp
// ❌ Lógica de negocio en la UI
private void btnGuardar_Click(object sender, EventArgs e) {
    var ranking = equipo.Ranking + 30 * (1 - probabilidad); // lógica de negocio aquí
    db.SaveChanges();
}

// ✅ Correcto: delegar al servicio
private void btnGuardar_Click(object sender, EventArgs e) {
    _partidoService.RegistrarResultado(partidoId, golesLocal, golesVisitante);
}
```

---

## 6. Polimorfismo

**Problema**: ¿Cómo manejar alternativas basadas en el tipo de un objeto? ¿Cómo crear componentes intercambiables?

**Solución**: Usar operaciones polimórficas (interfaces o herencia) en lugar de if/switch sobre tipos.

**Señal de problema**:
```csharp
// ❌ Switch sobre tipo → viola OCP también
if (motor == "Probabilístico") { SimularProbabilistico(); }
else if (motor == "Aleatorio") { SimularAleatorio(); }

// ✅ Polimorfismo
IMotorSimulacion motor = _motores[tipo];
motor.Simular(fixture);
```

---

## 7. Fabricación Pura

**Problema**: ¿A quién asignar una responsabilidad cuando aplicar otros GRASP baja la cohesión o sube el acoplamiento?

**Solución**: Crear una clase artificial (que no representa un concepto del dominio) altamente cohesiva.

**Ejemplos clásicos**:
- Repositorios (no existen en el dominio real, son inventados para persistencia)
- Servicios de aplicación
- Adaptadores, mappers, factories

**Señal de problema**: Poner responsabilidad de persistencia directamente en entidades de dominio.

---

## 8. Indirección

**Problema**: ¿Cómo evitar acoplamiento directo entre dos elementos?

**Solución**: Introducir un objeto intermediario que desacople los componentes.

**Ejemplo**: Un servicio no accede directo al repositorio SQL; accede a una interfaz `IEquipoRepository`. El intermediario permite cambiar la implementación sin afectar al servicio.

**Nota**: Un intermediario de indirección es frecuentemente una Fabricación Pura.

---

## 9. Variaciones Protegidas

**Problema**: ¿Cómo diseñar para minimizar el impacto de las variaciones de implementación?

**Solución**: Identificar puntos de variación e instabilidad, y crear interfaces estables alrededor de ellos.

**En el Obligatorio 2**: Los motores de simulación son un punto de variación → usar interfaz `IMotorSimulacion`.

**Técnicas**:
- Programar contra interfaces, no implementaciones
- Evitar violar Ley de Demeter
- Aplicar polimorfismo
- Usar indirección/intermediarios

---

## Ley de Demeter

También llamada "No hables con extraños".

**Regla**: Un método `M` de objeto `O` solo debe llamar métodos de:
1. El propio `O`
2. Parámetros de `M`
3. Objetos creados dentro de `M`
4. Campos/propiedades directas de `O`

**Señal de problema**:
```csharp
// ❌ Viola Demeter — conoce estructura interna de Partido
string ciudad = partido.GetEstadio().GetCiudad().GetNombre();

// ✅ Cumple Demeter — Partido expone lo que necesitan
string ciudad = partido.GetCiudadEstadio();
```

**Impacto**: Violar Demeter aumenta el acoplamiento estructural y hace el código frágil ante cambios internos.
