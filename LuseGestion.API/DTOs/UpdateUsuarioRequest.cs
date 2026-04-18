using FluentValidation;

namespace LuseGestion.API.DTOs;

public class UpdateUsuarioRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int? IDPerfil { get; set; }
}

public class UpdateUsuarioRequestValidator : AbstractValidator<UpdateUsuarioRequest>
{
    public UpdateUsuarioRequestValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.Telefono)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefono));
    }
}
