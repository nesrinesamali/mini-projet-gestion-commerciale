using GestionCommerciale.Api.Dtos;
using GestionCommerciale.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionCommerciale.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ClientDto>>> GetAll()
    {
        return Ok(await _clientService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetById(int id)
    {
        return Ok(await _clientService.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(ClientCreateUpdateDto dto)
    {
        var created = await _clientService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClientDto>> Update(int id, ClientCreateUpdateDto dto)
    {
        return Ok(await _clientService.UpdateAsync(id, dto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _clientService.DeleteAsync(id);
        return NoContent();
    }
}