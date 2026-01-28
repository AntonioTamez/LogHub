# LogHub - Sistema de Logs y Telemetria

Sistema centralizado para agregar, almacenar y visualizar logs de multiples aplicaciones.

## Stack Tecnologico

- **Backend**: .NET 8 (ASP.NET Core Web API + Worker Service)
- **Base de datos**: PostgreSQL 16
- **Cache/Buffer**: Redis 7
- **Cola de mensajes**: RabbitMQ 3.12
- **Frontend**: Angular 17 con Material Design
- **Graficas**: Chart.js
- **Tiempo real**: SignalR

## Arquitectura

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   Aplicaciones  │────▶│   LogHub API    │────▶│    RabbitMQ     │
│   (SDK Client)  │     │   (Ingesta)     │     │    (Cola)       │
└─────────────────┘     └────────┬────────┘     └────────┬────────┘
                                 │                       │
                                 │ SignalR               │
                                 ▼                       ▼
                        ┌─────────────────┐     ┌─────────────────┐
                        │    Dashboard    │     │     Worker      │
                        │    (Angular)    │     │  (Procesador)   │
                        └─────────────────┘     └────────┬────────┘
                                                         │
                                                         ▼
                                                ┌─────────────────┐
                                                │   PostgreSQL    │
                                                │  (Persistencia) │
                                                └─────────────────┘
```

## Requisitos Previos

- .NET 8 SDK
- Node.js 20+ y npm
- Angular CLI 17
- Docker Desktop

## Inicio Rapido

### 1. Clonar e iniciar servicios

```bash
# Iniciar infraestructura (PostgreSQL, RabbitMQ, Redis)
docker-compose up -d postgres rabbitmq redis

# Esperar a que los servicios esten listos
docker-compose ps
```

### 2. Ejecutar migraciones

```bash
cd src/LogHub.API
dotnet ef database update
```

### 3. Iniciar la API

```bash
cd src/LogHub.API
dotnet run
```

### 4. Iniciar el Worker

```bash
cd src/LogHub.Worker
dotnet run
```

### 5. Iniciar el Dashboard

```bash
cd dashboard
npm install
ng serve
```

## URLs de Acceso

| Servicio | URL |
|----------|-----|
| API | http://localhost:5100 |
| Dashboard | http://localhost:4200 |
| Swagger | http://localhost:5100/swagger |
| RabbitMQ Management | http://localhost:15672 |

## Uso del SDK

### Instalacion

```bash
dotnet add package LogHub.Client
```

### Configuracion

```csharp
// Program.cs
builder.Services.AddLogHub(options =>
{
    options.ApiUrl = "http://localhost:5100";
    options.ApiKey = "tu-api-key";
    options.ApplicationName = "MiAplicacion";
});
```

### Uso

```csharp
public class MiServicio
{
    private readonly ILogger<MiServicio> _logger;

    public MiServicio(ILogger<MiServicio> logger)
    {
        _logger = logger;
    }

    public void HacerAlgo()
    {
        _logger.LogInformation("Operacion iniciada");
        // ...
        _logger.LogError("Error en operacion: {Error}", ex.Message);
    }
}
```

## Endpoints de la API

### Autenticacion (JWT)

| Metodo | Endpoint | Descripcion |
|--------|----------|-------------|
| POST | /api/auth/register | Registrar usuario |
| POST | /api/auth/login | Iniciar sesion |

### Logs (API Key)

| Metodo | Endpoint | Descripcion |
|--------|----------|-------------|
| POST | /api/logs | Enviar log individual |
| POST | /api/logs/batch | Enviar multiples logs |

### Consultas (JWT)

| Metodo | Endpoint | Descripcion |
|--------|----------|-------------|
| GET | /api/logs | Consultar logs con filtros |
| GET | /api/logs/stats | Estadisticas para dashboard |
| GET | /api/applications | Listar aplicaciones |
| POST | /api/applications | Registrar aplicacion |

### SignalR

| Hub | Descripcion |
|-----|-------------|
| /hubs/logs | Stream de logs en tiempo real |

## Docker

### Build completo

```bash
docker-compose up -d --build
```

### Escalar workers

```bash
docker-compose up -d --scale worker=3
```

### Ver logs

```bash
docker-compose logs -f api
docker-compose logs -f worker
```

## Estructura del Proyecto

```
LogHub/
├── src/
│   ├── LogHub.API/           # Web API
│   ├── LogHub.Worker/        # Worker Service
│   ├── LogHub.Core/          # Entidades e interfaces
│   ├── LogHub.Infrastructure/# DbContext y repositorios
│   └── LogHub.Client/        # SDK NuGet
├── dashboard/                # Angular 17
├── tests/                    # Pruebas
└── docker-compose.yml
```

## Licencia

MIT
