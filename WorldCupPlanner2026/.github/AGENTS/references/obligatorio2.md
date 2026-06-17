# Referencia: Requerimientos Específicos Obligatorio 2

Checklist detallado de los requerimientos del Obligatorio 2 de DA1 (ORT, entrega 17/06/2026).

---

## Requerimientos de Diseño (sección 1.2 — son los más importantes para la nota)

### GRASP Controlador
- [ ] Existe una clase que actúa como controlador (Fachada o por Caso de Uso)
- [ ] El controlador **delega** en lugar de implementar directamente
- [ ] La UI no contiene lógica de negocio — solo llama al controlador/servicio

### GRASP Experto en Cumplimiento
- [ ] Las responsabilidades están asignadas a las clases que tienen la información
- [ ] No hay "envidia de características" (Feature Envy): una clase accediendo excesivamente a datos de otra

### Bajo Acoplamiento entre capas
- [ ] La capa de servicios depende de **interfaces** de repositorios, no de implementaciones
- [ ] El dominio no depende de la infraestructura (EF, SQL, UI)
- [ ] Los servicios no se acoplan innecesariamente entre sí

### Alta Cohesión
- [ ] Cada clase de servicio tiene responsabilidades bien definidas
- [ ] No hay servicios "God object" que hacen todo

### Ley de Demeter
- [ ] En el dominio: no hay cadenas de llamadas `a.GetB().GetC().Hacer()`
- [ ] En servicios: se trabaja con los objetos directos, no con sus internos
- [ ] Los objetos exponen comportamiento, no solo datos (Tell, don't ask)

### SOLID — SRP
- [ ] Al menos una clase de dominio tiene una sola razón para cambiar
- [ ] Documentado en la sección "Explicación de mecanismos generales"

### SOLID — OCP
- [ ] Al menos una clase de dominio está abierta a extensión y cerrada a modificación
- [ ] Documentado en la sección "Explicación de mecanismos generales"
- [ ] Los motores de simulación son el ejemplo natural (interfaz `IMotorSimulacion`)

---

## Requerimientos Funcionales críticos para el diseño

### Sistema de Ranking Dinámico (2.3)
El cálculo ELO debe implementarse correctamente:

```
RankingNuevo = RankingActual + [K × (Resultado − Probabilidad)] × MultiplicadorFase
```

Donde:
- K = 30 (constante, idealmente como `const int FACTOR_K = 30`)
- Probabilidad = 1 / (1 + 10^((RankingOponente - RankingEquipo)/400))
  - ⚠️ **ATENCIÓN**: el enunciado usa base 10 y divisor 400 implícito (ver ejemplo numérico)
  - Verificar: con diferencia de 300 puntos → probabilidad ≈ 0.33 para el equipo más débil
- Resultado: 1 (victoria), 0.5 (empate), 0 (derrota)
- MultiplicadorFase: 1.0 grupos, 1.5 eliminatorias

**Clamping**: resultado nunca < 300 ni > 2500

**Redondeo**: `MidpointRounding.AwayFromZero` (determinístico, 0.5 → entero superior)

```csharp
// ✅ Implementación correcta con constantes y redondeo determinístico
private const int FACTOR_K = 30;
private const int RANKING_MAXIMO = 2500;
private const int RANKING_MINIMO = 300;

private int CalcularNuevoRanking(int rankingActual, double resultado, 
                                  double probabilidad, double multiplicador) {
    double cambio = FACTOR_K * (resultado - probabilidad) * multiplicador;
    int nuevo = (int)Math.Round(rankingActual + cambio, MidpointRounding.AwayFromZero);
    return Math.Clamp(nuevo, RANKING_MINIMO, RANKING_MAXIMO);
}
```

### Motores de Simulación Intercambiables (2.7)
- [ ] Existe una interfaz `IMotorSimulacion` (o nombre equivalente)
- [ ] Hay al menos 2 implementaciones: motor probabilístico y motor aleatorio
- [ ] El fixture guarda una referencia al tipo de motor usado
- [ ] Al crear el fixture se selecciona el motor
- [ ] En pantalla se muestra el nombre del simulador

**Patrón esperado**: Strategy Pattern (relacionado con Polimorfismo GRASP y OCP SOLID)

### Sistema de Notificaciones (2.4)
- [ ] Se generan notificaciones automáticamente al registrar resultado
- [ ] Solo se generan para usuarios con rol Periodista
- [ ] Las notificaciones se pueden marcar como leídas
- [ ] Las notificaciones leídas no aparecen en la vista principal del periodista

### Auditoría extendida (2.6)
- [ ] Timestamp en formato ISO-8601 (`DateTime.UtcNow.ToString("o")` o similar)
- [ ] Registra: cambios de ranking (valor anterior y nuevo)
- [ ] Registra: simulaciones (incluida la semilla)
- [ ] Registra: exportaciones (quién, qué, cuándo)
- [ ] Registra: lectura de notificaciones por periodistas
- [ ] Los logs no están asociados a un torneo específico (son del sistema)

### Exportación CSV/XLSX (2.5)
- [ ] Funcionalidad en servicio, no en UI directamente
- [ ] Disponible para Administrador y Periodista
- [ ] Exporta: Fixture Final y Log de Auditoría por rango de fechas

### Rol Periodista (2.2)
- [ ] Puede: ver fixture, estadísticas, rankings
- [ ] **No puede**: editar partidos, gestionar usuarios
- [ ] Los permisos se verifican en el servicio, no solo en la UI

---

## Persistencia (1.1)

- [ ] Azure SQL Edge en Docker
- [ ] Entity Framework Core Code First
- [ ] Los datos se persisten durante la operación (no al cerrar)
- [ ] Incluye migration generada automáticamente por EF
- [ ] Script SQL vacío incluido en la entrega
- [ ] Script SQL con datos de prueba incluido en la entrega

**Datos de prueba mínimos requeridos**:
- 2 administradores
- 2 editores
- 2 periodistas
- 5 estadios
- 1 torneo con fase de grupos simulada
- Al menos 4 equipos con bandera (base64)
- Entradas de auditoría correspondientes

---

## Diagrama de capas esperado

```
┌─────────────────────┐
│   UI / Presentación  │  ← Solo llama a servicios
├─────────────────────┤
│   Servicios          │  ← Lógica de aplicación, orquestación
├─────────────────────┤
│   Dominio            │  ← Lógica de negocio pura
├─────────────────────┤
│   Repositorios       │  ← Acceso a datos (implementa interfaces del dominio)
├─────────────────────┤
│   Infraestructura    │  ← EF Core, Azure SQL, etc.
└─────────────────────┘
```

Las dependencias deben ir **hacia abajo** (UI → Servicios → Dominio), con interfaces para invertir dependencias en los bordes (Dominio define `IRepository`, Infraestructura lo implementa).

---

## Puntos de la rúbrica que más impactan el diseño

| Ítem | Puntos | Qué miran |
|------|--------|-----------|
| Diseño y documentación | 8 pts | Diagramas correctos, explicación de principios, notación UML correcta |
| Calidad del código | 7 pts | Clean Code, TDD evidenciado, concordancia código-diseño, acceso BD |
| Funcionalidad | 7 pts | Todo funciona, sin errores bloqueantes |
| IA Generativa | 3 pts | Skills creados, instrucciones del agente documentadas |

**Total**: 25 puntos, mínimo 7 para aprobar.
