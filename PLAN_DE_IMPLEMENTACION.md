# Plan de Implementación — Sprint 4
## Tests Unitarios e Integración
> PicasYFamas API — ESCMB / UNC  
> Estado al: 30/06/2026 | Sprints 1-3 completados ✅

---

## Contexto

Los Sprints 1, 2 y 3 están completados. La API compila y funciona con:
- Registro y login con JWT
- Inicio de juego asociado al jugador
- Adivinar número usando `ESCMB.GameCore`
- Migración EF Core generada

**Único pendiente: Sprint 4 — Tests**

---

## Sprint 4 — Tests Unitarios e Integración

### Objetivos
1. Escribir tests unitarios que cubran la lógica de dominio y los handlers
2. Escribir al menos un test de integración de flujo completo
3. Verificar que `dotnet test` pasa sin errores

---

### 4.1 Setup del Proyecto `PicasYFamas.UnitTests`

#### Paquetes a agregar
```bash
dotnet add tests/PicasYFamas.UnitTests package Moq
dotnet add tests/PicasYFamas.UnitTests package FluentAssertions
```

#### Estructura de carpetas a crear
```
tests/PicasYFamas.UnitTests/
├── Domain/
│   ├── SecretNumberTests.cs
│   └── GameTests.cs
├── Application/
│   ├── CreateGameCommandHandlerTests.cs
│   ├── MakeGuessCommandHandlerTests.cs
│   ├── RegisterPlayerCommandHandlerTests.cs
│   └── LoginCommandHandlerTests.cs
└── Helpers/
    └── MockFactory.cs   (helpers para crear mocks reutilizables)
```

---

### 4.2 Tests de Dominio — `SecretNumberTests.cs`

**Archivo:** `tests/PicasYFamas.UnitTests/Domain/SecretNumberTests.cs`

Casos a cubrir:
- [ ] `Create` con número válido (4 dígitos únicos) → no lanza excepción
- [ ] `Create` con string vacío → lanza `DomainException`
- [ ] `Create` con menos de 4 dígitos → lanza `DomainException`
- [ ] `Create` con más de 4 dígitos → lanza `DomainException`
- [ ] `Create` con letras → lanza `DomainException`
- [ ] `Create` con dígitos repetidos → lanza `DomainException`
- [ ] `GenerateRandom` → retorna número de 4 dígitos únicos
- [ ] `GenerateRandom` → cada llamada puede retornar un número diferente (probabilístico)

---

### 4.3 Tests de Dominio — `GameTests.cs`

**Archivo:** `tests/PicasYFamas.UnitTests/Domain/GameTests.cs`

Casos a cubrir:
- [ ] Nuevo juego → `Status == InProgress`
- [ ] `MakeGuess` con número inválido → lanza `DomainException`
- [ ] `MakeGuess` sobre juego no `InProgress` → lanza `DomainException`
- [ ] `MakeGuess` con el número correcto → `Status == Won`
- [ ] `MakeGuess` agotando todos los intentos → `Status == Lost`
- [ ] `Cancel()` en juego activo → `Status == Cancelled`
- [ ] Múltiples intentos → `Guesses.Count` incrementa correctamente
- [ ] `PlayerId` asignado correctamente al construir

---

### 4.4 Tests de Application — `RegisterPlayerCommandHandlerTests.cs`

**Archivo:** `tests/PicasYFamas.UnitTests/Application/RegisterPlayerCommandHandlerTests.cs`

Mocks necesarios: `IPlayerRepository`, `IJwtService`, `IPasswordHasher`

Casos a cubrir:
- [ ] Email no registrado → crea jugador y retorna `AuthResponseDto` con token
- [ ] Email ya registrado → lanza `DomainException("Email already registered.")`
- [ ] Verifica que `IPasswordHasher.Hash()` es llamado exactamente una vez
- [ ] Verifica que `IPlayerRepository.AddAsync()` es llamado exactamente una vez
- [ ] Verifica que `IJwtService.GenerateToken()` es llamado con el `PlayerId` correcto

---

### 4.5 Tests de Application — `LoginCommandHandlerTests.cs`

**Archivo:** `tests/PicasYFamas.UnitTests/Application/LoginCommandHandlerTests.cs`

Mocks necesarios: `IPlayerRepository`, `IJwtService`, `IPasswordHasher`

Casos a cubrir:
- [ ] Credenciales correctas → retorna `AuthResponseDto` con token
- [ ] Email no encontrado → lanza `DomainException("Invalid email or password.")`
- [ ] Password incorrecto → lanza `DomainException("Invalid email or password.")`
- [ ] Verifica que `IPasswordHasher.Verify()` es llamado con el hash almacenado

