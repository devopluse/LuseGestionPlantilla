namespace LuseGestion.API.DTOs;

public class UsuarioResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmado { get; set; }
    public string? Telefono { get; set; }
    public DateTime? FechaCambioPass { get; set; }
    public int? IntentosFallidos { get; set; }
    public bool Activo { get; set; }
    public int? IDPerfil { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static UsuarioResponse FromDomainUsuario(LuseGestion.Domain.Entities.Usuario usuario)
    {
        return new UsuarioResponse
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            EmailConfirmado = usuario.EmailConfirmado,
            Telefono = usuario.Telefono,
            FechaCambioPass = usuario.FechaCambioPass,
            IntentosFallidos = usuario.IntentosFallidos,
            Activo = usuario.Activo,
            IDPerfil = usuario.IDPerfil,
            CreatedAt = usuario.CreatedAt,
            UpdatedAt = usuario.UpdatedAt
        };
    }
}
