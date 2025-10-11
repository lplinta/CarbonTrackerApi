using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Interfaces.Repositories;
using CarbonTrackerApi.Interfaces.Services;
using CarbonTrackerApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace CarbonTrackerApi.Services;

public class AuthService(
    IRepository<Usuario> userRepository,
    IPasswordHasher passwordHasher,
    IConfiguration configuration) : IAuthService
{
    public async Task<LoginOutput?> Authenticate(LoginInput loginInput)
    {
        var user = await userRepository.GetAsync(u => u.Username == loginInput.Username);

        if (user == null || !passwordHasher.VerifyPassword(loginInput.Password, user.PasswordHash))
            return null;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        };

        var jwtSecretKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurada.");
        var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer não configurado.");
        var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience não configurado.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new LoginOutput(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public async Task<Usuario?> Register(RegisterInput registerInput)
    {
        if (await userRepository.ExistsAsync(u => u.Username == registerInput.Username))
            return null;

        var newUser = new Usuario
        {
            Username = registerInput.Username,
            Email = registerInput.Email,
            PasswordHash = passwordHasher.HashPassword(registerInput.Password),
            Role = registerInput.Role
        };

        await userRepository.AddAsync(newUser);
        await userRepository.SaveChangesAsync();

        return newUser;
    }
}