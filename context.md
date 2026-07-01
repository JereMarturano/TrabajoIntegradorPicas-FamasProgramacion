# Contexto General del Proyecto: API RESTful - Juego "Picas y Famas"

## 1. Información Académica e Institucional
* **Institución:** Escuela Superior de Comercio Manuel Belgrano (ESCMB) - Universidad Nacional de Córdoba (UNC).
* **Carrera:** Analista Universitario en Sistemas.
* **Materia:** Programación Lógica Aplicada II.
* **Asignación:** Actividad Práctica Integradora - Unidad 2.

---

## 2. Descripción General del Sistema
El objetivo es desarrollar el backend de una aplicación full-stack para jugar a **Picas y Famas** en modalidad solitario. 
* **Frontend (Referencia):** Desarrollado en Vercel o BoltNew.
* **Backend (Foco):** API RESTful desarrollada en **C#** con persistencia de datos (Entity Framework Core recomendado) y autenticación mediante **JWT**.

### Reglas del Juego (Picas y Famas)
1. El sistema genera un número secreto aleatorio de **4 dígitos sin repetir**.
2. El jugador intenta adivinar el número enviando un número de 4 dígitos (sin repetir).
3. El sistema responde con pistas basadas en el intento:
   * **Famas:** Dígitos correctos en la **posición correcta**.
   * **Picas:** Dígitos correctos pero en una **posición incorrecta**.
4. El juego termina cuando el jugador logra 4 Famas (adivina el número). El objetivo es lograrlo en la menor cantidad de intentos.

---

## 3. Requerimientos Funcionales y Endpoints

### 3.1. Registro de Usuarios
* **Endpoint:** `POST api/game/v1/register`
* **Campos Obligatorios (Request Body):** `lastname`, `firstname`, `age`, `email`, `password`.
* **Reglas de Negocio:**
  * Validar campos obligatorios (si faltan, devolver `400 BadRequest`).
  * Verificar que el email no esté registrado previamente (si ya existe, devolver `400 BadRequest`).
  * Generar un identificador único (**UUID**) y registrar la `fecha_creacion`.
  * **Autenticación Automática:** Al registrarse con éxito, debe iniciar sesión automáticamente, generar un token JWT (que incluya el `playerId`) y devolverlo en la respuesta (`200 OK`).

### 3.2. Login de Usuarios
* **Endpoint:** `POST api/game/v1/login`
* **Campos Requeridos:** `email`, `password`.
* **Reglas de Negocio:**
  * Si las credenciales son inválidas o el usuario no existe, retornar error (`400 BadRequest`) sin token.
  * Si son correctas, generar un nuevo token JWT y retornar `200 OK`.

### 3.3. Inicio del Juego
* **Endpoint:** `POST api/game/v1/start`
* **Seguridad:** Requiere encabezado `Authorization: bearer <token>`. Si no es válido o está vencido, retornar `401 Unauthorized`.
* **Reglas de Negocio:**
  * Validar si el jugador **ya tiene un juego activo**. No se debe permitir iniciar uno nuevo hasta terminar el actual (informar al usuario).
  * Generar un nuevo juego: ID de juego, número secreto (4 dígitos aleatorios no repetidos) y fecha de creación. Persistir en la base de datos.
  * Retornar `gameid`, `playerid` y `createat`.

### 3.4. Adivinar el Número Secreto (Ciclo de Juego)
* **Endpoint:** `POST api/game/v1/guess`
* **Seguridad:** Requiere encabezado `Authorization: bearer <token>`.
* **Campos Requeridos (Body):** `gameid`, `attemptedNumber`.
* **Reglas de Negocio:**
  * Validar que `gameid` tenga el formato correcto y exista.
  * Validar que `attemptedNumber` sea de 4 dígitos y no tenga números repetidos (si falla, `400 BadRequest`).
  * Validar que el juego **no haya finalizado** (`Finished`). Si ya terminó, bloquear el intento.
  * **Cálculo de Picas y Famas:** Es **OBLIGATORIO** consumir el paquete NuGet institucional **`ESCMB.GameCore`** disponible en: `https://www.nuget.org/packages/ESCMB.GameCore`. Utilizar el algoritmo de esta librería para obtener los mensajes de pistas (e.g., *"Tu número tiene 1 fama y 2 pica"*).
  * Cada intento (`Attempt`) debe persistirse en la base de datos.
  * Si el intento alcanza las 4 Famas (el mensaje del Core confirma el acierto), marcar el estado del juego como **`Finished`**.

