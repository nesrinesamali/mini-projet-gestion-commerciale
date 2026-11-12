namespace GestionCommerciale.Api.Dtos;

public class ClientDto
{
    public int Id { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstNameOrCompany { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<AddressDto> Addresses { get; set; } = new();
}