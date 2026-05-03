using Microsoft.AspNetCore.Mvc;
using PetOS.DTOs.Pets;
using PetOS.Services;

namespace PetOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController(IPetService petService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PetResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? especie, [FromQuery] string? responsavel)
    {
        var pets = await petService.GetAllAsync(especie, responsavel);
        return Ok(pets);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PetResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var pet = await petService.GetByIdAsync(id);
        return Ok(pet);
    }

    [HttpGet("especie/{especie}")]
    [ProducesResponseType(typeof(IReadOnlyList<PetResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEspecie(string especie)
    {
        var pets = await petService.GetAllAsync(especie, null);
        return Ok(pets);
    }

    [HttpGet("responsavel/{responsavel}")]
    [ProducesResponseType(typeof(IReadOnlyList<PetResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByResponsavel(string responsavel)
    {
        var pets = await petService.GetAllAsync(null, responsavel);
        return Ok(pets);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PetResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePetDto dto)
    {
        var pet = await petService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePetDto dto)
    {
        await petService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await petService.DeleteAsync(id);
        return NoContent();
    }
}

