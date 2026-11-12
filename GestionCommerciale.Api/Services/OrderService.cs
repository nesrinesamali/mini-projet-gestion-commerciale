using GestionCommerciale.Api.Data;
using GestionCommerciale.Api.Dtos;
using GestionCommerciale.Api.Exceptions;
using GestionCommerciale.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionCommerciale.Api.Services;

public class OrderService : IOrderService
{
    private const decimal VatRate = 0.19m;

    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<OrderDto>> GetAllAsync()
    {
        var orders = await _db.Orders
            .AsNoTracking()
            .Include(o => o.Client)
            .Include(o => o.Lines).ThenInclude(l => l.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return orders.Select(ToDto).ToList();
    }

    public async Task<OrderDto> GetByIdAsync(int id)
    {
        var order = await GetEntityAsync(id);
        return ToDto(order);
    }

    public async Task<OrderDto> CreateAsync(OrderCreateUpdateDto dto)
    {
        await EnsureClientExistsAsync(dto.ClientId);

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            ClientId = dto.ClientId
        };

        await FillLinesAndTotalsAsync(order, dto.Lines);

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(order.Id);
    }

    public async Task<OrderDto> UpdateAsync(int id, OrderCreateUpdateDto dto)
    {
        var order = await GetEntityAsync(id);
        EnsureIsDraft(order);
        await EnsureClientExistsAsync(dto.ClientId);

        order.ClientId = dto.ClientId;
        order.Lines.Clear();
        await FillLinesAndTotalsAsync(order, dto.Lines);

        await _db.SaveChangesAsync();
        return await GetByIdAsync(order.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var order = await GetEntityAsync(id);

        if (order.Status == OrderStatus.Validated)
        {
            throw new BusinessRuleException("Impossible de supprimer une commande validée.");
        }

        _db.Orders.Remove(order);
        await _db.SaveChangesAsync();
    }

    public async Task<OrderDto> ValidateAsync(int id)
    {
        var order = await GetEntityAsync(id);
        EnsureIsDraft(order);

        if (order.Lines.Count == 0)
        {
            throw new BusinessRuleException("Impossible de valider une commande sans ligne.");
        }

        // 1) On vérifie le stock de toutes les lignes avant de modifier quoi que ce soit.
        foreach (var line in order.Lines)
        {
            if (line.Quantity > line.Product!.StockQuantity)
            {
                throw new BusinessRuleException(
                    $"Stock insuffisant pour '{line.Product.Name}' : demandé {line.Quantity}, disponible {line.Product.StockQuantity}.");
            }
        }

        // 2) Tout est bon : on diminue le stock et on valide.
        foreach (var line in order.Lines)
        {
            line.Product!.StockQuantity -= line.Quantity;
        }

        order.Status = OrderStatus.Validated;
        await _db.SaveChangesAsync();

        return ToDto(order);
    }

    private async Task<Order> GetEntityAsync(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Client)
            .Include(o => o.Lines).ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order ?? throw new NotFoundException($"Commande {id} introuvable.");
    }

    private async Task EnsureClientExistsAsync(int clientId)
    {
        var exists = await _db.Clients.AnyAsync(c => c.Id == clientId);
        if (!exists)
        {
            throw new NotFoundException($"Client {clientId} introuvable.");
        }
    }

    private static void EnsureIsDraft(Order order)
    {
        if (order.Status != OrderStatus.Draft)
        {
            throw new BusinessRuleException("Seule une commande en brouillon peut être modifiée ou validée.");
        }
    }

    private async Task FillLinesAndTotalsAsync(Order order, List<OrderLineCreateDto> lines)
    {
        // Si le même produit apparaît sur plusieurs lignes, on additionne les quantités.
        var requested = lines
            .GroupBy(l => l.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(l => l.Quantity) })
            .ToList();

        var productIds = requested.Select(r => r.ProductId).ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        foreach (var item in requested)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
            {
                throw new NotFoundException($"Produit {item.ProductId} introuvable.");
            }

            if (item.Quantity > product.StockQuantity)
            {
                throw new BusinessRuleException(
                    $"Stock insuffisant pour '{product.Name}' : demandé {item.Quantity}, disponible {product.StockQuantity}.");
            }

            order.Lines.Add(new OrderLine
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.UnitPriceHt,
                LineTotal = item.Quantity * product.UnitPriceHt
            });
        }

        order.TotalHt = order.Lines.Sum(l => l.LineTotal);
        order.TotalTtc = Math.Round(order.TotalHt * (1 + VatRate), 2);
    }

    private static string GenerateOrderNumber()
    {
        var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        return $"CMD-{DateTime.UtcNow:yyyyMMdd}-{suffix}";
    }

    private static OrderDto ToDto(Order order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        ClientId = order.ClientId,
        ClientName = order.Client is null
            ? string.Empty
            : $"{order.Client.FirstNameOrCompany} {order.Client.LastName}".Trim(),
        OrderDate = order.OrderDate,
        Status = order.Status.ToString(),
        TotalHt = order.TotalHt,
        TotalTtc = order.TotalTtc,
        Lines = order.Lines.Select(l => new OrderLineDto
        {
            Id = l.Id,
            ProductId = l.ProductId,
            ProductReference = l.Product?.Reference ?? string.Empty,
            ProductName = l.Product?.Name ?? string.Empty,
            Quantity = l.Quantity,
            UnitPrice = l.UnitPrice,
            LineTotal = l.LineTotal
        }).ToList()
    };
}