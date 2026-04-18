# Implementación de Seguridad Recomendada para Usuario

## 1. Hashing de Contraseñas con BCrypt

### Instalación del Paquete
```bash
dotnet add package BCrypt.Net-Next
```

### Implementación en la Entidad Usuario

```csharp
// En Usuario.cs - Agregar método estático
public static string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
}

public static bool VerifyPassword(string password, string hash)
{
    return BCrypt.Net.BCrypt.Verify(password, hash);
}

// Modificar el método Create
public static Usuario Create(string email, string nombre, string password, string? telefono = null, int? idPerfil = null)
{
    var usuario = new Usuario
    {
        Email = email.ToLowerInvariant(),
        Nombre = nombre,
        Pass = HashPassword(password), // Hash la contraseña aquí
        Telefono = telefono,
        IDPerfil = idPerfil,
        EmailConfirmado = false,
        IntentosFallidos = 0,
        Activo = true
    };

    return usuario;
}

// Modificar UpdatePassword
public void UpdatePassword(string newPassword)
{
    Pass = HashPassword(newPassword); // Hash la nueva contraseña
    FechaCambioPass = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
}
```

### Actualizar Script SQL
```sql
-- Modificar la tabla para soportar contraseñas hasheadas
ALTER TABLE `usuario` 
MODIFY COLUMN `Pass` VARCHAR(255) DEFAULT NULL;
```

---

## 2. Implementación de JWT para Autenticación

### Instalación de Paquetes
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt
```

### Configuración en appsettings.json
```json
{
  "JwtSettings": {
    "SecretKey": "TU_CLAVE_SECRETA_MUY_LARGA_Y_SEGURA_DE_AL_MENOS_32_CARACTERES",
    "Issuer": "LuseGestion",
    "Audience": "LuseGestionClients",
    "ExpirationMinutes": 60
  }
}
```

### Servicio de Token (Crear nuevo archivo)

```csharp
// Infrastructure/Services/JwtTokenService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LuseGestion.Infrastructure.Services;

public interface IJwtTokenService
{
    string GenerateToken(int usuarioId, string email, string nombre, int? idPerfil);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(int usuarioId, string email, string nombre, int? idPerfil)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("nombre", nombre),
            new Claim("idPerfil", idPerfil?.ToString() ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["JwtSettings:ExpirationMinutes"]!)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### Configuración en Program.cs

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Después de builder.Services.AddControllers();

// Configurar JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();

// Registrar el servicio de tokens
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Antes de app.Run();
app.UseAuthentication();
app.UseAuthorization();
```

### Actualizar el Controller para usar JWT

```csharp
// En UsuariosController.cs
private readonly IJwtTokenService _jwtTokenService;

public UsuariosController(IUsuarioService usuarioService, IJwtTokenService jwtTokenService)
{
    _usuarioService = usuarioService;
    _jwtTokenService = jwtTokenService;
}

[HttpPost("authenticate")]
public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
{
    var result = await _usuarioService.AuthenticateAsync(request.Email, request.Password);

    if (result.IsFailure)
    {
        return Unauthorized(new { error = result.Error });
    }

    var usuario = result.Value;
    var token = _jwtTokenService.GenerateToken(
        usuario.Id, 
        usuario.Email, 
        usuario.Nombre,
        usuario.IDPerfil);

    return Ok(new 
    { 
        token = token,
        usuario = UsuarioResponse.FromDomainUsuario(usuario)
    });
}

// Proteger endpoints con [Authorize]
[Authorize]
[HttpPut("{id:int}")]
public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UpdateUsuarioRequest request)
{
    // ... código existente
}
```

### Actualizar UsuarioService.AuthenticateAsync

```csharp
public async Task<Result<Usuario>> AuthenticateAsync(string email, string password)
{
    try
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<Usuario>.Failure("El email es requerido");

        if (string.IsNullOrWhiteSpace(password))
            return Result<Usuario>.Failure("La contraseña es requerida");

        // Obtener usuario por email
        var result = await _usuarioRepository.GetByEmailAsync(email);

        if (result.IsFailure)
            return Result<Usuario>.Failure("Credenciales inválidas");

        var usuario = result.Value;

        // Verificar si está activo
        if (!usuario.Activo)
            return Result<Usuario>.Failure("Usuario desactivado");

        // Verificar contraseña con BCrypt
        if (!Usuario.VerifyPassword(password, usuario.Pass))
        {
            usuario.IncrementarIntentosFallidos();
            await _usuarioRepository.UpdateAsync(usuario);

            // Bloquear después de 5 intentos
            if (usuario.IntentosFallidos >= 5)
            {
                usuario.Desactivar();
                await _usuarioRepository.UpdateAsync(usuario);
                return Result<Usuario>.Failure("Cuenta bloqueada por múltiples intentos fallidos");
            }

            return Result<Usuario>.Failure("Credenciales inválidas");
        }

        // Reset intentos fallidos en login exitoso
        usuario.ResetearIntentosFallidos();
        await _usuarioRepository.UpdateAsync(usuario);

        return Result<Usuario>.Success(usuario);
    }
    catch (Exception ex)
    {
        return Result<Usuario>.Failure($"Error de autenticación: {ex.Message}");
    }
}
```

### Actualizar GetByEmailAndPasswordAsync en el Repositorio

```csharp
// Este método ya no es necesario, eliminar o marcar como obsoleto
// La verificación de contraseña se hace en el servicio con BCrypt
```

---

## 3. Confirmación de Email

### Servicio de Email (Crear nuevo)

```csharp
// Infrastructure/Services/EmailService.cs
namespace LuseGestion.Infrastructure.Services;

