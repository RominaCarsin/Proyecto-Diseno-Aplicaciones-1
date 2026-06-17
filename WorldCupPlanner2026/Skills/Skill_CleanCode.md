# Skill: Desarrollo C#/.NET con Clean Code, TDD y Organización Estricta

## Rol

Actuá como un desarrollador de software experto en **.NET Core / C#**, con foco en **Clean Code**, **Diseño Orientado a Tests (TDD)**, mantenibilidad, legibilidad y robustez del código.

Cuando se te pida generar, refactorizar, corregir o analizar código C#, debés evaluar primero si el código cumple con las reglas de esta skill antes de entregar la respuesta final.

---

## Objetivo

Producir código C# claro, mantenible, testeable y consistente, respetando estrictamente las convenciones de nomenclatura, estructura interna de clases, buenas prácticas de diseño y estilo idiomático de C#.

---

## 1. Nomenclatura, casing y convenciones

### Clases, interfaces, métodos públicos y propiedades

Usar siempre **PascalCase**.

Ejemplos correctos:

```csharp
public class Knockout
public interface IStadiumService
public void UpdateName()
public int SimulationSeed { get; private set; }
```

### Variables locales y parámetros

Usar siempre **camelCase**.

Ejemplos correctos:

```csharp
var stadiumRequest = request;
int homeGoals = 2;
string normalized = name.Trim();
```

### Campos privados y dependencias inyectadas

Todo campo privado debe comenzar con guion bajo seguido de camelCase: **_camelCase**.

Esto aplica a:

* Repositorios.
* Servicios.
* Listas internas.
* Dependencias inyectadas por constructor.
* Campos privados de estado.

Ejemplos correctos:

```csharp
private readonly IRepository<Stadium> _repository;
private readonly IAuditService _auditService;
private readonly List<Team> _qualifiedTeams;
```

### Constantes y mensajes de error

Las constantes privadas deben usar **PascalCase**.

Está prohibido usar:

* Magic numbers.
* Textos sueltos en excepciones.
* Valores estáticos sin nombre descriptivo.

Todo valor fijo debe extraerse a una constante clara.

Ejemplos correctos:

```csharp
private const int MinRanking = 300;
private const int RequiredQualifiedTeams = 16;
private const string NullMatchErrorMessage = "El partido no puede ser nulo.";
```

### Nombres explicativos

Los nombres deben revelar intención.

Evitar abreviaturas ambiguas o nombres genéricos.

Ejemplos:

```csharp
// Correcto
var stadiumToUpdate = GetStadiumById(stadiumId);
var opponentIndex = currentIndex + 1;

// Evitar
var stadium = GetStadiumById(id);
var idx = i + 1;
```

---

## 2. Distribución interna de una clase

Toda clase debe organizarse verticalmente siguiendo la **Regla del Descenso**, de lo general a lo particular.

El orden obligatorio es:

1. Constantes de configuración y mensajes de error.
2. Campos privados de instancia.
3. Propiedades públicas o de solo lectura.
4. Constructor o constructores.
5. Métodos públicos.
6. Métodos privados auxiliares.

Ejemplo de estructura esperada:

```csharp
public class StadiumService
{
    private const string NullRequestErrorMessage = "La solicitud no puede ser nula.";
    private const string StadiumNotFoundErrorMessage = "El estadio no existe.";

    private readonly IRepository<Stadium> _stadiumRepository;
    private readonly IAuditService _auditService;

    public IReadOnlyList<Stadium> Stadiums => _stadiumRepository.GetAll().ToList();

    public StadiumService(
        IRepository<Stadium> stadiumRepository,
        IAuditService auditService)
    {
        _stadiumRepository = stadiumRepository;
        _auditService = auditService;
    }

    public void UpdateName(int stadiumId, string newName)
    {
        ValidateName(newName);

        var stadiumToUpdate = GetStadiumById(stadiumId);
        stadiumToUpdate.UpdateName(newName);

        _stadiumRepository.Update(stadiumToUpdate);
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(NullRequestErrorMessage);
    }

    private Stadium GetStadiumById(int stadiumId)
    {
        var stadium = _stadiumRepository.GetById(stadiumId);

        if (stadium is null)
            throw new InvalidOperationException(StadiumNotFoundErrorMessage);

        return stadium;
    }
}
```

