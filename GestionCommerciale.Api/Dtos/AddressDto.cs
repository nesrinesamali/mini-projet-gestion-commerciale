using System.ComponentModel.DataAnnotations;

namespace GestionCommerciale.Api.Dtos;

public class AddressDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La rue est obligatoire.")]
    [StringLength(200)]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ville est obligatoire.")]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le pays est obligatoire.")]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;
}