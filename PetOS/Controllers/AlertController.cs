using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Alert;
using PetOS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PetOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertController : ControllerBase
{
    private readonly IAlertService _service;
    private readonly IPetService _petService;
    private readonly IVaccineService _vaccineService;

    public AlertController(
        IAlertService service,
        IPetService petService,
        IVaccineService vaccineService)
    {
        _service = service;
        _petService = petService;
        _vaccineService = vaccineService;
    }

    /// <summary>
    /// Lista os alertas cadastrados.
    /// </summary>
    /// <response code="200">Alertas encontrados.</response>
    /// <response code="204">Nenhum alerta cadastrado.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar alertas",
        Description = "Consulta todos os alertas registrados no PetOS. Retorna uma lista de alertas ou 204 quando não existem registros.",
        OperationId = "Alerts_GetAll"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAll()
    {
        var alerts = await _service.GetAllAsync();

        if (!alerts.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Alertas encontrados com sucesso",
            data = alerts
        });
    }

    /// <summary>
    /// Consulta um alerta pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do alerta.</param>
    /// <response code="200">Alerta encontrado.</response>
    /// <response code="404">Alerta não encontrado.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Consultar alerta por ID",
        Description = "Busca os detalhes de um alerta utilizando seu identificador. Retorna 404 quando o registro não existe.",
        OperationId = "Alerts_GetById"
    )]
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

        return Ok(new
        {
            message = "Alerta encontrado com sucesso",
            data = alert
        });
    }

    /// <summary>
    /// Lista os alertas não lidos.
    /// </summary>
    /// <response code="200">Alertas não lidos encontrados.</response>
    /// <response code="204">Nenhum alerta não lido.</response>
    [HttpGet("unread")]
    [SwaggerOperation(
        Summary = "Listar alertas não lidos",
        Description = "Retorna os alertas que ainda não foram marcados como lidos. Caso não existam alertas pendentes de leitura, retorna 204.",
        OperationId = "Alerts_GetUnread"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetUnread()
    {
        var alerts = await _service.GetUnreadAsync();

        if (!alerts.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Alertas encontrados com sucesso",
            data = alerts
        });
    }

    /// <summary>
    /// Cadastra um alerta para um pet.
    /// </summary>
    /// <remarks>
    /// O pet deve existir. Caso uma vacina seja informada,
    /// ela também deve existir e pertencer ao mesmo pet.
    /// </remarks>
    /// <response code="201">Alerta cadastrado com sucesso.</response>
    /// <response code="400">Dados inválidos ou vacina de outro pet.</response>
    /// <response code="401">Autenticação necessária.</response>
    /// <response code="404">Pet ou vacina não encontrado.</response>
    [Authorize]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastrar alerta",
        Description = "Registra um alerta associado a um pet. A associação com uma vacina é opcional, mas, quando informada, a vacina deve pertencer ao pet indicado.",
        OperationId = "Alerts_Create"
    )]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(AlertCreateDto dto)
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
            new
            {
                message = "Alerta criado com sucesso",
                data = created
            }
        );
    }

    /// <summary>
    /// Atualiza um alerta existente.
    /// </summary>
    /// <remarks>
    /// Valida a existência do alerta e do pet.
    /// Caso uma vacina seja informada, verifica se pertence ao pet.
    /// </remarks>
    /// <param name="id">Identificador do alerta.</param>
    /// <response code="200">Alerta atualizado.</response>
    /// <response code="400">Dados inválidos ou associação inconsistente.</response>
    /// <response code="401">Autenticação necessária.</response>
    /// <response code="404">Alerta, pet ou vacina não encontrado.</response>
    [Authorize]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualizar alerta",
        Description = "Atualiza um alerta existente após validar o pet e a vacina associada, garantindo a integridade dos relacionamentos.",
        OperationId = "Alerts_Update"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, AlertCreateDto dto)
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
            message = "Alerta atualizado com sucesso"
        });
    }

    /// <summary>
    /// Exclui um alerta pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do alerta.</param>
    /// <response code="200">Alerta excluído.</response>
    /// <response code="401">Autenticação necessária.</response>
    /// <response code="404">Alerta não encontrado.</response>
    [Authorize]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Excluir alerta",
        Description = "Remove um alerta existente a partir de seu identificador. Retorna 404 quando o alerta informado não existe.",
        OperationId = "Alerts_Delete"
    )]
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
                message = "Alerta não encontrado."
            });
        }

        await _service.DeleteAsync(id);

        return Ok(new
        {
            message = "Alerta removido com sucesso."
        });
    }
}