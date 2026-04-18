# Resumen de Cambios - Migración de User a Usuario

## Fecha de Migración
**Fecha:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

## Descripción General
Se ha realizado una refactorización completa del proyecto para cambiar la entidad `User` por `Usuario`, adaptándola a la estructura de la tabla MySQL `usuario` existente.

## Cambios Realizados por Capa

### 1. **Domain Layer (LuseGestion.Domain)**

#### Entidad Principal
- **Archivo:** `Usuario.cs` (anteriormente `User.cs`)
- **Cambios en la estructura:**
  - ID cambió de `Guid` a `int` (para coincidir con `IDUsuario` AUTO_INCREMENT)
  - Campos agregados:
    - `Nombre` (antes era `FirstName` y `LastName` separados)
    - `Pass` (contraseña)
    - `EmailConfirmado` (bool)
    - `EmailConfirmadoToken` (string?)
    - `EmailConfirmadoTokenExpira` (DateTime?)
    - `Telefono` (string?)
    - `FechaCambioPass` (DateTime?)
    - `IntentosFallidos` (int?)
    - `IDPerfil` (int?)
  - `IsActive` cambió a `Activo`
  - `Email` se mantiene

#### BaseEntity
- **Archivo:** `Primitives\BaseEntity.cs`
- **Cambios:**
  - `Id` cambió de `Guid` (auto-generado) a `int` (asignado por la BD)
  - `CreatedAt` y `UpdatedAt` ahora son `DateTime?` (nullable)

#### Interfaces
- **IUsuarioRepository.cs** (antes `IUserRepository.cs`)
  - Todos los métodos actualizados para usar `int` en lugar de `Guid`
  - Agregado método `GetByEmailAndPasswordAsync` para autenticación

- **IUsuarioService.cs** (antes `IUserService.cs`)
  - Métodos actualizados con nuevos parámetros según la estructura de Usuario
  - Agregados métodos:
    - `AuthenticateAsync`
    - `ChangePasswordAsync`
    - `ActivateUsuarioAsync`
    - `DeactivateUsuarioAsync`

### 2. **Application Layer (LuseGestion.Application)**

#### Servicio
- **Archivo:** `UsuarioService.cs` (anteriormente `UserService.cs`)
- **Cambios:**
  - Inyección de dependencias actualizada a `IUsuarioRepository`
  - Todos los métodos refactorizados para usar la nueva estructura
  - Validaciones en español
  - Nuevos métodos implementados:
    - Autenticación con email y contraseña
    - Cambio de contraseña con validación
    - Activación/Desactivación de usuarios

### 3. **Infrastructure Layer (LuseGestion.Infrastructure)**

#### Repositorio
- **Archivo:** `UsuarioRepository.cs` (anteriormente `UserRepository.cs`)
- **Cambios:**
  - Todas las consultas SQL actualizadas para usar la tabla `usuario`
  - Mapeo de columnas MySQL a propiedades de C#:
    ```
    IDUsuario -> Id
    Nombre -> Nombre
    Email -> Email
    Pass -> Pass
    EmailConfirmado -> EmailConfirmado
    EmailConfirmadoToken -> EmailConfirmadoToken
    EmailConfirmadoTokenExpira -> EmailConfirmadoTokenExpira
    Telefono -> Telefono
    FechaCambioPass -> FechaCambioPass
    IntentosFallidos -> IntentosFallidos
    Activo -> Activo
    IDPerfil -> IDPerfil
    ```
  - Uso de `LAST_INSERT_ID()` para obtener el ID generado en inserts
  - Método de autenticación implementado

### 4. **API Layer (LuseGestion.API)**

#### Controller
- **Archivo:** `UsuariosController.cs` (anteriormente `UsersController.cs`)
- **Ruta:** `/api/Usuarios` (antes `/api/Users`)
- **Cambios en endpoints:**
  - `GET /api/Usuarios/{id:int}` (antes usaba `guid`)
  - `GET /api/Usuarios/by-email/{email}`
  - `GET /api/Usuarios`
  - `POST /api/Usuarios`
  - `PUT /api/Usuarios/{id:int}`
  - `DELETE /api/Usuarios/{id:int}`
  - **Nuevos endpoints:**
    - `POST /api/Usuarios/authenticate` - Autenticación de usuarios
    - `POST /api/Usuarios/{id:int}/change-password` - Cambio de contraseña
    - `PUT /api/Usuarios/{id:int}/activate` - Activar usuario
    - `PUT /api/Usuarios/{id:int}/deactivate` - Desactivar usuario

#### DTOs
- **CreateUsuarioRequest.cs** (antes `CreateUserRequest.cs`)
  - Propiedades: `Email`, `Nombre`, `Password`, `Telefono`, `IDPerfil`
  - Validaciones en español

