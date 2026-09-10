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
    /// Lista todos os alertas
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Lista todos os alertas")]
    [ProducesResponseType((StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status204NoContent))]
    public async Task<IActionResult> GetAll()
    {
        var alerts = await _service.GetAllAsync();

        if (!alerts.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Alertas encontradas com sucesso",
            data = alerts
        });
    }

    /// <summary>
    /// Busca alerta por Id
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Realiza busca do alerta por id")]
    [ProducesResponseType((StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status404NotFound))]
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
    /// Busca alertas não lidos
    /// </summary>
    [HttpGet("unread")]
    [SwaggerOperation(Summary = "Busca alertas não lidos")]
    [ProducesResponseType((StatusCodes.Status200OK))]
    [ProducesResponseType((StatusCodes.Status204NoContent))]
    public async Task<IActionResult> GetUnread()
    {
        var alerts = await _service.GetUnreadAsync();

        if (!alerts.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Alertas encontradas com sucesso",
            data = alerts
        });
    }

    /// <summary>
    /// Cadastra um novo alerta
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Cadastra um novo alerta no banco")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            var vaccine = await _vaccineService.GetByIdAsync(dto.VaccineId.Value);

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
            });
    }

    /// <summary>
    /// Atualiza alerta por id
    /// </summary>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualiza alerta")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            var vaccine = await _vaccineService.GetByIdAsync(dto.VaccineId.Value);

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
    /// Remove alerta por id
    /// </summary>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Excluir um alerta no banco")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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