using GestionCommerciale.Api.Data;
using GestionCommerciale.Api.Dtos;
using GestionCommerciale.Api.Exceptions;
using GestionCommerciale.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionCommerciale.Api.Services;

public class ClientService : IClientService
{
    private readonly AppDbContext _db;

    public ClientService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ClientDto>> GetAllAsync()
    {
        var clients = await _db.Clients
            .AsNoTracking()
            .Include(c => c.Addresses)
            .OrderBy(c => c.LastName)
            .ToListAsync();

        return clients.Select(ToDto).ToList();
    }

    public async Task<ClientDto> GetByIdAsync(int id)
    {
        var client = await GetEntityAsync(id);
        return ToDto(client);
    }

    public async Task<ClientDto> CreateAsync(ClientCreateUpdateDto dto)
    {
        var client = new Client
        {
            LastName = dto.LastName.Trim(),
            FirstNameOrCompany = dto.FirstNameOrCompany.Trim(),
            Email = dto.Email.Trim(),
            Phone = dto.Phone.Trim(),
            Addresses = dto.Addresses.Select(ToAddress).ToList()
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return ToDto(client);
    }

    public async Task<ClientDto> UpdateAsync(int id, ClientCreateUpdateDto dto)
    {
        var client = await GetEntityAsync(id);

        client.LastName = dto.LastName.Trim();
        client.FirstNameOrCompany = dto.FirstNameOrCompany.Trim();
        client.Email = dto.Email.Trim();
        client.Phone = dto.Phone.Trim();

        client.Addresses.Clear();
        foreach (var address in dto.Addresses)
        {
            client.Addresses.Add(ToAddress(address));
        }

        await _db.SaveChangesAsync();
        return ToDto(client);
    }

    public async Task DeleteAsync(int id)
    {
        var client = await GetEntityAsync(id);

        var hasOrders = await _db.Orders.AnyAsync(o => o.ClientId == id);
        if (hasOrders)
        {
            throw new BusinessRuleException("Impossible de supprimer un client qui a des commandes.");
        }

        _db.Clients.Remove(client);
        await _db.SaveChangesAsync();
    }

    private async Task<Client> GetEntityAsync(int id)
    {
        var client = await _db.Clients
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == id);

        return client ?? throw new NotFoundException($"Client {id} introuvable.");
    }

    private static ClientAddress ToAddress(AddressDto dto) => new()
    {
        Street = dto.Street.Trim(),
        City = dto.City.Trim(),
        PostalCode = dto.PostalCode.Trim(),
        Country = dto.Country.Trim()
    };

    private static ClientDto ToDto(Client client) => new()
    {
        Id = client.Id,
        LastName = client.LastName,
        FirstNameOrCompany = client.FirstNameOrCompany,
        Email = client.Email,
        Phone = client.Phone,
        CreatedAt = client.CreatedAt,
        Addresses = client.Addresses.Select(a => new AddressDto
        {
            Id = a.Id,
            Street = a.Street,
            City = a.City,
            PostalCode = a.PostalCode,
            Country = a.Country
        }).ToList()
    };
}