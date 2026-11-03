using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Common;
using PetOS.Dto.Routine;
using PetOS.Hypermedia;
using PetOS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PetOS.Controllers;

/// <summary>
/// Gerencia as rotinas de cuidados dos animais.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoutineController : ControllerBase
{
    private readonly IRoutineService _service;
    private readonly IPetService _petService;
    private readonly IResourceReadService _queries;

    public RoutineController(
        IRoutineService service,
        IPetService petService,
        IResourceReadService queries)
    {
        _service = service;
        _petService = petService;
        _queries = queries;
    }

    /// <summary>
    /// Lista as rotinas cadastradas.
    /// </summary>
    /// <param name="query">Filtros, paginação e ordenação.</param>
    /// <response code="200">Rotinas encontradas.</response>
    /// <response code="204">Nenhuma rotina encontrada.</response>
    /// <response code="400">Parâmetros inválidos.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar rotinas",
        Description = "Consulta as rotinas com filtros por pet e tipo, paginação, ordenação e links HATEOAS.",
        OperationId = "Routines_GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] RoutineQuery query)
    {
        var result = await _queries.GetRoutinesAsync(query);

        return PageResult(result);
    }

    /// <summary>
    /// Consulta uma rotina pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da rotina.</param>
    /// <response code="200">Rotina encontrada.</response>
    /// <response code="404">Rotina não encontrada.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Buscar rotina por ID",
        Description = "Retorna os detalhes da rotina e os links para operações relacionadas.",
        OperationId = "Routines_GetById")]
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

        return Ok(Hateoas.Item(
            "Routine",
            routine.Id,
            routine,
            "Rotina encontrada com sucesso"
        ));
    }

    /// <summary>
    /// Lista as rotinas de um pet.
    /// </summary>
    /// <remarks>
    /// Verifica a existência do animal antes de consultar
    /// suas rotinas cadastradas.
    /// </remarks>
    /// <param name="petId">Identificador do pet.</param>
    /// <param name="query">Filtros, paginação e ordenação.</param>
    /// <response code="200">Rotinas encontradas.</response>
    /// <response code="204">Nenhuma rotina encontrada.</response>
    /// <response code="400">Parâmetros inválidos.</response>
    /// <response code="404">Pet não encontrado.</response>
    [HttpGet("pet/{petId}")]
    [SwaggerOperation(
        Summary = "Listar rotinas de um pet",
        Description = "Consulta as rotinas de um animal, com paginação e filtros adicionais.",
        OperationId = "Routines_GetByPetId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPetId(
        long petId,
        [FromQuery] RoutineQuery query)
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

        var result = await _queries.GetRoutinesAsync(query);

        return PageResult(result);
    }

    /// <summary>
    /// Cadastra uma rotina de cuidados.
    /// </summary>
    /// <remarks>
    /// Registra uma rotina associada a um animal existente.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="dto">Dados da rotina e identificador do pet.</param>
    /// <response code="201">Rotina cadastrada.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Pet não encontrado.</response>
    [Authorize]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastrar rotina",
        Description = "Registra uma atividade ou cuidado para um pet e retorna links HATEOAS.",
        OperationId = "Routines_Create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] RoutineCreateDto dto)
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
                "Routine",
                created.Id,
                created,
                "Rotina criada com sucesso"
            )
        );
    }

    /// <summary>
    /// Atualiza uma rotina cadastrada.
    /// </summary>
    /// <remarks>
    /// Verifica a existência da rotina e do pet.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="id">Identificador da rotina.</param>
    /// <param name="dto">Dados atualizados.</param>
    /// <response code="200">Rotina atualizada.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Rotina ou pet não encontrado.</response>
    [Authorize]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualizar rotina",
        Description = "Altera os dados de uma rotina de cuidados existente.",
        OperationId = "Routines_Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] RoutineCreateDto dto)
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
            message = "Rotina atualizada com sucesso",
            links = Hateoas.ForItem("Routine", id)
        });
    }

    /// <summary>
    /// Exclui uma rotina cadastrada.
    /// </summary>
    /// <remarks>
    /// Remove a rotina pelo identificador.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="id">Identificador da rotina.</param>
    /// <response code="200">Rotina excluída.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Rotina não encontrada.</response>
    [Authorize]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Excluir rotina",
        Description = "Remove uma rotina e retorna links para a coleção.",
        OperationId = "Routines_Delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            message = "Rotina excluída com sucesso",
            links = Hateoas.ForCollection("Routine")
        });
    }

    private IActionResult PageResult(
        PagedResult<RoutineResponseDto> result)
    {
        if (result.TotalCount == 0)
        {
            return NoContent();
        }

        return Ok(Hateoas.Page(
            Request,
            "Routine",
            result,
            "Rotinas encontradas com sucesso"
        ));
    }
}