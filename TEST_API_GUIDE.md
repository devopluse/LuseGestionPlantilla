# 🧪 Pruebas Rápidas de la API - Usuario

Este archivo contiene comandos para probar rápidamente todos los endpoints de la API de Usuarios.

## ⚙️ Configuración Previa

1. Asegúrate de que la API esté corriendo
2. Asegúrate de que la base de datos MySQL esté corriendo
3. Ejecuta el script `Database/CreateUsuarioTable.sql` si no lo has hecho

```bash
# Iniciar la API
cd LuseGestion.API
dotnet run
```

La API debería estar disponible en: `https://localhost:{puerto}/`

---

## 🌐 URL Base

```bash
# Configura la URL base según tu puerto
$BASE_URL = "https://localhost:5001"
```

---

## 🧪 Pruebas PowerShell

### 1. Crear un Usuario

```powershell
$createBody = @{
    email = "test@ejemplo.com"
    nombre = "Usuario de Prueba"
    password = "TestPassword123"
    telefono = "555-1234"
    idPerfil = 1
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios" `
    -Method POST `
    -Body $createBody `
    -ContentType "application/json" `
    -SkipCertificateCheck

Write-Host "✅ Usuario creado con ID: $($response.id)" -ForegroundColor Green
$response | ConvertTo-Json
```

### 2. Obtener Usuario por ID

```powershell
$usuarioId = 1
$response = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId" `
    -Method GET `
    -SkipCertificateCheck

Write-Host "✅ Usuario obtenido:" -ForegroundColor Green
$response | ConvertTo-Json
```

### 3. Obtener Usuario por Email

```powershell
$email = "test@ejemplo.com"
$response = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/by-email/$email" `
    -Method GET `
    -SkipCertificateCheck

Write-Host "✅ Usuario encontrado por email:" -ForegroundColor Green
$response | ConvertTo-Json
```

### 4. Obtener Todos los Usuarios

```powershell
$response = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios" `
    -Method GET `
    -SkipCertificateCheck

Write-Host "✅ Total de usuarios: $($response.Count)" -ForegroundColor Green
$response | ConvertTo-Json
```

### 5. Actualizar Usuario

```powershell
$usuarioId = 1
$updateBody = @{
    nombre = "Usuario Actualizado"
    telefono = "555-9999"
    idPerfil = 2
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId" `
    -Method PUT `
    -Body $updateBody `
    -ContentType "application/json" `
    -SkipCertificateCheck

Write-Host "✅ Usuario actualizado:" -ForegroundColor Green
$response | ConvertTo-Json
```

### 6. Autenticar Usuario

```powershell
$authBody = @{
    email = "test@ejemplo.com"
    password = "TestPassword123"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/authenticate" `
    -Method POST `
    -Body $authBody `
    -ContentType "application/json" `
    -SkipCertificateCheck

Write-Host "✅ Autenticación exitosa:" -ForegroundColor Green
$response | ConvertTo-Json
```

### 7. Cambiar Contraseña

```powershell
$usuarioId = 1
$changePassBody = @{
    currentPassword = "TestPassword123"
    newPassword = "NewPassword456"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId/change-password" `
    -Method POST `
    -Body $changePassBody `
    -ContentType "application/json" `
    -SkipCertificateCheck

Write-Host "✅ Contraseña cambiada:" -ForegroundColor Green
$response | ConvertTo-Json
```

### 8. Desactivar Usuario

```powershell
$usuarioId = 1
$response = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId/deactivate" `
    -Method PUT `
    -SkipCertificateCheck

Write-Host "✅ Usuario desactivado:" -ForegroundColor Green
$response | ConvertTo-Json
```

### 9. Activar Usuario

```powershell
$usuarioId = 1
$response = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId/activate" `
    -Method PUT `
    -SkipCertificateCheck

Write-Host "✅ Usuario activado:" -ForegroundColor Green
$response | ConvertTo-Json
```

### 10. Eliminar Usuario (Soft Delete)

```powershell
$usuarioId = 1
try {
    Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId" `
        -Method DELETE `
        -SkipCertificateCheck
    Write-Host "✅ Usuario eliminado (soft delete)" -ForegroundColor Green
}
catch {
    Write-Host "❌ Error al eliminar: $($_.Exception.Message)" -ForegroundColor Red
}
```

---

## 🧪 Script Completo de Pruebas (PowerShell)

