using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Alert;
using PetOS.Dto.Common;
using PetOS.Hypermedia;
using PetOS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PetOS.Controllers;

/// <summary>
/// Gerencia os alertas e lembretes de cuidados dos animais.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AlertController : ControllerBase
{
    private readonly IAlertService _service;
    private readonly IPetService _petService;
    private readonly IVaccineService _vaccineService;
    private readonly IResourceReadService _queries;

    public AlertController(
        IAlertService service,
        IPetService petService,
        IVaccineService vaccineService,
        IResourceReadService queries)
    {
        _service = service;
        _petService = petService;
        _vaccineService = vaccineService;
        _queries = queries;
    }

    /// <summary>
    /// Lista os alertas cadastrados.
    /// </summary>
    /// <param name="query">Filtros, paginação e ordenação.</param>
    /// <response code="200">Alertas encontrados.</response>
    /// <response code="204">Nenhum alerta encontrado.</response>
    /// <response code="400">Parâmetros inválidos.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar alertas",
        Description = "Consulta alertas com filtros por pet e status de leitura, paginação, ordenação e links HATEOAS.",
        OperationId = "Alerts_GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] AlertQuery query)
    {
        var result = await _queries.GetAlertsAsync(query);

        return PageResult(result);
    }

    /// <summary>
    /// Consulta um alerta pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do alerta.</param>
    /// <response code="200">Alerta encontrado.</response>
    /// <response code="404">Alerta não encontrado.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Buscar alerta por ID",
        Description = "Retorna os dados de um alerta e os links para operações relacionadas.",
        OperationId = "Alerts_GetById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        var alert = await _service.GetByIdAsync(id);

        if (alert == null)
        {
            return NotFound(new
            {
                message = "Alerta não encontrado"
            });
        }

        return Ok(Hateoas.Item(
            "Alert",
            alert.Id,
            alert,
            "Alerta encontrado com sucesso"
        ));
    }

    /// <summary>
    /// Lista os alertas não lidos.
    /// </summary>
    /// <param name="query">Filtros, paginação e ordenação.</param>
    /// <response code="200">Alertas não lidos encontrados.</response>
    /// <response code="204">Nenhum alerta não lido.</response>
    /// <response code="400">Parâmetros inválidos.</response>
    [HttpGet("unread")]
    [SwaggerOperation(
        Summary = "Listar alertas não lidos",
        Description = "Retorna apenas os alertas pendentes de leitura, com filtros adicionais e paginação.",
        OperationId = "Alerts_GetUnread")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUnread(
        [FromQuery] AlertQuery query)
    {
        query.IsRead = false;

        var result = await _queries.GetAlertsAsync(query);

        return PageResult(result);
    }

    /// <summary>
    /// Cadastra um alerta para um pet.
    /// </summary>
    /// <remarks>
    /// O pet deve existir. Caso uma vacina seja informada,
    /// ela também deve existir e pertencer ao mesmo pet.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="dto">Dados do alerta.</param>
    /// <response code="201">Alerta cadastrado.</response>
    /// <response code="400">Dados inválidos ou vacina de outro pet.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Pet ou vacina não encontrado.</response>
    [Authorize]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastrar alerta",
        Description = "Registra um alerta associado a um pet. A vacina é opcional, mas deve pertencer ao mesmo animal.",
        OperationId = "Alerts_Create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] AlertCreateDto dto)
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

        if (dto.VaccineId.HasValue)
        {
            var vaccine = await _vaccineService.GetByIdAsync(
                dto.VaccineId.Value
            );

            if (vaccine == null)
            {
                return NotFound(new
                {
                    message = "Vacina não encontrada"
                });
            }

            if (vaccine.PetId != dto.PetId)
            {
                return BadRequest(new
                {
                    message = "A vacina informada não pertence ao pet informado"
                });
            }
        }

        var created = await _service.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            Hateoas.Item(
                "Alert",
                created.Id,
                created,
                "Alerta criado com sucesso"
            )
        );
    }

    /// <summary>
    /// Atualiza um alerta existente.
    /// </summary>
    /// <remarks>
    /// Verifica a existência do alerta e do pet.
    /// Se houver vacina associada, valida seu vínculo com o animal.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="id">Identificador do alerta.</param>
    /// <param name="dto">Dados atualizados.</param>
    /// <response code="200">Alerta atualizado.</response>
    /// <response code="400">Dados inválidos ou associação inconsistente.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Alerta, pet ou vacina não encontrado.</response>
    [Authorize]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualizar alerta",
        Description = "Atualiza um alerta após validar o pet e a vacina associada.",
        OperationId = "Alerts_Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long id,
        [FromBody] AlertCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "Dados inválidos"
            });
        }

        var alert = await _service.GetByIdAsync(id);

        if (alert == null)
        {
            return NotFound(new
            {
                message = "Alerta não encontrado"
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

        if (dto.VaccineId.HasValue)
        {
            var vaccine = await _vaccineService.GetByIdAsync(
                dto.VaccineId.Value
            );

            if (vaccine == null)
            {
                return NotFound(new
                {
                    message = "Vacina não encontrada"
                });
            }

            if (vaccine.PetId != dto.PetId)
            {
                return BadRequest(new
                {
                    message = "A vacina informada não pertence ao pet informado"
                });
            }
        }

        await _service.UpdateAsync(id, dto);

        return Ok(new
        {
            message = "Alerta atualizado com sucesso",
            links = Hateoas.ForItem("Alert", id)
        });
    }

    /// <summary>
    /// Exclui um alerta pelo identificador.
    /// </summary>
    /// <remarks>
    /// Remove um alerta existente.
    /// Requer autenticação JWT.
    /// </remarks>
    /// <param name="id">Identificador do alerta.</param>
    /// <response code="200">Alerta excluído.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Alerta não encontrado.</response>
    [Authorize]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Excluir alerta",
        Description = "Remove um alerta e retorna links para a coleção.",
        OperationId = "Alerts_Delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        var alert = await _service.GetByIdAsync(id);

        if (alert == null)
        {
            return NotFound(new
            {
                message = "Alerta não encontrado"
            });
        }

        await _service.DeleteAsync(id);

        return Ok(new
        {
            message = "Alerta removido com sucesso",
            links = Hateoas.ForCollection("Alert")
        });
    }

    private IActionResult PageResult(
        PagedResult<AlertResponseDto> result)
    {
        if (result.TotalCount == 0)
        {
            return NoContent();
        }

        return Ok(Hateoas.Page(
            Request,
            "Alert",
            result,
            "Alertas encontrados com sucesso"
        ));
    }
}