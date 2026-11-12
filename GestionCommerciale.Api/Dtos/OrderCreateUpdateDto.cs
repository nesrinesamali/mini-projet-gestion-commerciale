using System.ComponentModel.DataAnnotations;

namespace GestionCommerciale.Api.Dtos;

public class OrderCreateUpdateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Le client est obligatoire.")]
    public int ClientId { get; set; }

    [MinLength(1, ErrorMessage = "La commande doit contenir au moins une ligne.")]
    public List<OrderLineCreateDto> Lines { get; set; } = new();
}