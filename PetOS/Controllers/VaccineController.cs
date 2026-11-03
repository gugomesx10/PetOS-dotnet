using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Common;
using PetOS.Dto.Vaccine;
using PetOS.Hypermedia;
using PetOS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PetOS.Controllers;

/// <summary>
/// Gerencia o histórico de vacinação dos animais.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VaccineController : ControllerBase
{
    private readonly IVaccineService _service;
    private readonly IPetService _petService;
    private readonly IResourceReadService _queries;

    public VaccineController(
        IVaccineService service,
        IPetService petService,
        IResourceReadService queries)
    {
        _service = service;
        _petService = petService;
        _queries = queries;
    }

    /// <summary>
    /// Lista as vacinas cadastradas.
    /// </summary>
    /// <param name="query">Filtros, paginação e ordenação.</param>
    /// <response code="200">Vacinas encontradas.</response>
    /// <response code="204">Nenhuma vacina encontrada.</response>
    /// <response code="400">Parâmetros inválidos.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar vacinas",
        Description = "Consulta as vacinas com filtros por pet, nome e fabricante, paginação, ordenação e links HATEOAS.",
        OperationId = "Vaccines_GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] VaccineQuery query)
    {
        var result = await _queries.GetVaccinesAsync(query);

        return PageResult(result);
    }

    /// <summary>
    /// Consulta uma vacina pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da vacina.</param>
    /// <response code="200">Vacina encontrada.</response>
    /// <response code="404">Vacina não encontrada.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Buscar vacina por ID",
        Description = "Retorna os dados de uma vacina e os links para operações relacionadas.",
        OperationId = "Vaccines_GetById")]
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

        return Ok(Hateoas.Item(
            "Vaccine",
            vaccine.Id,
            vaccine,
            "Vacina encontrada com sucesso"
        ));
    }

    /// <summary>
    /// Lista as vacinas de um pet.
    /// </summary>
    /// <remarks>
    /// Verifica a existência do animal e consulta
    /// seus registros de vacinação.
    /// </remarks>
    /// <param name="petId">Identificador do pet.</param>
    /// <param name="query">Filtros, paginação e ordenação.</param>
    /// <response code="200">Vacinas encontradas.</response>
    /// <response code="204">Nenhuma vacina encontrada.</response>
    /// <response code="400">Parâmetros inválidos.</response>
    /// <response code="404">Pet não encontrado.</response>
    [HttpGet("pet/{petId}")]
    [SwaggerOperation(
        Summary = "Listar vacinas de um pet",
        Description = "Consulta o histórico de vacinação de um animal com paginação e filtros adicionais.",
        OperationId = "Vaccines_GetByPetId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPetId(
        long petId,
        [FromQuery] VaccineQuery query)
    {
        var pet = await _petService.GetByIdAsync(petId);

        if (pet == null)
        {
            return NotFound(new
            {
                message = "Pet não encontrado"
            });
        }

        query.PetId = petId;

        var result = await _queries.GetVaccinesAsync(query);

        return PageResult(result);
    }

    /// <summary>
    /// Registra uma vacina para um pet.
    /// </summary>
    /// <remarks>
    /// Verifica a existência do animal antes de cadastrar
    /// o registro de vacinação.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="dto">Dados da vacina e identificador do pet.</param>
    /// <response code="201">Vacina cadastrada.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Pet não encontrado.</response>
    [Authorize]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastrar vacina",
        Description = "Registra uma vacina associada a um pet e retorna links HATEOAS.",
        OperationId = "Vaccines_Create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] VaccineCreateDto dto)
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
            Hateoas.Item(
                "Vaccine",
                created.Id,
                created,
                "Vacina cadastrada com sucesso"
            )
        );
    }

    /// <summary>
    /// Atualiza um registro de vacinação.
    /// </summary>
    /// <remarks>
    /// Verifica se a vacina e o pet informado existem.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="id">Identificador da vacina.</param>
    /// <param name="dto">Dados atualizados.</param>
    /// <response code="200">Vacina atualizada.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Vacina ou pet não encontrado.</response>
    [Authorize]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualizar vacina",
        Description = "Altera as informações de uma vacina cadastrada.",
        OperationId = "Vaccines_Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] VaccineCreateDto dto)
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
            message = "Vacina atualizada com sucesso",
            links = Hateoas.ForItem("Vaccine", id)
        });
    }

    /// <summary>
    /// Exclui um registro de vacinação.
    /// </summary>
    /// <remarks>
    /// Remove uma vacina cadastrada.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="id">Identificador da vacina.</param>
    /// <response code="200">Vacina excluída.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Vacina não encontrada.</response>
    [Authorize]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Excluir vacina",
        Description = "Remove um registro de vacinação e retorna links para a coleção.",
        OperationId = "Vaccines_Delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            message = "Vacina removida com sucesso",
            links = Hateoas.ForCollection("Vaccine")
        });
    }

    private IActionResult PageResult(
        PagedResult<VaccineResponseDto> result)
    {
        if (result.TotalCount == 0)
        {
            return NoContent();
        }

        return Ok(Hateoas.Page(
            Request,
            "Vaccine",
            result,
            "Vacinas encontradas com sucesso"
        ));
    }
}