using GestionCommerciale.Api.Data;
using GestionCommerciale.Api.Dtos;
using GestionCommerciale.Api.Exceptions;
using GestionCommerciale.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionCommerciale.Api.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _db.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync();

        return products.Select(ToDto).ToList();
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        var product = await GetEntityAsync(id);
        return ToDto(product);
    }

    public async Task<ProductDto> CreateAsync(ProductCreateUpdateDto dto)
    {
        await EnsureReferenceIsUniqueAsync(dto.Reference, null);

        var product = new Product
        {
            Reference = dto.Reference.Trim(),
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            UnitPriceHt = dto.UnitPriceHt,
            StockQuantity = dto.StockQuantity
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return ToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(int id, ProductCreateUpdateDto dto)
    {
        var product = await GetEntityAsync(id);
        await EnsureReferenceIsUniqueAsync(dto.Reference, id);

        product.Reference = dto.Reference.Trim();
        product.Name = dto.Name.Trim();
        product.Description = dto.Description?.Trim();
        product.UnitPriceHt = dto.UnitPriceHt;
        product.StockQuantity = dto.StockQuantity;

        await _db.SaveChangesAsync();
        return ToDto(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await GetEntityAsync(id);

        var isUsed = await _db.OrderLines.AnyAsync(l => l.ProductId == id);
        if (isUsed)
        {
            throw new BusinessRuleException("Impossible de supprimer un produit présent dans une commande.");
        }

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
    }

    private async Task<Product> GetEntityAsync(int id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        return product ?? throw new NotFoundException($"Produit {id} introuvable.");
    }

    private async Task EnsureReferenceIsUniqueAsync(string reference, int? currentId)
    {
        var cleanReference = reference.Trim();
        var exists = await _db.Products
            .AnyAsync(p => p.Reference == cleanReference && p.Id != currentId);

        if (exists)
        {
            throw new BusinessRuleException($"La référence '{cleanReference}' existe déjà.");
        }
    }

    private static ProductDto ToDto(Product product) => new()
    {
        Id = product.Id,
        Reference = product.Reference,
        Name = product.Name,
        Description = product.Description,
        UnitPriceHt = product.UnitPriceHt,
        StockQuantity = product.StockQuantity,
        CreatedAt = product.CreatedAt
    };
}