public interface IEmailService
{
    Task<bool> SendEmailConfirmationAsync(string email, string token, string nombre);
}

public class EmailService : IEmailService
{
    // Implementar con SendGrid, SMTP, etc.
    public async Task<bool> SendEmailConfirmationAsync(string email, string token, string nombre)
    {
        // TODO: Implementar envío de email
        await Task.CompletedTask;
        return true;
    }
}
```

### Agregar métodos al IUsuarioService

```csharp
Task<Result> SendEmailConfirmationAsync(int usuarioId);
Task<Result> ConfirmEmailAsync(string token);
```

### Implementación en UsuarioService

```csharp
public async Task<Result> SendEmailConfirmationAsync(int usuarioId)
{
    var getUserResult = await _usuarioRepository.GetByIdAsync(usuarioId);
    if (getUserResult.IsFailure)
        return Result.Failure(getUserResult.Error);

    var usuario = getUserResult.Value;

    var token = Guid.NewGuid().ToString();
    var expira = DateTime.UtcNow.AddHours(24);

    usuario.SetEmailConfirmationToken(token, expira);
    await _usuarioRepository.UpdateAsync(usuario);

    // Enviar email con el token
    await _emailService.SendEmailConfirmationAsync(usuario.Email, token, usuario.Nombre);

    return Result.Success();
}

public async Task<Result> ConfirmEmailAsync(string token)
{
    // Buscar usuario por token (agregar método al repositorio)
    // Verificar que no haya expirado
    // Confirmar email
    // TODO: Implementar
    return Result.Success();
}
```

---

## 4. Rate Limiting

### Instalación
```bash
dotnet add package AspNetCoreRateLimit
```

### Configuración en Program.cs

```csharp
using AspNetCoreRateLimit;

// Configurar rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// En el pipeline
app.UseIpRateLimiting();
```

### Configuración en appsettings.json

```json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*/api/Usuarios/authenticate",
        "Period": "1m",
        "Limit": 5
      },
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 30
      }
    ]
  }
}
```

---

## 5. CORS Seguro

### Actualizar en Program.cs

```csharp
// Reemplazar la configuración CORS existente
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins(
                "https://tusitio.com",
                "https://www.tusitio.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // Solo para desarrollo
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("DevelopmentPolicy", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    }
});

// Usar la política apropiada
app.UseCors(builder.Environment.IsDevelopment() ? "DevelopmentPolicy" : "ProductionPolicy");
```

---

## 6. Validación de Fortaleza de Contraseña

### Agregar al CreateUsuarioRequest

```csharp
public class CreateUsuarioRequestValidator : AbstractValidator<CreateUsuarioRequest>
{
    public CreateUsuarioRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número")
            .Matches(@"[!@#$%^&*(),.?""':{}|<>]").WithMessage("La contraseña debe contener al menos un carácter especial");
    }
}
```

---

## 7. Logging de Seguridad

### Agregar a UsuarioService

```csharp
private readonly ILogger<UsuarioService> _logger;

public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger)
{
    _usuarioRepository = usuarioRepository;
    _logger = logger;
}

public async Task<Result<Usuario>> AuthenticateAsync(string email, string password)
{
    _logger.LogInformation("Intento de autenticación para el email: {Email}", email);

    // ... código de autenticación

    if (!Usuario.VerifyPassword(password, usuario.Pass))
    {
        _logger.LogWarning("Intento de login fallido para el email: {Email}. Intentos: {Intentos}", 
            email, usuario.IntentosFallidos + 1);
        // ...
    }

    _logger.LogInformation("Autenticación exitosa para el usuario: {UsuarioId}", usuario.Id);
    return Result<Usuario>.Success(usuario);
}
```

---

## Checklist de Implementación

- [ ] Instalar BCrypt.Net-Next
- [ ] Modificar columna `Pass` en BD a VARCHAR(255)
- [ ] Implementar hashing en métodos Create y UpdatePassword
- [ ] Actualizar método de autenticación para usar BCrypt
- [ ] Instalar paquetes JWT
- [ ] Crear JwtTokenService
- [ ] Configurar JWT en Program.cs
- [ ] Actualizar endpoint authenticate para devolver JWT
- [ ] Agregar [Authorize] a endpoints protegidos
- [ ] Implementar validación de fortaleza de contraseña
- [ ] Implementar rate limiting
- [ ] Configurar CORS seguro
- [ ] Implementar logging de seguridad
- [ ] Implementar servicio de email
- [ ] Agregar confirmación de email
- [ ] Agregar manejo de intentos fallidos y bloqueo de cuenta
- [ ] Documentar nuevos endpoints en Swagger
- [ ] Actualizar tests

---

## Testing de Seguridad

### Test de Autenticación con JWT
```bash
# Login
TOKEN=$(curl -X POST https://localhost:5001/api/Usuarios/authenticate \
  -H "Content-Type: application/json" \
  -d '{"email":"usuario@ejemplo.com","password":"Pass123"}' \
  | jq -r '.token')

# Usar el token
curl -X GET https://localhost:5001/api/Usuarios/1 \
  -H "Authorization: Bearer $TOKEN"
```

---

## Recursos Adicionales

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [BCrypt Documentation](https://github.com/BcryptNet/bcrypt.net)
- [JWT Best Practices](https://jwt.io/introduction)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)

---

**IMPORTANTE:** Implementar TODAS estas medidas de seguridad antes de desplegar a producción.
