using System.ComponentModel.DataAnnotations;

namespace GestionCommerciale.Api.Dtos;

public class OrderLineCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Le produit est obligatoire.")]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être supérieure à 0.")]
    public int Quantity { get; set; }
}