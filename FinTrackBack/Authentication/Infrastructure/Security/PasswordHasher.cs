using FinTrackBack.Authentication.Application.Interfaces;

namespace FinTrackBack.Authentication.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // V-- AÑADE ESTE MÉTODO --V
    public bool VerifyPassword(string password, string passwordHash)
    {
        // BCrypt se encarga de comparar el texto plano con el hash
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}