# Guía de Uso - API de Usuarios (Usuario)

## Base URL
```
https://localhost:{puerto}/api/Usuarios
```

---

## Endpoints Disponibles

### 1. Crear Usuario
**POST** `/api/Usuarios`

**Request Body:**
```json
{
  "email": "usuario@ejemplo.com",
  "nombre": "Juan Pérez",
  "password": "MiPassword123",
  "telefono": "555-1234",
  "idPerfil": 1
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "nombre": "Juan Pérez",
  "email": "usuario@ejemplo.com",
  "emailConfirmado": false,
  "telefono": "555-1234",
  "fechaCambioPass": null,
  "intentosFallidos": 0,
  "activo": true,
  "idPerfil": 1,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

**Errores Posibles:**
- 400 Bad Request: Email ya existe, validaciones fallidas
- 500 Internal Server Error: Error de base de datos

---

### 2. Obtener Usuario por ID
**GET** `/api/Usuarios/{id}`

**Ejemplo:**
```
GET /api/Usuarios/1
```

**Response (200 OK):**
```json
{
  "id": 1,
  "nombre": "Juan Pérez",
  "email": "usuario@ejemplo.com",
  "emailConfirmado": true,
  "telefono": "555-1234",
  "fechaCambioPass": "2024-01-10T08:00:00Z",
  "intentosFallidos": 0,
  "activo": true,
  "idPerfil": 1,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-10T08:00:00Z"
}
```

**Errores Posibles:**
- 404 Not Found: Usuario no encontrado

---

### 3. Obtener Usuario por Email
**GET** `/api/Usuarios/by-email/{email}`

**Ejemplo:**
```
GET /api/Usuarios/by-email/usuario@ejemplo.com
```

**Response (200 OK):** (igual que el endpoint por ID)

---

### 4. Obtener Todos los Usuarios
**GET** `/api/Usuarios`

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "nombre": "Juan Pérez",
    "email": "usuario1@ejemplo.com",
    "emailConfirmado": true,
    "telefono": "555-1234",
    "fechaCambioPass": null,
    "intentosFallidos": 0,
    "activo": true,
    "idPerfil": 1,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": null
  },
  {
    "id": 2,
    "nombre": "María García",
    "email": "usuario2@ejemplo.com",
    "emailConfirmado": false,
    "telefono": "555-5678",
    "fechaCambioPass": null,
    "intentosFallidos": 0,
    "activo": true,
    "idPerfil": 2,
    "createdAt": "2024-01-02T00:00:00Z",
    "updatedAt": null
  }
]
```

**Nota:** Solo devuelve usuarios activos (Activo = 1)

---

### 5. Actualizar Usuario
**PUT** `/api/Usuarios/{id}`

**Request Body:**
```json
{
  "nombre": "Juan Pérez Actualizado",
  "telefono": "555-9999",
  "idPerfil": 2
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "nombre": "Juan Pérez Actualizado",
  "email": "usuario@ejemplo.com",
  "emailConfirmado": true,
  "telefono": "555-9999",
  "fechaCambioPass": null,
  "intentosFallidos": 0,
  "activo": true,
  "idPerfil": 2,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-15T14:30:00Z"
}
```

**Errores Posibles:**
- 404 Not Found: Usuario no encontrado
- 400 Bad Request: Validaciones fallidas

---

### 6. Eliminar Usuario (Soft Delete)
**DELETE** `/api/Usuarios/{id}`

**Response (204 No Content):** Sin cuerpo de respuesta

**Nota:** No elimina el registro, solo cambia `Activo` a 0

**Errores Posibles:**
- 404 Not Found: Usuario no encontrado

---

### 7. Autenticar Usuario (Login)
**POST** `/api/Usuarios/authenticate`

**Request Body:**
```json
{
  "email": "usuario@ejemplo.com",
  "password": "MiPassword123"
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "nombre": "Juan Pérez",
  "email": "usuario@ejemplo.com",
  "emailConfirmado": true,
  "telefono": "555-1234",
  "fechaCambioPass": null,
  "intentosFallidos": 0,
  "activo": true,
  "idPerfil": 1,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-15T15:00:00Z"
}
```

**Errores Posibles:**
- 401 Unauthorized: Credenciales inválidas

⚠️ **IMPORTANTE:** Este endpoint devuelve el usuario completo. En producción, debería devolver un JWT token.

---

### 8. Cambiar Contraseña
**POST** `/api/Usuarios/{id}/change-password`

**Request Body:**
```json
{
  "currentPassword": "MiPassword123",
  "newPassword": "NuevoPassword456"
}
```

**Response (200 OK):**
```json
{
  "message": "Contraseña actualizada exitosamente"
}
```

**Errores Posibles:**
- 400 Bad Request: Contraseña actual incorrecta, validaciones fallidas
- 404 Not Found: Usuario no encontrado

---

### 9. Activar Usuario
**PUT** `/api/Usuarios/{id}/activate`

**Response (200 OK):**
```json
{
  "message": "Usuario activado exitosamente"
}
```

