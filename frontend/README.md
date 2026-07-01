# Picas & Famas — Frontend

Frontend estático (HTML + CSS + JS puro, sin frameworks ni build step) para
el backend `TrabajoIntegradorPicas-FamasProgramacion` (ASP.NET Core).

Estética: pixel art / arcade tipo *Balatro* — panel "cabinet" con borde grueso,
scanlines de CRT, tipografía pixelada (`Press Start 2P` + `VT323`) y un
marcador de intentos con fichas que aparecen con un pequeño "pop".

## Archivos

- `index.html` — estructura (login, registro, juego)
- `style.css` — sistema visual 8-bit
- `app.js` — lógica: auth, llamadas a la API, render del marcador

## Cómo correrlo

1. Levantá el backend (por defecto en `http://localhost:5163`):
   ```
   dotnet run --project src/PicasYFamas.Api
   ```
2. Abrí `index.html` con un servidor estático simple (no hace falta build).
   Por ejemplo, desde esta carpeta:
   ```
   npx serve .
   ```
   o con la extensión "Live Server" de VS Code, o `python -m http.server`.

   ⚠️ Abrir el `index.html` con doble clic (protocolo `file://`) puede fallar
   por CORS en algunos navegadores — usar un servidor local evita el problema.

3. En la app, tocá el ícono ⚙ (arriba a la derecha) si tu backend corre en
   otra URL/puerto (por ejemplo `https://localhost:7120/api/game/v1`).
   Por defecto apunta a `http://localhost:5163/api/game/v1`.

4. Registrate o iniciá sesión, tocá **NUEVA PARTIDA** y empezá a adivinar el
   número secreto de 4 dígitos (todos distintos).

## Cómo se conecta con el backend

Endpoints usados (`Controllers/AuthController.cs` y `GamesController.cs`):

| Acción | Endpoint |
|---|---|
| Registro | `POST /api/game/v1/register` |
| Login | `POST /api/game/v1/login` |
| Nueva partida | `POST /api/game/v1/start` (requiere JWT) |
| Intentar | `POST /api/game/v1/guess` (requiere JWT) |
| Estado de partida | `GET /api/game/v1/{gameId}` |
| Historial de intentos | `GET /api/game/v1/{gameId}/guesses` |

El JWT y el `gameId` activo se guardan en `localStorage` para mantener la
sesión y la partida si recargás la página.

## Notas

- La validación de "4 dígitos sin repetir" se hace también en el cliente
  para dar feedback inmediato, pero la regla real vive en el dominio
  (`SecretNumber.Create`), así que el backend la vuelve a validar.
- Si el backend devuelve 401, la app cierra la sesión local automáticamente.
- Personalización de paleta/tipografía: ver los `:root` de `style.css`.
