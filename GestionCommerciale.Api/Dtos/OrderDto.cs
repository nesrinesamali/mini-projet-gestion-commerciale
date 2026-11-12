namespace GestionCommerciale.Api.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalHt { get; set; }
    public decimal TotalTtc { get; set; }
    public List<OrderLineDto> Lines { get; set; } = new();
}