**Errores Posibles:**
- 400 Bad Request: Error al activar
- 404 Not Found: Usuario no encontrado

---

### 10. Desactivar Usuario
**PUT** `/api/Usuarios/{id}/deactivate`

**Response (200 OK):**
```json
{
  "message": "Usuario desactivado exitosamente"
}
```

**Errores Posibles:**
- 400 Bad Request: Error al desactivar
- 404 Not Found: Usuario no encontrado

---

## Ejemplos con cURL

### Crear Usuario
```bash
curl -X POST https://localhost:5001/api/Usuarios \
  -H "Content-Type: application/json" \
  -d '{
    "email": "nuevo@ejemplo.com",
    "nombre": "Usuario Nuevo",
    "password": "Pass123",
    "telefono": "555-0000",
    "idPerfil": 1
  }'
```

### Obtener Usuario
```bash
curl -X GET https://localhost:5001/api/Usuarios/1
```

### Autenticar
```bash
curl -X POST https://localhost:5001/api/Usuarios/authenticate \
  -H "Content-Type: application/json" \
  -d '{
    "email": "usuario@ejemplo.com",
    "password": "MiPassword123"
  }'
```

### Actualizar Usuario
```bash
curl -X PUT https://localhost:5001/api/Usuarios/1 \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Nombre Actualizado",
    "telefono": "555-9999",
    "idPerfil": 2
  }'
```

### Cambiar Contraseña
```bash
curl -X POST https://localhost:5001/api/Usuarios/1/change-password \
  -H "Content-Type: application/json" \
  -d '{
    "currentPassword": "Pass123",
    "newPassword": "NewPass456"
  }'
```

---

## Ejemplos con JavaScript (Fetch)

### Crear Usuario
```javascript
const crearUsuario = async () => {
  const response = await fetch('https://localhost:5001/api/Usuarios', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      email: 'nuevo@ejemplo.com',
      nombre: 'Usuario Nuevo',
      password: 'Pass123',
      telefono: '555-0000',
      idPerfil: 1
    })
  });

  const data = await response.json();
  console.log(data);
};
```

### Autenticar Usuario
```javascript
const autenticar = async (email, password) => {
  const response = await fetch('https://localhost:5001/api/Usuarios/authenticate', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ email, password })
  });

  if (response.ok) {
    const usuario = await response.json();
    console.log('Login exitoso:', usuario);
    return usuario;
  } else {
    console.error('Login fallido');
    return null;
  }
};
```

### Obtener Todos los Usuarios
```javascript
const obtenerUsuarios = async () => {
  const response = await fetch('https://localhost:5001/api/Usuarios');
  const usuarios = await response.json();
  console.log(usuarios);
};
```

---

## Validaciones de Request

### CreateUsuarioRequest
- **Email:** Requerido, formato válido, máximo 50 caracteres
- **Nombre:** Requerido, máximo 200 caracteres
- **Password:** Requerido, máximo 50 caracteres
- **Telefono:** Opcional, máximo 20 caracteres
- **IDPerfil:** Opcional, número entero

### UpdateUsuarioRequest
- **Nombre:** Requerido, máximo 200 caracteres
- **Telefono:** Opcional, máximo 20 caracteres
- **IDPerfil:** Opcional, número entero

### ChangePasswordRequest
- **CurrentPassword:** Requerido
- **NewPassword:** Requerido

---

## Códigos de Estado HTTP

- **200 OK:** Operación exitosa
- **201 Created:** Recurso creado exitosamente
- **204 No Content:** Operación exitosa sin contenido de respuesta
- **400 Bad Request:** Error en la validación o datos inválidos
- **401 Unauthorized:** Credenciales inválidas
- **404 Not Found:** Recurso no encontrado
- **500 Internal Server Error:** Error interno del servidor

---

## Notas de Seguridad

⚠️ **ADVERTENCIAS IMPORTANTES:**

1. **Contraseñas en Texto Plano:** Actualmente las contraseñas se almacenan sin hash. **IMPLEMENTAR HASHING INMEDIATAMENTE en producción** (BCrypt, Argon2, etc.)

2. **Sin Autenticación:** Los endpoints no requieren autenticación. **IMPLEMENTAR JWT o similar** para proteger los endpoints.

3. **HTTPS Obligatorio:** Siempre usar HTTPS en producción para proteger las credenciales.

4. **Rate Limiting:** Implementar límites de peticiones para prevenir ataques de fuerza bruta en el login.

5. **Validación de Email:** Implementar el flujo completo de confirmación de email con tokens.

---

## Swagger UI

Para una documentación interactiva y pruebas, accede a:
```
https://localhost:{puerto}/
```

Swagger UI te permitirá:
- Ver todos los endpoints disponibles
- Probar los endpoints directamente desde el navegador
- Ver los modelos de datos y validaciones
- Descargar la especificación OpenAPI

---

## Soporte

Para más información o reportar problemas, contacta al equipo de desarrollo.
