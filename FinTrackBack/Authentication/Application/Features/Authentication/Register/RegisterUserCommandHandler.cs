using FinTrackBack.Authentication.Application.Interfaces;
using FinTrackBack.Authentication.Domain.Entities;
using FinTrackBack.Authentication.Infrastructure.Persistence.DbContext;
using MediatR;

namespace FinTrackBack.Authentication.Application.Features.Authentication.Register;

// Implementa IRequestHandler<Comando, Respuesta>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly FinTrackBackDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    // Pedimos al constructor las herramientas que necesitamos (Inyección de Dependencias)
    public RegisterUserCommandHandler(FinTrackBackDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Hashear la contraseña
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        
        // 2. Crear la entidad de dominio
        var user = new User
        {
            Id = Guid.NewGuid(), // Crea un nuevo ID único
            Nombre = request.Nombre,
            Email = request.Email,
            PasswordHash = passwordHash,
            Premium = false // Como en tu db.json, los nuevos usuarios no son premium
        };
        
        // 3. Guardar en la base de datos
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        // 4. Devolver el ID del nuevo usuario
        return user.Id;
    }
}