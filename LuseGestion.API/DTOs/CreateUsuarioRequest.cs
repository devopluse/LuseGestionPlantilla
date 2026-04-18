using FluentValidation;

namespace LuseGestion.API.DTOs;

public class CreateUsuarioRequest
{
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int? IDPerfil { get; set; }
}

public class CreateUsuarioRequestValidator : AbstractValidator<CreateUsuarioRequest>
{
    public CreateUsuarioRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("Formato de email inválido")
            .MaximumLength(50).WithMessage("El email no puede exceder 50 caracteres");

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MaximumLength(50).WithMessage("La contraseña no puede exceder 50 caracteres");

        RuleFor(x => x.Telefono)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefono));
    }
}
