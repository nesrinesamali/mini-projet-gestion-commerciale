namespace GestionCommerciale.Api.Models;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public decimal TotalHt { get; set; }
    public decimal TotalTtc { get; set; }

    public List<OrderLine> Lines { get; set; } = new();
}