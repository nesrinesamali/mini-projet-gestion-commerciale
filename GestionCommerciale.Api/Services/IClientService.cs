using GestionCommerciale.Api.Dtos;

namespace GestionCommerciale.Api.Services;

public interface IClientService
{
    Task<List<ClientDto>> GetAllAsync();
    Task<ClientDto> GetByIdAsync(int id);
    Task<ClientDto> CreateAsync(ClientCreateUpdateDto dto);
    Task<ClientDto> UpdateAsync(int id, ClientCreateUpdateDto dto);
    Task DeleteAsync(int id);
}