---

### 4.6 Tests de Application — `MakeGuessCommandHandlerTests.cs`

**Archivo:** `tests/PicasYFamas.UnitTests/Application/MakeGuessCommandHandlerTests.cs`

Mocks necesarios: `IGameRepository`

Casos a cubrir:
- [ ] Juego no encontrado → lanza `DomainException("Game not found.")`
- [ ] Juego ya terminado → lanza `DomainException("Game is already finished.")`
- [ ] Intento válido → retorna `GuessResultDto` con `Pica`, `Fama`, `Message`
- [ ] Verifica que `IGameRepository.UpdateAsync()` es llamado exactamente una vez

> ⚠️ **Nota:** `Evaluator.ValidateAttempt` es estático. Verificar si `ESCMB.GameCore` expone una interfaz o si hay que wrappearlo para testear de forma aislada.

---

### 4.7 Tests de Application — `CreateGameCommandHandlerTests.cs`

**Archivo:** `tests/PicasYFamas.UnitTests/Application/CreateGameCommandHandlerTests.cs`

Mocks necesarios: `IGameRepository`, `IPlayerRepository`

Casos a cubrir:
- [ ] Jugador no encontrado → lanza `DomainException("Player not found.")`
- [ ] Jugador con juego activo → lanza `DomainException("You already have an active game...")`
- [ ] Flujo exitoso → retorna `StartGameResponseDto` con `GameId`, `PlayerId`, `CreatedAt`
- [ ] Verifica que `IGameRepository.AddAsync()` es llamado exactamente una vez

---

### 4.8 Setup del Proyecto `PicasYFamas.IntegrationTests`

#### Paquetes a agregar
```bash
dotnet add tests/PicasYFamas.IntegrationTests package Microsoft.AspNetCore.Mvc.Testing
dotnet add tests/PicasYFamas.IntegrationTests package Microsoft.EntityFrameworkCore.InMemory
```

#### Estructura a crear
```
tests/PicasYFamas.IntegrationTests/
├── Setup/
│   └── ApiFactory.cs     (WebApplicationFactory con DB en memoria)
└── GameFlowTests.cs      (flujo completo: register → login → start → guess)
```

---

### 4.9 Test de Integración — Flujo Completo

**Archivo:** `tests/PicasYFamas.IntegrationTests/GameFlowTests.cs`

Casos a cubrir:
- [ ] `POST /register` → `200 OK` con token JWT en la respuesta
- [ ] `POST /login` con credenciales correctas → `200 OK` con token
- [ ] `POST /login` con credenciales incorrectas → `400 BadRequest`
- [ ] `POST /start` sin token → `401 Unauthorized`
- [ ] `POST /start` con token válido → `200 OK` con `gameid`, `playerid`, `createat`
- [ ] `POST /start` con juego activo existente → `400 BadRequest`
- [ ] `POST /guess` sin token → `401 Unauthorized`
- [ ] `POST /guess` con número inválido → `400 BadRequest`
- [ ] `POST /guess` con número válido → `200 OK` con mensaje de pistas

---

## Checklist de Ejecución

```
[ ] 4.1  Instalar Moq + FluentAssertions en UnitTests
[ ] 4.2  Crear SecretNumberTests.cs
[ ] 4.3  Crear GameTests.cs
[ ] 4.4  Crear RegisterPlayerCommandHandlerTests.cs
[ ] 4.5  Crear LoginCommandHandlerTests.cs
[ ] 4.6  Crear MakeGuessCommandHandlerTests.cs
[ ] 4.7  Crear CreateGameCommandHandlerTests.cs
[ ] 4.8  Instalar Mvc.Testing + InMemory en IntegrationTests
[ ] 4.9  Crear ApiFactory.cs (WebApplicationFactory)
[ ] 4.10 Crear GameFlowTests.cs (flujo completo)
[ ] 4.11 dotnet test → todos los tests pasan
```

---

## Criterio de Éxito del Sprint 4

```bash
dotnet test
# ✅ Todos los tests en verde
# ✅ 0 tests fallidos
```

---

## Decisión Pendiente: Evaluator estático

`ESCMB.GameCore.Evaluator.ValidateAttempt()` es un método estático. Opciones:

| Opción | Pro | Contra |
|--------|-----|--------|
| **A)** Crear `IGameCoreService` wrapper | Testeable con mocks | Un archivo más |
| **B)** Usar la clase estática directamente en tests | Simple | Tests acoplados al resultado real de GameCore |

**Recomendación:** Opción B para los tests unitarios del handler — se puede verificar que el handler llama al repositorio y devuelve el resultado, sin mockear GameCore (que es una librería externa de confianza institucional).
