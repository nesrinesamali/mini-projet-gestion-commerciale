using GestionCommerciale.Api.Dtos;
using GestionCommerciale.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionCommerciale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        return Ok(await _orderService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        return Ok(await _orderService.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(OrderCreateUpdateDto dto)
    {
        var created = await _orderService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDto>> Update(int id, OrderCreateUpdateDto dto)
    {
        return Ok(await _orderService.UpdateAsync(id, dto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _orderService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/validate")]
    public async Task<ActionResult<OrderDto>> Validate(int id)
    {
        return Ok(await _orderService.ValidateAsync(id));
    }
}