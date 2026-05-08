using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Pet;
using PetOS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PetOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetController : ControllerBase
{
    private readonly IPetService _service;

    public PetController(IPetService service)
    {
        _service = service;
    }

    /// <summary>
    /// lista todos os pets que são cadastrado no sistema
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Lista por todos os pets")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAll()
    {
        var pets = await _service.GetAllSync();

        if (!pets.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Pets encontrados com sucesso",
            data = pets
        });
    }

    /// <summary>
    /// buscar um pet pelo id que inserir
    /// </summar>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Busca um pet por ID")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var pet = await _service.GetByIdAsync(id);

        if (pet == null)
        {
            return NotFound(new
            {
                message = "Pet não encontrado"
            });
        }

        return Ok(new
        {
            message = "Pet encontrado com sucesso",
            data = pet
        });
    }
    
    /// <summary>
    /// Busca pets por espécie
    /// </summary>
    [HttpGet("species/{species}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetBySpecies(string species)
    {
        var pets = await _service.GetBySpeciesAsync(species);

        if (!pets.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Pets encontrados com sucesso.",
            data = pets
        });
    }

    /// <summary>
    /// Busca pets por nome
    /// </summary>
    [HttpGet("name/{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetByName(string name)
    {
        var pets = await _service.GetByNameAsync(name);

        if (!pets.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Pets encontrados com sucesso.",
            data = pets
        });
    }

    /// <summary>
    /// cadastrar um novo petzinho
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastrar um novo pet")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(PetCreateDto dto)
    {
        var createdPet = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdPet.Id },
            new
            {
                message = "Pet criado com sucesso",
                data = createdPet
            });
    }

    /// <summary>
    /// atualizar o pet existente
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar um pet existente")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(long id, PetCreateDto dto)
    {
        var pet = await _service.GetByIdAsync(id);

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
            message = "Pet atualizado com sucesso",
        });
    }

    /// <summary>
    /// deleta o pet
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Excluir um pet")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        var pet = await _service.GetByIdAsync(id);

        if (pet == null)
        {
            return NotFound(new
            {
                message = "Pet não encontrado"
            });
        }
        
        await _service.DeleteAsync(id);
        return Ok(new
        {
            message = "Pet excluido com sucesso"
        });
    }
}