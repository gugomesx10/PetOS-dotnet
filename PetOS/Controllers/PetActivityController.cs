using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Common;
using PetOS.Dto.PetActivity;
using PetOS.Hypermedia;
using PetOS.Models;
using PetOS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PetOS.Controllers;

/// <summary>
/// Gerencia o histórico de atividades dos pets armazenado no MongoDB.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PetActivityController : ControllerBase
{
    private readonly IPetActivityService _service;
    private readonly IPetService _petService;

    public PetActivityController(
        IPetActivityService service,
        IPetService petService)
    {
        _service = service;
        _petService = petService;
    }

    /// <summary>
    /// Lista as atividades dos pets com paginação, filtros e ordenação.
    /// </summary>
    /// <response code="200">Atividades encontradas.</response>
    /// <response code="204">Nenhuma atividade encontrada.</response>
    /// <response code="400">Parâmetros de consulta inválidos.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar atividades dos pets",
        Description = "Consulta o histórico de atividades armazenado no MongoDB.",
        OperationId = "PetActivities_GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] PetActivityQuery query)
    {
        var result = await _service.GetPagedAsync(query);

        return PageResult(result);
    }

    /// <summary>
    /// Consulta uma atividade pelo identificador MongoDB.
    /// </summary>
    /// <param name="id">ObjectId da atividade.</param>
    /// <response code="200">Atividade encontrada.</response>
    /// <response code="400">Identificador inválido.</response>
    /// <response code="404">Atividade não encontrada.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Buscar atividade por ID",
        Description = "Retorna os dados de uma atividade pelo identificador MongoDB.",
        OperationId = "PetActivities_GetById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var activity = await _service.GetByIdAsync(id);

        if (activity == null)
        {
            return NotFound(new
            {
                message = "Atividade não encontrada"
            });
        }

        return Ok(
            Hateoas.Item(
                "PetActivity",
                activity.Id,
                activity,
                "Atividade encontrada com sucesso"
            )
        );
    }

    /// <summary>
    /// Lista as atividades de um pet específico.
    /// </summary>
    /// <param name="petId">Identificador do pet no Oracle.</param>
    /// <param name="query">Parâmetros de paginação e filtros.</param>
    /// <response code="200">Atividades encontradas.</response>
    /// <response code="204">O pet não possui atividades.</response>
    /// <response code="404">Pet não encontrado.</response>
    [HttpGet("pet/{petId:long}")]
    [SwaggerOperation(
        Summary = "Listar atividades de um pet",
        Description = "Consulta o histórico de atividades de um animal.",
        OperationId = "PetActivities_GetByPetId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPetId(
        long petId,
        [FromQuery] PetActivityQuery query)
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

        var result = await _service.GetPagedAsync(query);

        return PageResult(result);
    }

    /// <summary>
    /// Registra uma nova atividade no MongoDB.
    /// </summary>
    /// <remarks>
    /// O pet deve existir no Oracle.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="dto">Dados da atividade.</param>
    /// <response code="201">Atividade criada.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Autenticação necessária.</response>
    /// <response code="404">Pet não encontrado.</response>
    [Authorize]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastrar atividade",
        Description = "Registra uma atividade de um pet no MongoDB.",
        OperationId = "PetActivities_Create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] PetActivityCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            Hateoas.Item(
                "PetActivity",
                created.Id,
                created,
                "Atividade criada com sucesso"
            )
        );
    }

    /// <summary>
    /// Atualiza uma atividade existente.
    /// </summary>
    /// <param name="id">ObjectId da atividade.</param>
    /// <param name="dto">Dados atualizados.</param>
    /// <response code="200">Atividade atualizada.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Autenticação necessária.</response>
    /// <response code="404">Atividade ou pet não encontrado.</response>
    [Authorize]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualizar atividade",
        Description = "Atualiza os dados de uma atividade armazenada no MongoDB.",
        OperationId = "PetActivities_Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] PetActivityCreateDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Atividade não encontrada"
            });
        }

        return Ok(new
        {
            message = "Atividade atualizada com sucesso",
            links = Hateoas.ForItem("PetActivity", id)
        });
    }

    /// <summary>
    /// Exclui uma atividade do MongoDB.
    /// </summary>
    /// <param name="id">ObjectId da atividade.</param>
    /// <response code="200">Atividade excluída.</response>
    /// <response code="400">Identificador inválido.</response>
    /// <response code="401">Autenticação necessária.</response>
    /// <response code="404">Atividade não encontrada.</response>
    [Authorize]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Excluir atividade",
        Description = "Remove um registro de atividade do MongoDB.",
        OperationId = "PetActivities_Delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Atividade não encontrada"
            });
        }

        return Ok(new
        {
            message = "Atividade excluída com sucesso",
            links = Hateoas.ForCollection("PetActivity")
        });
    }

    private IActionResult PageResult(
        PagedResult<PetOS.Models.PetActivity> result)
    {
        if (result.TotalCount == 0)
        {
            return NoContent();
        }

        return Ok(
            Hateoas.Page(
                Request,
                "PetActivity",
                result,
                "Atividades encontradas com sucesso"
            )
        );
    }
}