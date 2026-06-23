# Picas y Famas (Bulls and Cows) API

Esta es la implementación del clásico juego Picas y Famas (Bulls and Cows) construida como una API REST profesional, lista para producción y fácilmente escalable.

## 🚀 Arquitectura y Tecnologías

El proyecto fue desarrollado utilizando **Clean Architecture** y principios **Domain-Driven Design (DDD)** para garantizar alto nivel de mantenibilidad, desacoplamiento y testing.

### Stack Tecnológico
* **.NET 9** (Web API)
* **C# 13**
* **PostgreSQL** + Entity Framework Core 9 (Code-First)
* **MediatR** (CQRS Pattern)
* **FluentValidation** (Validation Pipeline)
* **Serilog** (Structured Logging)
* **OpenTelemetry** (Metrics and Distributed Tracing)
* **xUnit + FluentAssertions + Moq** (Unit Tests)
* **Testcontainers** (Integration Tests)
* **Asp.Versioning** (API Versioning)
* **Swagger/OpenAPI** (Documentación)
* **Docker & Docker Compose**

### Capas del Proyecto
1. **Domain**: Contiene entidades ricas (`Game`), Value Objects inmutables (`SecretNumber`), y excepciones de dominio. No tiene dependencias externas.
2. **Application**: Contiene los Casos de Uso (Commands, Queries) mediante MediatR, DTOs (`ApiResponse` envelope uniforme), y Pipeline Behaviors (Validaciones automáticas).
3. **Infrastructure**: Implementación de persistencia con EF Core (DbContext, Configurations) y Repositorios.
4. **Api**: Controladores REST versionados, Middlewares globales de manejo de errores, Rate Limiting y Observabilidad.

## ⚙️ Funcionalidades
* Crear una nueva partida (genera número secreto único de 4 dígitos).
* Realizar intentos con validaciones robustas.
* Historial de intentos por partida.
* Límite de 10 intentos máximos (`GameStatus: Won, Lost, InProgress`).
* Responses uniformes para frontend (`{ success, data, errors }`).
* Observabilidad integrada.

## 🛠️ Cómo Ejecutar

### Usando Docker (Recomendado)
El proyecto contiene la definición en `docker-compose` para la base de datos PostgreSQL y la API.

```bash
docker-compose up --build
```
La API estará disponible en `http://localhost:8080/swagger`

### Desarrollo Local (Sin Docker)
Requiere PostgreSQL y .NET 9 SDK instalados.

1. Actualizar el `ConnectionStrings:DefaultConnection` en `appsettings.json`.
2. Aplicar las migraciones:
   ```bash
   dotnet ef database update --project src/PicasYFamas.Infrastructure --startup-project src/PicasYFamas.Api
   ```
3. Ejecutar:
   ```bash
   dotnet run --project src/PicasYFamas.Api
   ```

## 🧪 Pruebas Automatizadas

Para correr los tests (Unitarios e Integración con Testcontainers):

```bash
dotnet test
```

## 📐 Decisiones de Diseño
* **CQRS con MediatR**: Facilita separar lectura de escritura, lo cual prepara la API para escalar en caso de agregar funcionalidades complejas o multiusuario en un futuro.
* **Pipeline Behaviors**: Para quitar la validación de los controladores y del dominio. Se valida antes de entrar al handler, dejando el código más limpio.
* **Envelope Pattern en Responses**: `ApiResponse<T>` hace que consumir la API desde frameworks modernos (React, Flutter) sea predecible.
* **Testcontainers**: Garantiza que las pruebas de integración prueban verdaderamente las transacciones con PostgreSQL y no con un mock en memoria que se comporta diferente.
* **Value Object `SecretNumber`**: Encapsula y asegura la validez del número de 4 dígitos siempre. No se puede crear un juego en estado inválido.
