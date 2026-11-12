using System.ComponentModel.DataAnnotations;

namespace GestionCommerciale.Api.Dtos;

public class ClientCreateUpdateDto
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom ou la raison sociale est obligatoire.")]
    [StringLength(150)]
    public string FirstNameOrCompany { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email est obligatoire.")]
    [EmailAddress(ErrorMessage = "Email invalide.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Numéro de téléphone invalide.")]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    public List<AddressDto> Addresses { get; set; } = new();
}