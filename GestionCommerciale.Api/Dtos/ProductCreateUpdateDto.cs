using System.ComponentModel.DataAnnotations;

namespace GestionCommerciale.Api.Dtos;

public class ProductCreateUpdateDto
{
    [Required(ErrorMessage = "La référence est obligatoire.")]
    [StringLength(50)]
    public string Reference { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom du produit est obligatoire.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "Le prix unitaire HT doit être supérieur à 0.")]
    public decimal UnitPriceHt { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Le stock ne peut pas être négatif.")]
    public int StockQuantity { get; set; }
}