```powershell
# SCRIPT DE PRUEBAS COMPLETO
# Ejecutar todo de una vez

$BASE_URL = "https://localhost:5001"

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "🧪 INICIANDO PRUEBAS DE API - USUARIO" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Test 1: Crear Usuario
Write-Host "1️⃣  Creando usuario..." -ForegroundColor Yellow
$createBody = @{
    email = "prueba@ejemplo.com"
    nombre = "Usuario de Prueba"
    password = "TestPass123"
    telefono = "555-TEST"
    idPerfil = 1
} | ConvertTo-Json

try {
    $usuario = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios" `
        -Method POST `
        -Body $createBody `
        -ContentType "application/json" `
        -SkipCertificateCheck

    $usuarioId = $usuario.id
    Write-Host "   ✅ Usuario creado con ID: $usuarioId" -ForegroundColor Green
}
catch {
    Write-Host "   ⚠️  Error o usuario ya existe" -ForegroundColor Yellow
    # Si falla, intentar obtener por email
    $usuario = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/by-email/prueba@ejemplo.com" `
        -Method GET `
        -SkipCertificateCheck
    $usuarioId = $usuario.id
    Write-Host "   ℹ️  Usando usuario existente con ID: $usuarioId" -ForegroundColor Cyan
}

Start-Sleep -Seconds 1

# Test 2: Obtener por ID
Write-Host "`n2️⃣  Obteniendo usuario por ID..." -ForegroundColor Yellow
$usuario = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId" `
    -Method GET `
    -SkipCertificateCheck
Write-Host "   ✅ Usuario obtenido: $($usuario.nombre)" -ForegroundColor Green

Start-Sleep -Seconds 1

# Test 3: Obtener por Email
Write-Host "`n3️⃣  Obteniendo usuario por email..." -ForegroundColor Yellow
$usuario = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/by-email/prueba@ejemplo.com" `
    -Method GET `
    -SkipCertificateCheck
Write-Host "   ✅ Usuario encontrado: $($usuario.email)" -ForegroundColor Green

Start-Sleep -Seconds 1

# Test 4: Listar todos
Write-Host "`n4️⃣  Listando todos los usuarios..." -ForegroundColor Yellow
$usuarios = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios" `
    -Method GET `
    -SkipCertificateCheck
Write-Host "   ✅ Total de usuarios activos: $($usuarios.Count)" -ForegroundColor Green

Start-Sleep -Seconds 1

# Test 5: Actualizar
Write-Host "`n5️⃣  Actualizando usuario..." -ForegroundColor Yellow
$updateBody = @{
    nombre = "Usuario Actualizado"
    telefono = "555-9999"
    idPerfil = 2
} | ConvertTo-Json

$usuario = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId" `
    -Method PUT `
    -Body $updateBody `
    -ContentType "application/json" `
    -SkipCertificateCheck
Write-Host "   ✅ Usuario actualizado: $($usuario.nombre)" -ForegroundColor Green

Start-Sleep -Seconds 1

# Test 6: Autenticar
Write-Host "`n6️⃣  Autenticando usuario..." -ForegroundColor Yellow
$authBody = @{
    email = "prueba@ejemplo.com"
    password = "TestPass123"
} | ConvertTo-Json

