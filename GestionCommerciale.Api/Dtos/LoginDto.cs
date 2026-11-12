using System.ComponentModel.DataAnnotations;

namespace GestionCommerciale.Api.Dtos;

public class LoginDto
{
    [Required(ErrorMessage = "L'identifiant est obligatoire.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
    public string Password { get; set; } = string.Empty;
}