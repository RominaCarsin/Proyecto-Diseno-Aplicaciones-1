# Referencia: TDD y Pruebas Unitarias

Basado en el material del curso DA1. Framework: MSTest o NUnit con C#.

---

## El ciclo TDD

```
🔴 RED   → Escribir un test que falla (test primero)
🟢 GREEN  → Escribir el mínimo código para que el test pase
🔵 REFACTOR → Mejorar el código sin romper tests
```

**Evidencia de TDD en el repositorio**: los commits deben mostrar tests antes que producción. Si solo hay código de producción con tests agregados después, no es TDD.

---

## Pruebas F.I.R.S.T.

| Letra | Significa       | Implicación práctica |
|-------|-----------------|----------------------|
| F     | Fast            | Tests deben correr en ms, no segundos |
| I     | Independent     | Tests no dependen entre sí, pueden correr en cualquier orden |
| R     | Repeatable      | Mismo resultado siempre, sin depender de estado externo |
| S     | Self-validating | El test dice Pass/Fail sin interpretación manual |
| T     | Timely          | Se escribe antes (o junto) al código de producción |

**Para BD**: usar EF Core In-Memory en tests, no la BD real de Docker. La BD de Docker es para producción.

---

## Estructura de un test correcto

### Patrón AAA (Arrange-Act-Assert)
```csharp
[TestMethod]
public void RecalcularRanking_EquipoDebilGana_IncrementaRankingCorrectamente() {
    // Arrange
    var equipoA = new Equipo("Uruguay", 1500);
    var equipoB = new Equipo("Bolivia", 1200);
    var partido = new Partido(equipoA, equipoB, Fase.Grupos);
    var service = new RankingService();

    // Act
    service.RecalcularRanking(partido, resultadoLocal: 0, resultadoVisitante: 1);

    // Assert
    Assert.AreEqual(1220, equipoB.Ranking); // valor esperado según fórmula del enunciado
    Assert.AreEqual(1480, equipoA.Ranking);
}
```

### Nombres de tests descriptivos
Formato recomendado: `Metodo_Escenario_ResultadoEsperado`

```csharp
// ✅ Nombre descriptivo — se entiende sin leer el cuerpo
RecalcularRanking_EmpateEntreIguales_NoCambiaRanking()
RecalcularRanking_ResultadoExcedeMaximo_AplicaClamping()
RegistrarResultado_PartidoYaFinalizado_LanzaExcepcion()
AgregarEquipo_RankingMenorA300_LanzaValidationException()

// ❌ Nombres que no dicen nada
Test1()
TestRanking()
PruebaServicio()
```

---

## Qué testear

### Casos que siempre deben estar
1. **Caso feliz (happy path)**: funcionamiento normal esperado
2. **Casos borde**: valores límite (0, máximo, mínimo, lista vacía)
3. **Casos de error**: input inválido, null, objeto no encontrado
4. **Reglas de negocio específicas**: cada regla del enunciado debe tener al menos un test

### Para el Obligatorio 2 — tests requeridos por funcionalidad

**Ranking**:
- Equipo fuerte gana → ranking sube poco al fuerte, baja poco al débil
- Equipo débil gana → ranking sube mucho al débil
- Empate → ajuste proporcional
- Clamping: resultado > 2500 → queda en 2500
- Clamping: resultado < 300 → queda en 300
- Fase eliminatoria: multiplicador 1.5 aplicado correctamente

**Motores de simulación**:
- Motor probabilístico: equipo con mayor ranking gana más frecuentemente
- Motor aleatorio: resultado no depende del ranking
- Ambos motores producen resultado válido (no null, scores no negativos)

**Notificaciones**:
- Al registrar resultado → notificación generada para periodistas
- Marcar como leída → no aparece en vista

---

## Señales de tests mal escritos

```csharp
// ❌ Test sin Assert (no verifica nada)
[TestMethod]
public void TestRegistrarResultado() {
    var service = new PartidoService(_repo);
    service.RegistrarResultado(1, 2, 1); // ¿qué debería haber pasado?
}

// ❌ Test que prueba múltiples cosas (si falla, ¿cuál falló?)
[TestMethod]
public void TestTodo() {
    // testea ranking, notificaciones y auditoría en un solo test
}

// ❌ Test que depende de BD real (no es F.I.R.S.T. - Repeatable)
[TestMethod]
public void TestConBDReal() {
    var repo = new SqlEquipoRepository("Server=localhost;..."); // BD real
}

// ❌ Tests que dependen entre sí
[TestMethod]
public void Test2_AsumeCosasDe_Test1() {
    // asume que Test1 ya corrió y dejó datos
}
```

---

## Uso de mocks/fakes

```csharp
// ✅ Aislar la unidad bajo prueba con mocks
[TestMethod]
public void RecalcularRanking_LlamaGuardarEquipo() {
    var mockRepo = new Mock<IEquipoRepository>();
    var service = new RankingService(mockRepo.Object);
    
    service.RecalcularRanking(partido, 1, 0);
    
    mockRepo.Verify(r => r.Guardar(It.IsAny<Equipo>()), Times.Exactly(2));
}
```

**Cuándo usar qué**:
- **Mock**: verificar que se llamaron ciertos métodos
- **Stub/Fake**: proveer datos de prueba sin lógica
- **In-Memory EF**: cuando se necesita testear lógica de repositorio con queries reales pero sin BD

---

## Cobertura

- No hay un número mágico, pero el código del **dominio** debe tener alta cobertura
- Los **servicios** deben tener cobertura de los flujos principales y de error
- **Repositorios**: testear con EF In-Memory para las queries más complejas
- **UI**: no se requiere cobertura de formularios/vistas
