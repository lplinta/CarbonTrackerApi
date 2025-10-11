using System.ComponentModel.DataAnnotations;

namespace CarbonTrackerApi.DTOs.Inputs;

public class LoginInput
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Password { get; set; } = string.Empty;

    public LoginInput() { }

    public LoginInput(string username, string password)
    {
        Username = username;
        Password = password;
    }
}