- **UpdateUsuarioRequest.cs** (antes `UpdateUserRequest.cs`)
  - Propiedades: `Nombre`, `Telefono`, `IDPerfil`
  - Validaciones en español

- **UsuarioResponse.cs** (antes `UserResponse.cs`)
  - Incluye todos los campos de la entidad (excepto contraseña)
  - Método `FromDomainUsuario` para mapeo

- **Nuevos DTOs:**
  - `AuthenticateRequest` - Para login (Email, Password)
  - `ChangePasswordRequest` - Para cambio de contraseña

#### Program.cs
- Registro de dependencias actualizado:
  ```csharp
  builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
  builder.Services.AddScoped<IUsuarioService, UsuarioService>();
  ```

## Base de Datos

### Script SQL
Se ha creado el archivo `Database\CreateUsuarioTable.sql` con:
- Estructura completa de la tabla `usuario`
- Índices optimizados (email único, activo, emailConfirmado)
- Datos de ejemplo (opcional)

### Estructura de la Tabla
```sql
CREATE TABLE `usuario` (
  `IDUsuario` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(200) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `Pass` varchar(50) DEFAULT NULL,
  `EmailConfirmado` tinyint DEFAULT '0',
  `EmailConfirmadoToken` varchar(50) DEFAULT NULL,
  `EmailConfirmadoTokenExpira` datetime(3) DEFAULT NULL,
  `Telefono` varchar(20) DEFAULT NULL,
  `FechaCambioPass` datetime(3) DEFAULT NULL,
  `IntentosFallidos` int DEFAULT NULL,
  `Activo` tinyint DEFAULT NULL,
  `IDPerfil` int DEFAULT NULL,
  PRIMARY KEY (`IDUsuario`)
) ENGINE=InnoDB;
```

## Archivos Renombrados

| Archivo Original | Archivo Nuevo |
|-----------------|---------------|
| `User.cs` | `Usuario.cs` |
| `IUserRepository.cs` | `IUsuarioRepository.cs` |
| `IUserService.cs` | `IUsuarioService.cs` |
| `UserRepository.cs` | `UsuarioRepository.cs` |
| `UserService.cs` | `UsuarioService.cs` |
| `UsersController.cs` | `UsuariosController.cs` |
| `CreateUserRequest.cs` | `CreateUsuarioRequest.cs` |
| `UpdateUserRequest.cs` | `UpdateUsuarioRequest.cs` |
| `UserResponse.cs` | `UsuarioResponse.cs` |

## Nuevas Funcionalidades

1. **Autenticación de Usuarios**
   - Validación de credenciales (email/password)
   - Control de intentos fallidos

2. **Gestión de Contraseñas**
   - Cambio de contraseña con validación
   - Registro de fecha de cambio

3. **Confirmación de Email**
   - Token de confirmación
   - Fecha de expiración del token

4. **Gestión de Perfiles**
   - Soporte para IDPerfil
   - Múltiples perfiles de usuario

5. **Control de Acceso**
   - Activación/Desactivación de usuarios
   - Estado de email confirmado

## Estado de Compilación
✅ **COMPILACIÓN EXITOSA** - Todos los proyectos compilan sin errores

## Próximos Pasos Recomendados

1. **Seguridad:**
   - Implementar hashing de contraseñas (BCrypt, Argon2, etc.)
   - Agregar JWT o autenticación basada en tokens
   - Implementar validación de tokens de email

2. **Validaciones:**
   - Política de contraseñas fuertes
   - Límite de intentos fallidos antes de bloqueo
   - Validación de email único en la BD (constraint)

3. **Testing:**
   - Unit tests para servicios
   - Integration tests para repositorios
   - End-to-end tests para endpoints

4. **Documentación:**
   - Actualizar Swagger/OpenAPI con ejemplos
   - Documentar todos los endpoints nuevos

5. **Monitoreo:**
   - Logging de intentos fallidos de login
   - Auditoría de cambios de contraseña
   - Alertas de seguridad

## Notas Importantes

⚠️ **IMPORTANTE:** 
- Las contraseñas actualmente se almacenan en texto plano. Se debe implementar hashing inmediatamente.
- El campo `Pass` tiene un límite de 50 caracteres. Considerar aumentarlo si se implementa hashing (recomendado: 255).
- Validar la lógica de bloqueo de cuentas por intentos fallidos.
- Implementar expiración automática de tokens de confirmación de email.

## Contacto y Soporte
Para preguntas o problemas relacionados con esta migración, contactar al equipo de desarrollo.

---
**Migración completada exitosamente** ✅
