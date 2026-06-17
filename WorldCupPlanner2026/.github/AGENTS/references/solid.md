# Referencia: Principios SOLID

Principios de diseño a nivel de clases. Objetivo: controlar las dependencias para que el código sea mantenible.

---

## S — Single Responsibility Principle (SRP)

**Enunciado**: Una clase debe tener un solo motivo para cambiar.

**Clave**: los motivos de cambio son del negocio (actores, stakeholders), no técnicos.

**Señales de violación**:
- Clase que tiene lógica de negocio + persistencia + logging + notificaciones
- Clase que cambia por razones de diferentes áreas (ej: cambio de DB y cambio de regla de negocio)
- Nombres vagos: `Manager`, `Helper`, `Utils`, `Processor` (suelen ser de baja cohesión)

**Ejemplo de violación**:
```csharp
// ❌ Viola SRP: cambia por reglas de ranking Y por cambio de motor de notificación
public class PartidoService {
    public void RegistrarResultado(int id, int golesL, int golesV) {
        // ... actualiza resultado
        // ... recalcula ranking
        // ... envía email a periodistas
        // ... escribe log de auditoría
        // ... guarda en BD
    }
}
```

**Solución**: separar en `RankingService`, `NotificacionService`, `AuditoriaService`.

---

## O — Open/Closed Principle (OCP)

**Enunciado**: Las clases deben estar abiertas a la extensión y cerradas a la modificación.

**En práctica**: agregar nuevo comportamiento creando nuevas clases, sin tocar el código existente que funciona.

**Señales de violación**:
```csharp
// ❌ Para agregar motor "PorEdad" hay que modificar esta clase
public class SimuladorFixture {
    public Resultado Simular(string tipoMotor, Partido p) {
        if (tipoMotor == "Probabilistico") { /* ... */ }
        else if (tipoMotor == "Aleatorio") { /* ... */ }
        // Agregar nuevo motor = modificar este código existente
    }
}
```

**Solución**:
```csharp
// ✅ OCP: agregar motor = crear nueva clase, no modificar existentes
public interface IMotorSimulacion {
    Resultado Simular(Partido partido, int semilla);
}
public class MotorProbabilistico : IMotorSimulacion { ... }
public class MotorAleatorio : IMotorSimulacion { ... }
// Motor nuevo → nueva clase, sin tocar las anteriores
```

**Advertencia**: No abstraer todo de entrada. Aplicar OCP cuando:
- Se sabe que algo CLARAMENTE va a variar
- Se encuentran if/switch sobre tipos de clases

---

## L — Liskov Substitution Principle (LSP)

**Enunciado**: Las instancias de una subclase deben poder usarse en lugar de la clase base sin que el programa se rompa.

**Señales de violación**:
- Subclase que lanza `NotImplementedException` para métodos heredados
- Subclase que ignora precondiciones de la clase base
- El clásico problema Rectángulo-Cuadrado:
```csharp
// ❌ Cuadrado hereda Rectangulo pero su comportamiento rompe contratos
public class Cuadrado : Rectangulo {
    public override int Ancho { set { base.Ancho = value; base.Alto = value; } }
    // Ahora SetAncho también cambia el Alto → viola contrato de Rectangulo
}
```

**Solución**: reestructurar la jerarquía cuando una subclase no puede honrar los contratos de la base.

---

## I — Interface Segregation Principle (ISP)

**Enunciado**: Los módulos clientes solo deben depender de los métodos que realmente invocan.

**Señales de violación**:
```csharp
// ❌ IEmpleadoRepository tiene métodos que no todos los clientes necesitan
public interface IEmpleadoRepository {
    Employee FindEmployee(int id);
    void AddTimeCard(TimeCard tc);
    void AddSalesReceipt(SalesReceipt sr);
}
// TimekeepingService solo necesita FindEmployee y AddTimeCard
// SalesService solo necesita FindEmployee y AddSalesReceipt
// Ambos dependen de métodos que no usan → compilación innecesaria ante cambios
```

**Solución**: segregar en interfaces específicas por cliente:
```csharp
public interface ITimekeepingRepo { Employee FindEmployee(int id); void AddTimeCard(TimeCard tc); }
public interface ISalesRepo { Employee FindEmployee(int id); void AddSalesReceipt(SalesReceipt sr); }
```

---

## D — Dependency Inversion Principle (DIP)

**Enunciado**: Los módulos de alto nivel no deben depender de módulos de bajo nivel. Ambos deben depender de abstracciones. Las abstracciones no deben depender de detalles; los detalles deben depender de abstracciones.

**Señales de violación**:
```csharp
// ❌ Servicio (alto nivel) depende de implementación concreta (bajo nivel)
public class RankingService {
    private SqlEquipoRepository _repo = new SqlEquipoRepository(); // DIP violado
}
```

**Solución**:
```csharp
// ✅ DIP correcto: depende de la abstracción, recibe implementación por inyección
public class RankingService {
    private readonly IEquipoRepository _repo;
    public RankingService(IEquipoRepository repo) { _repo = repo; }
}
```

**Relación con OCP**: DIP es el mecanismo que permite implementar OCP. Sin DIP, es difícil aplicar OCP.

**Relación con LSP**: Para que DIP funcione, las implementaciones deben cumplir LSP (ser intercambiables).

---

## Relaciones entre SOLID

```
OCP ←→ DIP: DIP es el mecanismo para lograr OCP
LSP → DIP: Sin LSP, el polimorfismo que habilita DIP no funciona
SRP ↔ ISP: ISP es SRP aplicado a interfaces
```

**Síntoma general de violaciones SOLID**:
- Rigidez: cambiar algo fuerza cambios en cadena
- Fragilidad: cambiar algo rompe cosas aparentemente no relacionadas
- Inmovilidad: difícil reusar componentes en otros contextos
