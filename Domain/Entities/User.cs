using MyBackendApi.Domain.Primitives;

namespace MyBackendApi.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    // Constructor for EF Core
    private User() { }

    public static User Create(string email, string firstName, string lastName)
    {
        var user = new User
        {
            Email = email.ToLowerInvariant(),
            FirstName = firstName,
            LastName = lastName
        };

        return user;
    }

    public void Update(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
