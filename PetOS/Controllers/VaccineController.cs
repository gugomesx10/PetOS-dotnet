using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Vaccine;
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

    public VaccineController(
        IVaccineService service,
        IPetService petService)
    {
        _service = service;
        _petService = petService;
    }

    /// <summary>
    /// Lista todas as vacinas cadastradas.
    /// </summary>
    /// <response code="200">Vacinas encontradas com sucesso.</response>
    /// <response code="204">Nenhuma vacina cadastrada.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar todas as vacinas",
        Description = "Consulta todos os registros de vacinação disponíveis no PetOS.",
        OperationId = "Vaccines_GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAll()
    {
        var vaccines = await _service.GetAllAsync();

        if (!vaccines.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Vacinas encontradas com sucesso",
            data = vaccines
        });
    }

    /// <summary>
    /// Consulta uma vacina pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da vacina.</param>
    /// <response code="200">Vacina encontrada com sucesso.</response>
    /// <response code="404">Vacina não encontrada.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Buscar vacina por ID",
        Description = "Retorna os dados de uma vacina a partir do identificador informado.",
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

        return Ok(new
        {
            message = "Vacina encontrada com sucesso",
            data = vaccine
        });
    }

    /// <summary>
    /// Lista as vacinas de um pet.
    /// </summary>
    /// <remarks>
    /// Verifica se o animal existe e consulta seus registros de vacinação.
    /// </remarks>
    /// <param name="petId">Identificador do pet.</param>
    /// <response code="200">Vacinas encontradas com sucesso.</response>
    /// <response code="204">O pet não possui vacinas cadastradas.</response>
    /// <response code="404">Pet não encontrado.</response>
    [HttpGet("pet/{petId}")]
    [SwaggerOperation(
        Summary = "Listar vacinas de um pet",
        Description = "Consulta o histórico de vacinação de um animal específico.",
        OperationId = "Vaccines_GetByPetId")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPetId(long petId)
    {
        var pet = await _petService.GetByIdAsync(petId);

        if (pet == null)
        {
            return NotFound(new
            {
                message = "Pet não encontrado"
            });
        }

        var vaccines = await _service.GetByPetIdAsync(petId);

        if (!vaccines.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Vacinas encontradas com sucesso",
            data = vaccines
        });
    }

    /// <summary>
    /// Registra uma vacina para um pet.
    /// </summary>
    /// <remarks>
    /// Verifica a existência do animal antes de registrar a vacinação.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="dto">Dados da vacina e identificador do pet.</param>
    /// <response code="201">Vacina cadastrada com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Pet não encontrado.</response>
    [Authorize]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastrar vacina",
        Description = "Registra uma vacina associada a um animal existente.",
        OperationId = "Vaccines_Create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(VaccineCreateDto dto)
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
            new
            {
                message = "Vacina adicionada com sucesso",
                data = created
            });
    }

    /// <summary>
    /// Atualiza um registro de vacinação.
    /// </summary>
    /// <remarks>
    /// Verifica se a vacina e o pet informado existem antes da atualização.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="id">Identificador da vacina.</param>
    /// <param name="dto">Dados atualizados da vacinação.</param>
    /// <response code="200">Vacina atualizada com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Vacina ou pet não encontrado.</response>
    [Authorize]
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualizar vacina",
        Description = "Altera as informações de uma vacina já cadastrada.",
        OperationId = "Vaccines_Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, VaccineCreateDto dto)
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
            message = "Vacina atualizada com sucesso"
        });
    }

    /// <summary>
    /// Exclui um registro de vacinação.
    /// </summary>
    /// <remarks>
    /// Remove uma vacina cadastrada.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="id">Identificador da vacina.</param>
    /// <response code="200">Vacina excluída com sucesso.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Vacina não encontrada.</response>
    [Authorize]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Excluir vacina",
        Description = "Remove um registro de vacinação pelo identificador.",
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
            message = "Vacina removida com sucesso"
        });
    }
}