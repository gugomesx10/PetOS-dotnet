using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Common;
using PetOS.Dto.Pet;
using PetOS.Hypermedia;
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
    private readonly IResourceReadService _queries;

    public PetController(
        IPetService service,
        IResourceReadService queries)
    {
        _service = service;
        _queries = queries;
    }

    /// <summary>
    /// Lista os pets cadastrados com paginação.
    /// </summary>
    /// <param name="query">Filtros, paginação e ordenação da consulta.</param>
    /// <response code="200">Página de pets encontrada.</response>
    /// <response code="204">Nenhum pet encontrado.</response>
    /// <response code="400">Parâmetros de consulta inválidos.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar pets",
        Description = "Consulta pets com filtros por nome e espécie, paginação e ordenação. Inclui metadados e links HATEOAS.",
        OperationId = "Pets_GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] PetQuery query)
    {
        var result = await _queries.GetPetsAsync(query);

        return PageResult(result);
    }

    /// <summary>
    /// Consulta um pet pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do pet.</param>
    /// <response code="200">Pet encontrado.</response>
    /// <response code="404">Pet não encontrado.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Buscar pet por ID",
        Description = "Retorna os dados de um pet e os links para consultar, atualizar ou excluir o registro.",
        OperationId = "Pets_GetById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        var pet = await _service.GetByIdAsync(id);

        if (pet == null)
        {
            return NotFound(new
            {
                message = "Pet não encontrado"
            });
        }

        return Ok(Hateoas.Item(
            "Pet",
            pet.Id,
            pet,
            "Pet encontrado com sucesso"
        ));
    }

    /// <summary>
    /// Busca pets por espécie.
    /// </summary>
    /// <param name="species">Espécie dos pets.</param>
    /// <param name="query">Parâmetros de paginação e ordenação.</param>
    /// <response code="200">Pets encontrados.</response>
    /// <response code="204">Nenhum pet corresponde à espécie.</response>
    /// <response code="400">Parâmetros inválidos.</response>
    [HttpGet("species/{species}")]
    [SwaggerOperation(
        Summary = "Buscar pets por espécie",
        Description = "Filtra os animais pela espécie e retorna resultados paginados.",
        OperationId = "Pets_GetBySpecies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBySpecies(
        string species,
        [FromQuery] PetQuery query)
    {
        query.Species = species;

        var result = await _queries.GetPetsAsync(query);

        return PageResult(result);
    }

    /// <summary>
    /// Busca pets pelo nome.
    /// </summary>
    /// <param name="name">Nome utilizado na pesquisa.</param>
    /// <param name="query">Parâmetros de paginação e ordenação.</param>
    /// <response code="200">Pets encontrados.</response>
    /// <response code="204">Nenhum pet corresponde ao nome.</response>
    /// <response code="400">Parâmetros inválidos.</response>
    [HttpGet("name/{name}")]
    [SwaggerOperation(
        Summary = "Buscar pets por nome",
        Description = "Pesquisa animais pelo nome e retorna uma lista paginada.",
        OperationId = "Pets_GetByName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByName(
        string name,
        [FromQuery] PetQuery query)
    {
        query.Name = name;

        var result = await _queries.GetPetsAsync(query);

        return PageResult(result);
    }

    /// <summary>
    /// Cadastra um novo pet.
    /// </summary>
    /// <remarks>
    /// Registra um animal no sistema.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="dto">Dados necessários para cadastrar o pet.</param>
    /// <response code="201">Pet cadastrado.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [Authorize]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastrar pet",
        Description = "Cria um pet e retorna seus dados, links HATEOAS e o endereço do recurso.",
        OperationId = "Pets_Create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody] PetCreateDto dto)
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
            Hateoas.Item(
                "Pet",
                createdPet.Id,
                createdPet,
                "Pet criado com sucesso"
            )
        );
    }

    /// <summary>
    /// Atualiza os dados de um pet.
    /// </summary>
    /// <remarks>
    /// Altera os dados de um animal existente.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="id">Identificador do pet.</param>
    /// <param name="dto">Dados atualizados.</param>
    /// <response code="200">Pet atualizado.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Pet não encontrado.</response>
    [Authorize]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualizar pet",
        Description = "Atualiza as informações de um animal cadastrado.",
        OperationId = "Pets_Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] PetCreateDto dto)
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
            message = "Pet atualizado com sucesso",
            links = Hateoas.ForItem("Pet", id)
        });
    }

    /// <summary>
    /// Exclui um pet cadastrado.
    /// </summary>
    /// <remarks>
    /// Remove o animal pelo identificador.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="id">Identificador do pet.</param>
    /// <response code="200">Pet excluído.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Pet não encontrado.</response>
    [Authorize]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Excluir pet",
        Description = "Remove um pet existente e retorna links para a coleção.",
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
            message = "Pet excluído com sucesso",
            links = Hateoas.ForCollection("Pet")
        });
    }

    private IActionResult PageResult(
        PagedResult<PetResponseDto> result)
    {
        if (result.TotalCount == 0)
        {
            return NoContent();
        }

        return Ok(Hateoas.Page(
            Request,
            "Pet",
            result,
            "Pets encontrados com sucesso"
        ));
    }
}