using Microsoft.AspNetCore.Mvc;
using PetOS.DTOs.Alertas;
using PetOS.Models;
using PetOS.Services;

namespace PetOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertasController(IAlertaService alertaService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? petId, [FromQuery] AlertaStatus? status, [FromQuery] DateTime? ate)
    {
        var alertas = await alertaService.GetAllAsync(petId, status, ate);
        return Ok(alertas);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AlertaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var alerta = await alertaService.GetByIdAsync(id);
        return Ok(alerta);
    }

    [HttpGet("pet/{petId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<AlertaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPet(int petId)
    {
        var alertas = await alertaService.GetAllAsync(petId, null, null);
        return Ok(alertas);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AlertaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAlertaDto dto)
    {
        var alerta = await alertaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = alerta.Id }, alerta);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAlertaDto dto)
    {
        await alertaService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await alertaService.DeleteAsync(id);
        return NoContent();
    }
}

