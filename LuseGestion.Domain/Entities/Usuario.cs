using LuseGestion.Domain.Primitives;

namespace LuseGestion.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nombre { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Pass { get; private set; } = string.Empty;
    public bool EmailConfirmado { get; private set; } = false;
    public string? EmailConfirmadoToken { get; private set; }
    public DateTime? EmailConfirmadoTokenExpira { get; private set; }
    public string? Telefono { get; private set; }
    public DateTime? FechaCambioPass { get; private set; }
    public int? IntentosFallidos { get; private set; }
    public bool Activo { get; private set; } = true;
    public int? IDPerfil { get; private set; }

    // Constructor for Dapper
    private Usuario() { }

    public static Usuario Create(string email, string nombre, string pass, string? telefono = null, int? idPerfil = null)
    {
        var usuario = new Usuario
        {
            Email = email.ToLowerInvariant(),
            Nombre = nombre,
            Pass = pass,
            Telefono = telefono,
            IDPerfil = idPerfil,
            EmailConfirmado = false,
            IntentosFallidos = 0,
            Activo = true
        };

        return usuario;
    }

    public void Update(string nombre, string? telefono = null, int? idPerfil = null)
    {
        Nombre = nombre;
        Telefono = telefono;
        IDPerfil = idPerfil;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        Email = email.ToLowerInvariant();
        EmailConfirmado = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string newPass)
    {
        Pass = newPass;
        FechaCambioPass = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEmailConfirmationToken(string token, DateTime expira)
    {
        EmailConfirmadoToken = token;
        EmailConfirmadoTokenExpira = expira;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmEmail()
    {
        EmailConfirmado = true;
        EmailConfirmadoToken = null;
        EmailConfirmadoTokenExpira = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementarIntentosFallidos()
    {
        IntentosFallidos = (IntentosFallidos ?? 0) + 1;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ResetearIntentosFallidos()
    {
        IntentosFallidos = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Desactivar()
    {
        Activo = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activar()
    {
        Activo = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