---

## 4. Estructura de Código Recomendada
El agente debe respetar la siguiente estructura de solución para mantener el orden del proyecto ASP.NET Core:

```text
NumberGuessGameApi/
│
├── Controllers/
│   └── GameController.cs
│
├── Models/
│   ├── Player.cs
│   ├── Game.cs
│   └── Attempt.cs
│
├── Data/
│   └── GameDbContext.cs
│
├── DataTransferObjects/ (DTOs)
│   ├── RegisterPlayerRequest.cs
│   ├── RegisterPlayerResponse.cs
│   ├── StartGameRequest.cs
│   ├── StartGameResponse.cs
│   ├── GuessNumberRequest.cs
│   └── GuessNumberResponse.cs
│
├── Services/
│   ├── IGameService.cs
│   └── GameService.cs
│
├── Migrations/
├── Program.cs
├── Startup.cs
└── appsettings.json

---

## 5. Estado de Implementación
> Última actualización: 30/06/2026

### Arquitectura Elegida
Se implementó **Clean Architecture** en 4 capas en lugar de la estructura mínima sugerida. Esto es válido y excede los requisitos.

```
src/
├── PicasYFamas.Domain          → Entidades, ValueObjects, Interfaces de Repos, Excepciones
├── PicasYFamas.Application     → CQRS (Commands/Queries), DTOs, Validators, MediatR
├── PicasYFamas.Infrastructure  → EF Core + PostgreSQL, Repos concretos, JWT, BCrypt
└── PicasYFamas.Api             → Controllers, Middleware, Program.cs
tests/
├── PicasYFamas.UnitTests        → (pendiente Sprint 4)
└── PicasYFamas.IntegrationTests → (pendiente Sprint 4)
```

### Paquetes NuGet Instalados
| Paquete | Versión | Proyecto |
|---------|---------|---------|
| `ESCMB.GameCore` | 1.0.0 | Application + Infrastructure |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 9.0.0 | Api + Infrastructure |
| `BCrypt.Net-Next` | 4.0.3 | Infrastructure |
| `MediatR` | 14.1.0 | Application |
| `FluentValidation` | 12.1.1 | Application |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 9.0.0 | Infrastructure |
| `Serilog.AspNetCore` | 10.0.0 | Api |
| `Swashbuckle.AspNetCore` | 6.5.0 | Api |

### Endpoints Implementados
| Endpoint | Método | Auth | Estado |
|----------|--------|------|--------|
| `api/game/v1/register` | POST | ❌ Público | ✅ Completo |
| `api/game/v1/login` | POST | ❌ Público | ✅ Completo |
| `api/game/v1/start` | POST | ✅ JWT | ✅ Completo |
| `api/game/v1/guess` | POST | ✅ JWT | ✅ Completo |

### Completitud por Sprint
- ✅ **Sprint 1** — Fundamentos (Player, JWT, BCrypt, GameCore, Migración): **COMPLETO**
- ✅ **Sprint 2** — Endpoints obligatorios (register, login, start, guess): **COMPLETO**
- ✅ **Sprint 3** — Correcciones (NotFoundException, Serilog, tipado): **COMPLETO**
- ⏳ **Sprint 4** — Tests unitarios + integración: **PENDIENTE**

### Notas Importantes
- El número secreto se genera con `SecretNumber.GenerateRandom()` (4 dígitos únicos).
- El cálculo de picas/famas usa **`ESCMB.GameCore` → `Evaluator.ValidateAttempt(secret, attempt)`**.
- Propiedades del resultado: `result.Pica`, `result.Fama`, `result.Message`.
- La migración `InitialCreate` está generada. Para aplicarla:
  ```bash
  dotnet ef database update --project src/PicasYFamas.Infrastructure --startup-project src/PicasYFamas.Api
  ```
- Para levantar la API:
  ```bash
  dotnet run --project src/PicasYFamas.Api
  ```
- Swagger UI disponible en `https://localhost:{port}/swagger` (modo Development).
- El botón "Authorize" en Swagger acepta el token JWT con formato `Bearer {token}`.