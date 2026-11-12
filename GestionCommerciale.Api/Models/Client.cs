namespace GestionCommerciale.Api.Models;

public class Client
{
    public int Id { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstNameOrCompany { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ClientAddress> Addresses { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
}