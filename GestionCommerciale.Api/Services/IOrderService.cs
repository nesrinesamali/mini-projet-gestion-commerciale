using GestionCommerciale.Api.Dtos;

namespace GestionCommerciale.Api.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync();
    Task<OrderDto> GetByIdAsync(int id);
    Task<OrderDto> CreateAsync(OrderCreateUpdateDto dto);
    Task<OrderDto> UpdateAsync(int id, OrderCreateUpdateDto dto);
    Task DeleteAsync(int id);
    Task<OrderDto> ValidateAsync(int id);
}