using Microsoft.AspNetCore.Mvc;
using PetOS.DTOs.Vacinas;
using PetOS.Services;

namespace PetOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VacinasController(IVacinaService vacinaService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<VacinaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int? petId, [FromQuery] DateOnly? aplicadaAte)
    {
        var vacinas = await vacinaService.GetAllAsync(petId, aplicadaAte);
        return Ok(vacinas);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VacinaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var vacina = await vacinaService.GetByIdAsync(id);
        return Ok(vacina);
    }

    [HttpGet("pet/{petId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<VacinaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPet(int petId)
    {
        var vacinas = await vacinaService.GetAllAsync(petId, null);
        return Ok(vacinas);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VacinaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateVacinaDto dto)
    {
        var vacina = await vacinaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = vacina.Id }, vacina);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVacinaDto dto)
    {
        await vacinaService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await vacinaService.DeleteAsync(id);
        return NoContent();
    }
}

