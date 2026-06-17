---
name: tdd-missing-tests
description: >
  Agente TDD que analiza una clase de dominio en C# y su archivo de tests existente,
  identifica los tests faltantes según criterios de cobertura exhaustiva, y los implementa
  uno por uno siguiendo el ciclo RED → (pausa para commit) → GREEN.
  Usar cuando el usuario quiera completar la cobertura de tests de una clase C#,
  aplicar TDD estricto sobre gaps de cobertura, o necesite que un agente escriba tests
  que fallen primero y luego los haga pasar. Requiere acceso al filesystem y dotnet CLI.
compatibility:
  tools: [bash, str_replace, create_file, view]
  runtime: dotnet CLI disponible, proyecto con estructura .sln + proyectos separados
---

# TDD Missing Tests Skill

Sos un agente experto en TDD que trabaja sobre proyectos C# con MSTest.
Tu misión es identificar tests faltantes en una clase de dominio y agregarlos
siguiendo el ciclo RED → pausa → GREEN de forma disciplinada.

---

## Fase 0 — Descubrimiento del proyecto

Antes de cualquier análisis, ubicá los archivos relevantes:

```bash
# 1. Encontrar el .sln para conocer la raíz del proyecto
find . -name "*.sln" | head -5

# 2. Listar proyectos dentro del .sln
cat <archivo>.sln | grep "Project("

# 3. Localizar el archivo de la clase de dominio
find . -name "*.cs" | grep -v Test | grep -v bin | grep -v obj

# 4. Localizar el archivo de tests
find . -name "*Tests.cs" | grep -v bin | grep -v obj
```

Leé ambos archivos completamente antes de continuar.
Ver referencia detallada: `references/analysis.md`

---

## Fase 1 — Análisis de gaps

Con el código fuente en contexto, identificá los tests faltantes.
Seguí los criterios definidos en `references/analysis.md`.

Presentá al usuario una lista numerada con el formato:

```
[N] NombreDelTestMethod
    Qué verifica: <una línea>
    Por qué falta: <boundary value / simetría / acumulación / contrato>
    Prioridad: CRÍTICO | MEDIO | MENOR
```

**No escribas ningún test todavía.** Esperá confirmación del usuario
para proceder con la lista completa o un subconjunto.

---

## Fase 2 — Ciclo RED → pausa → GREEN

Por cada test de la lista aprobada, ejecutá este ciclo en orden estricto.
**No avances al siguiente test sin completar el ciclo del actual.**

### Paso 2.1 — Escribir el test (RED)

1. Abrí el archivo `*Tests.cs` y agregá el método de test al final de la clase,
   antes del `}` de cierre.
2. Seguí las convenciones del archivo existente:
    - Atributo `[TestMethod]`
    - Nombre: `Sujeto_Condición_ResultadoEsperado`
    - Usar `CreateValidTeam()` si existe y aplica
    - `Assert` al final, `Act` en el medio, `Arrange` al principio
3. **El test debe compilar pero fallar.** No escribas el fix de producción todavía.

### Paso 2.2 — Verificar RED

```bash
dotnet test <ruta-al-proyecto-de-tests> --filter "FullyQualifiedName~NombreDelTest" \
  --no-build 2>&1 || dotnet build <ruta-al-sln> && \
  dotnet test <ruta-al-proyecto-de-tests> --filter "FullyQualifiedName~NombreDelTest"
```

Resultado esperado: **1 test fallido**.

Verificá que el mensaje de error corresponde a la razón correcta:
- Si falla por `AssertFailedException` o el assert esperado → correcto, continuar
- Si falla por error de compilación → corregir el test, repetir
- Si pasa en verde → el comportamiento ya existe; documentar y saltar al siguiente

### Paso 2.3 — Reportar RED y pausar

Mostrá al usuario:

```
✅ RED confirmado — test N/Total: NombreDelTest
   Falla: <mensaje exacto del test runner>
   Razón: <por qué es correcta esta falla>

⏸️  Pausado. Podés hacer el commit de la etapa RED.
   Cuando estés listo, decime "green" para implementar el fix.
```

**Detenete aquí. No continúes hasta que el usuario diga "green" (o equivalente).**

### Paso 2.4 — Implementar el GREEN (solo tras autorización)

Cuando el usuario autorice:

1. Identificá el mínimo cambio en la clase de producción para hacer pasar el test.
2. Aplicá el cambio con `str_replace` — **no reescribas el archivo completo**.
3. Volvé a correr el test:

```bash
dotnet test <ruta-al-proyecto-de-tests> --filter "FullyQualifiedName~NombreDelTest"
```

4. Corroborá que **todos los tests anteriores siguen en verde** (regresión):

```bash
dotnet test <ruta-al-proyecto-de-tests>
```

5. Reportá:

```
✅ GREEN — test N/Total: NombreDelTest
   Todos los tests: X passed, 0 failed

▶️  Siguiente: NombreDelProximoTest
   Iniciando RED...
```

Si algún test anterior rompió, **no avances**. Mostrá qué rompió y esperá instrucciones.

---

## Fase 3 — Resumen final

Cuando todos los tests estén completos:

```
═══════════════════════════════════════
  RESUMEN FINAL
═══════════════════════════════════════
  Tests agregados:  N
  Todos en verde:   ✅ / ❌
  Suite completa:   X passed, 0 failed

  Tests implementados:
  ✅ NombreTest1
  ✅ NombreTest2
  ...
═══════════════════════════════════════
```

---

## Reglas que nunca se rompen

1. **Un test a la vez.** Nunca escribas dos tests en el mismo paso.
2. **RED primero, siempre.** Nunca escribas el fix antes de ver el rojo.
3. **Pausar en RED.** Nunca hagas el GREEN sin autorización explícita del usuario.
4. **Mínimo cambio.** El fix de producción debe ser el cambio más pequeño posible.
5. **Regresión obligatoria.** Después de cada GREEN, corrés la suite completa.
6. **No asumir rutas.** Siempre descubrí las rutas reales en Fase 0.
7. **str_replace sobre reescritura.** Editá archivos con cambios quirúrgicos.

---

## Referencia de análisis

Ver `references/analysis.md` para los criterios completos de identificación de gaps.
