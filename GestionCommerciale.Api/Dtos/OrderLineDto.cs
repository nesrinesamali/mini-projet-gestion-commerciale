namespace GestionCommerciale.Api.Dtos;

public class OrderLineDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductReference { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}