try {
    $authResponse = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/authenticate" `
        -Method POST `
        -Body $authBody `
        -ContentType "application/json" `
        -SkipCertificateCheck
    Write-Host "   ✅ Autenticación exitosa" -ForegroundColor Green
}
catch {
    Write-Host "   ❌ Error en autenticación" -ForegroundColor Red
}

Start-Sleep -Seconds 1

# Test 7: Cambiar contraseña
Write-Host "`n7️⃣  Cambiando contraseña..." -ForegroundColor Yellow
$changePassBody = @{
    currentPassword = "TestPass123"
    newPassword = "NewPass456"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId/change-password" `
        -Method POST `
        -Body $changePassBody `
        -ContentType "application/json" `
        -SkipCertificateCheck
    Write-Host "   ✅ Contraseña cambiada" -ForegroundColor Green

    # Cambiar de vuelta para mantener consistencia
    $changeBackBody = @{
        currentPassword = "NewPass456"
        newPassword = "TestPass123"
    } | ConvertTo-Json

    Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId/change-password" `
        -Method POST `
        -Body $changeBackBody `
        -ContentType "application/json" `
        -SkipCertificateCheck | Out-Null
}
catch {
    Write-Host "   ⚠️  Error al cambiar contraseña" -ForegroundColor Yellow
}

Start-Sleep -Seconds 1

# Test 8: Desactivar
Write-Host "`n8️⃣  Desactivando usuario..." -ForegroundColor Yellow
$response = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId/deactivate" `
    -Method PUT `
    -SkipCertificateCheck
Write-Host "   ✅ Usuario desactivado" -ForegroundColor Green

Start-Sleep -Seconds 1

# Test 9: Activar
Write-Host "`n9️⃣  Activando usuario..." -ForegroundColor Yellow
$response = Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId/activate" `
    -Method PUT `
    -SkipCertificateCheck
Write-Host "   ✅ Usuario activado" -ForegroundColor Green

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "✅ TODAS LAS PRUEBAS COMPLETADAS" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan
```

---

## 🎯 Ejecutar Todas las Pruebas

Guarda el script completo en un archivo `Test-UsuarioAPI.ps1` y ejecútalo:

```powershell
# Dar permisos de ejecución (solo primera vez)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Ejecutar el script
.\Test-UsuarioAPI.ps1
```

---

## 📊 Resultados Esperados

Deberías ver algo como:

```
========================================
🧪 INICIANDO PRUEBAS DE API - USUARIO
========================================

1️⃣  Creando usuario...
   ✅ Usuario creado con ID: 159

2️⃣  Obteniendo usuario por ID...
   ✅ Usuario obtenido: Usuario de Prueba

3️⃣  Obteniendo usuario por email...
   ✅ Usuario encontrado: prueba@ejemplo.com

4️⃣  Listando todos los usuarios...
   ✅ Total de usuarios activos: 3

5️⃣  Actualizando usuario...
   ✅ Usuario actualizado: Usuario Actualizado

6️⃣  Autenticando usuario...
   ✅ Autenticación exitosa

7️⃣  Cambiando contraseña...
   ✅ Contraseña cambiada

8️⃣  Desactivando usuario...
   ✅ Usuario desactivado

9️⃣  Activando usuario...
   ✅ Usuario activado

========================================
✅ TODAS LAS PRUEBAS COMPLETADAS
========================================
```

---

## 🔍 Verificación en la Base de Datos

Después de ejecutar las pruebas, verifica en MySQL:

```sql
-- Ver todos los usuarios
SELECT IDUsuario, Nombre, Email, Activo, IntentosFallidos 
FROM usuario 
ORDER BY IDUsuario DESC 
LIMIT 10;

-- Ver el usuario de prueba
SELECT * FROM usuario WHERE Email = 'prueba@ejemplo.com';
```

---

## 🧹 Limpieza (Opcional)

Para eliminar el usuario de prueba:

```powershell
# Soft delete
$usuarioId = 159  # Cambiar por el ID real
Invoke-RestMethod -Uri "$BASE_URL/api/Usuarios/$usuarioId" `
    -Method DELETE `
    -SkipCertificateCheck
```

O en MySQL:

```sql
-- Soft delete
UPDATE usuario SET Activo = 0 WHERE Email = 'prueba@ejemplo.com';

-- Hard delete (no recomendado)
DELETE FROM usuario WHERE Email = 'prueba@ejemplo.com';
```

---

## ⚠️ Troubleshooting

### Error: "No se puede conectar a localhost"
- Verifica que la API esté corriendo
- Verifica el puerto correcto en `$BASE_URL`

### Error: "Certificate validation failed"
- Usa el parámetro `-SkipCertificateCheck` (ya incluido en los ejemplos)

### Error: "Email already exists"
- El usuario ya existe, usa otro email o elimina el existente

### Error: "Usuario no encontrado"
- Verifica que el ID sea correcto
- Verifica que el usuario esté activo

---

## 📝 Notas

- Las pruebas son **no destructivas** (excepto el DELETE que es soft)
- Puedes ejecutar el script múltiples veces
- Los IDs se generan automáticamente (AUTO_INCREMENT)
- Las contraseñas están en **texto plano** (implementar hashing en producción)

---

## 🎓 Próximo Paso

Después de verificar que todo funciona, implementa las medidas de seguridad descritas en:
- `SECURITY_IMPLEMENTATION.md`

---

*Happy Testing! 🧪✨*