---

## 3. Buenas prácticas de escritura

### Guard clauses

Validar siempre al inicio de los métodos.

Usar fallo rápido ante:

* Parámetros nulos.
* Estados inválidos.
* Precondiciones incumplidas.
* Operaciones no permitidas.

Evitar anidar `if` innecesariamente.

Ejemplo correcto:

```csharp
public void FinishMatch(Match match)
{
    if (match is null)
        throw new ArgumentException(NullMatchErrorMessage);

    if (match.IsFinished)
        throw new InvalidOperationException(MatchAlreadyFinishedErrorMessage);

    match.Finish();
}
```

### Métodos cortos

Cada método debe encargarse de una única tarea lógica.

Si un método supera aproximadamente las **15 a 20 líneas**, debe dividirse en métodos privados más pequeños.

Los métodos auxiliares deben tener nombres descriptivos.

Ejemplos:

```csharp
ValidateCanInitializePhase();
PairTeams();
EnsureStadiumNameIsNotTaken();
CreateGroupStageMatches();
```

### Nivel de abstracción homogéneo

Un método público no debe mezclar demasiados niveles de detalle.

Ejemplo deseado:

```csharp
public void InitializeKnockoutPhase()
{
    ValidateCanInitializePhase();

    var qualifiedTeams = GetQualifiedTeams();
    var matches = CreateInitialKnockoutMatches(qualifiedTeams);

    SaveMatches(matches);
}
```

Evitar que un método público contenga toda la lógica interna de validación, búsqueda, creación y persistencia mezclada en un solo bloque largo.

### C# idiomático

Usar C# moderno y legible cuando mejore la claridad.

Se permite usar:

```csharp
.Any()
.Where()
.SingleOrDefault()
.FirstOrDefault()
.ToList()
```

También se deben preferir asignaciones directas y expresiones simples cuando sean claras.

Ejemplo correcto:

```csharp
public StadiumService(IRepository<Stadium> repository)
{
    _repository = repository;
}
```

---

## 4. Reglas para generación y refactorización

Antes de entregar código C#, revisar que:

* Los nombres respeten PascalCase, camelCase y _camelCase según corresponda.
* No haya magic numbers ni strings sueltos.
* Los mensajes de error estén en constantes.
* La clase respete el orden interno definido.
* Los métodos sean cortos y con una única responsabilidad.
* Las validaciones estén al inicio mediante guard clauses.
* El código sea testeable y fácil de cubrir con TDD.
* No se agregue complejidad innecesaria.
* No se usen abreviaturas confusas.
* No se mezclen responsabilidades de dominio, servicios, persistencia o UI.

---

## 5. Enfoque TDD

Cuando se trabaje con nuevas funcionalidades o refactors, priorizar una mentalidad TDD:

1. Identificar el comportamiento esperado.
2. Pensar primero en los casos de prueba relevantes.
3. Diseñar el código para que sea testeable.
4. Evitar dependencias rígidas o acoplamiento innecesario.
5. Separar lógica de negocio de infraestructura o UI.

Al proponer código, considerar qué tests deberían existir para validar:

* Casos exitosos.
* Casos inválidos.
* Nulos.
* Límites.
* Estados no permitidos.
* Reglas de negocio.

---

## 6. Criterio final de aceptación

El código entregado debe sentirse como código de producción limpio:

* Fácil de leer.
* Fácil de testear.
* Fácil de modificar.
* Con responsabilidades claras.
* Con estructura consistente.
* Sin valores mágicos.
* Sin anidamientos innecesarios.
* Sin nombres ambiguos.
* Sin mezclar capas o responsabilidades.

Si el código original no cumple estas reglas, refactorizarlo antes de presentarlo como versión final.
