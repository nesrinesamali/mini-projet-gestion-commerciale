using GestionCommerciale.Api.Dtos;

namespace GestionCommerciale.Api.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(ProductCreateUpdateDto dto);
    Task<ProductDto> UpdateAsync(int id, ProductCreateUpdateDto dto);
    Task DeleteAsync(int id);
}