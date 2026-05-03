using Microsoft.AspNetCore.Mvc;
using PetOS.DTOs.Rotinas;
using PetOS.Services;

namespace PetOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RotinasController(IRotinaService rotinaService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RotinaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? petId, [FromQuery] bool? ativa)
    {
        var rotinas = await rotinaService.GetAllAsync(petId, ativa);
        return Ok(rotinas);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RotinaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var rotina = await rotinaService.GetByIdAsync(id);
        return Ok(rotina);
    }

    [HttpGet("pet/{petId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<RotinaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPet(int petId)
    {
        var rotinas = await rotinaService.GetAllAsync(petId, null);
        return Ok(rotinas);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RotinaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRotinaDto dto)
    {
        var rotina = await rotinaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = rotina.Id }, rotina);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRotinaDto dto)
    {
        await rotinaService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await rotinaService.DeleteAsync(id);
        return NoContent();
    }
}

