using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Pet;
using PetOS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PetOS.Controllers;

/// <summary>
/// Gerencia o cadastro e a consulta de animais no PetOS.
/// </summary>
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
    /// Lista todos os pets cadastrados.
    /// </summary>
    /// <remarks>
    /// Retorna os animais registrados no sistema.
    /// Se não houver registros, retorna 204.
    /// </remarks>
    /// <response code="200">Pets encontrados com sucesso.</response>
    /// <response code="204">Nenhum pet cadastrado.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar todos os pets",
        Description = "Consulta todos os animais cadastrados no PetOS.",
        OperationId = "Pets_GetAll")]
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
    /// Consulta um pet pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do pet.</param>
    /// <response code="200">Pet encontrado com sucesso.</response>
    /// <response code="404">Pet não encontrado.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Buscar pet por ID",
        Description = "Retorna os dados de um pet a partir do identificador informado.",
        OperationId = "Pets_GetById")]
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
    /// Busca pets por espécie.
    /// </summary>
    /// <param name="species">Espécie dos animais que serão consultados.</param>
    /// <response code="200">Pets encontrados com sucesso.</response>
    /// <response code="204">Nenhum pet corresponde à espécie informada.</response>
    [HttpGet("species/{species}")]
    [SwaggerOperation(
        Summary = "Buscar pets por espécie",
        Description = "Filtra os animais cadastrados pela espécie informada.",
        OperationId = "Pets_GetBySpecies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetBySpecies(string species)
    {
        var pets = await _service.GetBySpecieAsync(species);

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
    /// Busca pets pelo nome.
    /// </summary>
    /// <param name="name">Nome utilizado na pesquisa.</param>
    /// <response code="200">Pets encontrados com sucesso.</response>
    /// <response code="204">Nenhum pet corresponde ao nome informado.</response>
    [HttpGet("name/{name}")]
    [SwaggerOperation(
        Summary = "Buscar pets por nome",
        Description = "Consulta os animais cadastrados utilizando o nome informado.",
        OperationId = "Pets_GetByName")]
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
    /// Cadastra um novo pet.
    /// </summary>
    /// <remarks>
    /// Registra um animal no sistema.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="dto">Dados necessários para cadastrar o pet.</param>
    /// <response code="201">Pet cadastrado com sucesso.</response>
    /// <response code="400">Dados de cadastro inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [Authorize]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastrar pet",
        Description = "Cria um novo registro de animal e retorna seus dados.",
        OperationId = "Pets_Create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(PetCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Dados inválidos"
            });
        }

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
    /// Atualiza os dados de um pet.
    /// </summary>
    /// <remarks>
    /// Altera os dados de um animal já cadastrado.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="id">Identificador do pet.</param>
    /// <param name="dto">Dados atualizados do animal.</param>
    /// <response code="200">Pet atualizado com sucesso.</response>
    /// <response code="400">Dados de atualização inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Pet não encontrado.</response>
    [Authorize]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualizar pet",
        Description = "Atualiza as informações de um animal existente.",
        OperationId = "Pets_Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, PetCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Dados inválidos"
            });
        }

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
            message = "Pet atualizado com sucesso"
        });
    }

    /// <summary>
    /// Exclui um pet cadastrado.
    /// </summary>
    /// <remarks>
    /// Remove o animal identificado pelo ID.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="id">Identificador do pet que será excluído.</param>
    /// <response code="200">Pet excluído com sucesso.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Pet não encontrado.</response>
    [Authorize]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Excluir pet",
        Description = "Remove um animal cadastrado a partir do seu identificador.",
        OperationId = "Pets_Delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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