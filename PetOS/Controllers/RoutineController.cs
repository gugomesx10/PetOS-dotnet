using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Routine;
using PetOS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PetOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutineController : ControllerBase
{
    private readonly IRoutineService _service;

    public RoutineController(IRoutineService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista todas as rotinas
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Lista de rotinas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAll()
    {
        var routines = await _service.GetAllAsync();

        if (!routines.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Rotinas encontradas com sucesso",
            data = routines
        });
    }

    /// <summary>
    /// Busca rotina por Id
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Busca rotina por id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        var routine = await _service.GetByIdAsync(id);

        if (routine == null)
        {
            return NotFound(new
            {
                message = "Rotina não encontrada"
            });
        }

        return Ok(new
        {
            message = "Rotina encontrada com sucesso",
            data = routine
        });
    }

    /// <summary>
    /// Busca rotinas de um pet especifico 
    /// </summary>
    [HttpGet("pet/{petId}")]
    [SwaggerOperation(Summary = "Busca rotina por pet específico")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetByPetId(long petId)
    {
        var routines = await _service.GetByPetIdAsync(petId);

        if (!routines.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Rotinas encontradas com sucesso",
            data = routines
        });
    }

    /// <summary>
    /// Cadastra uma nova rotina
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Adiciona a rotina")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(RoutineCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Dados inválidos"
            });
        }

        var created = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            new
            {
                message = "Rotina criado com sucesso",
                data = created
            });
    }

    /// <summary>
    /// Atualiza rotina por Id
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualiza a rotina")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(long id, RoutineCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Dados inválidos"
            });
        }

        var routine = await _service.GetByIdAsync(id);

        if (routine == null)
        {
            return NotFound(new
            {
                message = "Rotina não encontrada"
            });
        }

        await _service.UpdateAsync(id, dto);

        return Ok(new
        {
            message = "Rotina atualizado com sucesso",
        });
    }

    /// <summary>
    /// Remove rotina por Id
    /// </summary>

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Remover a rotina")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(long id)
    {
        var routine = await _service.GetByIdAsync(id);

        if (routine == null)
        {
            return NotFound(new
            {
                message = "Rotina não encontrada"
            });
        }
        
        await _service.DeleteAsync(id);
        return Ok(new
        {
            message = "Rotina excluido com sucesso"
        });
    }
}