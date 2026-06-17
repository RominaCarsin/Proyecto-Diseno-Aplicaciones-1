# Referencia: Clean Code

Basado en el material del curso DA1 (capítulos 1-10 y 12 de Clean Code, R.C. Martin).

---

## Nombres

### ✅ Deben ser
- **Reveladores de intención**: `diasHastaVencimiento` en vez de `d`
- **Pronunciables**: `fechaCreacion` no `fchCrc`
- **Sin encodings**: no `strNombre`, no `iCantidad` (notación húngara)
- **Distinguibles**: no `data1`, `data2` — qué representan cada uno

### ❌ Señales de problema
```csharp
int d;                          // nombre sin significado
List<int[]> theList;            // qué es theList?
bool flag;                      // flag de qué?
void ProcessData() { ... }      // procesa qué datos?
class Manager { ... }           // maneja qué exactamente?
```

### ✅ Correcto
```csharp
int diasRestantes;
List<Celda> celdas;
bool esActivo;
void RecalcularRanking(Partido partido) { ... }
class RankingService { ... }
```

---

## Funciones / Métodos

### Tamaño y nivel de abstracción
- **Hacer una sola cosa**: si el nombre del método con "y" describe lo que hace, probablemente hace dos cosas
- **Un nivel de abstracción**: todas las instrucciones de un método deben estar al mismo nivel de abstracción
- **Largo orientativo**: idealmente < 20 líneas; métodos de > 40 líneas son señal de alarma

```csharp
// ❌ Hace demasiado y mezcla niveles de abstracción
public void FinalizarPartido(int id) {
    var partido = _db.Partidos.Include(p => p.EquipoLocal)
                              .Include(p => p.EquipoVisitante)
                              .FirstOrDefault(p => p.Id == id);
    if (partido == null) throw new NotFoundException();
    partido.Estado = EstadoPartido.Finalizado;
    // fórmula ELO completa aquí (15 líneas)
    // envío de notificaciones aquí (10 líneas)
    // escritura de auditoría aquí (8 líneas)
    _db.SaveChanges();
}

// ✅ Un nivel de abstracción, delega
public void FinalizarPartido(int id) {
    var partido = ObtenerPartidoOFallar(id);
    partido.Finalizar();
    _rankingService.Recalcular(partido);
    _notificacionService.NotificarResultado(partido);
    _auditoriaService.RegistrarFinalizacion(partido);
    _repo.Guardar(partido);
}
```

### Argumentos
- Idealmente 0-2 argumentos
- 3 argumentos: justificar
- 4+: señal de alarma — considerar objeto parámetro o clase
- **No usar booleanos como argumentos** (flag arguments):
```csharp
// ❌ ¿Qué significa true acá?
SimularPartido(partido, true);

// ✅ Dos métodos o un tipo explícito
SimularPartidoConRanking(partido);
SimularPartidoAleatorio(partido);
```

---

## Comentarios

### Comentarios que AGREGAN valor (✅ usar)
- Explicar POR QUÉ se tomó una decisión no obvia
- Advertencias sobre consecuencias
- TODOs marcados explícitamente

```csharp
// Usamos Mid-point rounding para garantizar determinismo en todos los entornos
Math.Round(nuevoRanking, MidpointRounding.AwayFromZero);
```

### Comentarios que NO agregan valor (❌ eliminar)
```csharp
// Obtiene el equipo   ← redundante con el código
var equipo = _repo.ObtenerEquipo(id);

// Constructor ← obviamente es el constructor
public EquipoService() { }

// Incrementa i ← peor que no tener comentario
i++;
```

---

## Formato y estructura

### Vertical
- Variables declaradas cerca de donde se usan
- Métodos relacionados juntos
- Métodos llamados cerca de los que los llaman

### Clases
- **Una razón para cambiar** (SRP)
- **Pequeñas**: preferir muchas clases pequeñas a pocas clases grandes

---

## Números mágicos y constantes

```csharp
// ❌ Números mágicos
if (ranking > 2500) ranking = 2500;
if (ranking < 300) ranking = 300;
double cambio = 30 * (resultado - probabilidad);

// ✅ Constantes nombradas con intención
private const int RANKING_MAXIMO = 2500;
private const int RANKING_MINIMO = 300;
private const int FACTOR_K = 30;

if (ranking > RANKING_MAXIMO) ranking = RANKING_MAXIMO;
double cambio = FACTOR_K * (resultado - probabilidad);
```

---

## DRY — Don't Repeat Yourself

- Si el mismo bloque de código aparece 2+ veces → extraer a método
- Si la misma lógica de negocio está en múltiples lugares → centralizar en dominio

---

## Excepciones

```csharp
// ✅ Lanzar excepción específica, no retornar null ni código de error
public Equipo ObtenerPorId(int id) {
    var equipo = _repo.FindById(id);
    if (equipo == null) throw new EquipoNoEncontradoException(id);
    return equipo;
}

// ❌ Retornar null obliga al caller a verificar siempre
public Equipo ObtenerPorId(int id) {
    return _repo.FindById(id); // puede ser null
}

// ❌ Usar excepciones para flujo de control normal
try { 
    var e = lista[indice]; 
} catch (IndexOutOfRangeException) { 
    // esto es control flow, no excepción real
}
```

---

## Checklist Clean Code rápido

- [ ] Todos los nombres son reveladores de intención
- [ ] No hay números mágicos (usar constantes nombradas)
- [ ] Los métodos hacen una sola cosa
- [ ] No hay métodos con más de 3 parámetros sin justificación
- [ ] No hay comentarios redundantes (que solo repiten el código)
- [ ] No hay código duplicado
- [ ] No hay booleanos como parámetros de métodos
- [ ] No hay código muerto (comentado, nunca llamado)
- [ ] Las excepciones son específicas y se lanzan temprano
- [ ] No se retorna null innecesariamente
