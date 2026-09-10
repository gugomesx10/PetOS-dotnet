using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Vaccine;
using PetOS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PetOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VaccineController : ControllerBase
{
    private readonly IVaccineService _service;
    private readonly IPetService _petService;

    public VaccineController(
        IVaccineService service,
        IPetService petService)
    {
        _service = service;
        _petService = petService;
    }

    /// <summary>
    /// Lista todas vacinas
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Lista todas vacinas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAll()
    {
        var vacines = await _service.GetAllAsync();
        
        if(!vacines.Any())
        {
            return NoContent();
        }
        return Ok(new
        {
            message = "Vacinas encontradas com sucesso",
            data = vacines
        });
    }

    /// <summary>
    /// Faz uma buscar pela vacina ao inserir Id
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Busca vacina por Id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        var vaccine = await _service.GetByIdAsync(id);

        if (vaccine == null)
        {
            return NotFound(new
            {
                message = "Vacina não encontrada"
            });
        }

        return Ok(new
        {
            message = "Vacina encontrada com sucesso",
            data = vaccine
        });
    }

    /// <summary>
    /// Busca as vacinas de um pet especifico
    /// </summary>
    [HttpGet("pet/{petId}")]
    [SwaggerOperation(Summary = "Busca as vacinas de um pet específico")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPetId(long petId)
    {
        var pet = await _petService.GetByIdAsync(petId);

        if (pet == null)
        {
            return NotFound(new
            {
                message = "Pet não encontrado"
            });
        }

        var vaccines = await _service.GetByPetIdAsync(petId);

        if (!vaccines.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Vacinas encontradas com sucesso",
            data = vaccines
        });
    }

    /// <summary>
    /// Cria a vacina e insere ao banco
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Adiciona a vacina no banco")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(VaccineCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Dados inválidos"
            });
        }

        var pet = await _petService.GetByIdAsync(dto.PetId);

        if (pet == null)
        {
            return NotFound(new
            {
                message = "Pet não encontrado"
            });
        }

        var created = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            new
            {
                message = "Vacina adicionada com sucesso",
                data = created
            });
    }

    /// <summary>
    /// Atualiza a vacina do pet
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualiza a vacina por Id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(long id, VaccineCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Dados inválidos"
            });
        }

        var vaccine = await _service.GetByIdAsync(id);

        if (vaccine == null)
        {
            return NotFound(new
            {
                message = "Vacina não encontrada"
            });
        }

        var pet = await _petService.GetByIdAsync(dto.PetId);

        if (pet == null)
        {
            return NotFound(new
            {
                message = "Pet não encontrado"
            });
        }

        await _service.UpdateAsync(id, dto);

        return Ok(new
        {
            message = "Vacina atualizada com sucesso"
        });
    }

    /// <summary>
    /// Deleta a vacina 
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover a vacina por Id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(long id)
    {
        var vaccine = await _service.GetByIdAsync(id);

        if (vaccine == null)
        {
            return NotFound(new
            {
                message = "Vacina não encontrada"
            });
        }

        await _service.DeleteAsync(id);

        return Ok(new
        {
            message = "Vacina removida com sucesso"
        });
    }

}