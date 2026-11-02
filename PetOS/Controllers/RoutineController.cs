using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Routine;
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

    public RoutineController(
        IRoutineService service,
        IPetService petService)
    {
        _service = service;
        _petService = petService;
    }

    /// <summary>
    /// Lista todas as rotinas cadastradas.
    /// </summary>
    /// <response code="200">Rotinas encontradas com sucesso.</response>
    /// <response code="204">Nenhuma rotina cadastrada.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar todas as rotinas",
        Description = "Consulta os registros de rotinas de cuidados cadastrados no PetOS.",
        OperationId = "Routines_GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetAll()
    {
        var routines = await _service.GetAllAsync();

        if (!routines.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Rotinas encontradas com sucesso",
            data = routines
        });
    }

    /// <summary>
    /// Consulta uma rotina pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da rotina.</param>
    /// <response code="200">Rotina encontrada com sucesso.</response>
    /// <response code="404">Rotina não encontrada.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Buscar rotina por ID",
        Description = "Retorna os dados de uma rotina específica.",
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

        return Ok(new
        {
            message = "Rotina encontrada com sucesso",
            data = routine
        });
    }

    /// <summary>
    /// Lista as rotinas de cuidados de um pet.
    /// </summary>
    /// <remarks>
    /// Verifica a existência do animal e consulta suas rotinas cadastradas.
    /// </remarks>
    /// <param name="petId">Identificador do pet.</param>
    /// <response code="200">Rotinas encontradas com sucesso.</response>
    /// <response code="204">O pet não possui rotinas cadastradas.</response>
    /// <response code="404">Pet não encontrado.</response>
    [HttpGet("pet/{petId}")]
    [SwaggerOperation(
        Summary = "Listar rotinas de um pet",
        Description = "Consulta os cuidados e atividades registrados para um animal.",
        OperationId = "Routines_GetByPetId")]
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

        var routines = await _service.GetByPetIdAsync(petId);

        if (!routines.Any())
        {
            return NoContent();
        }

        return Ok(new
        {
            message = "Rotinas encontradas com sucesso",
            data = routines
        });
    }

    /// <summary>
    /// Cadastra uma rotina de cuidados.
    /// </summary>
    /// <remarks>
    /// Registra uma rotina associada a um animal existente.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="dto">Dados da rotina e identificador do pet.</param>
    /// <response code="201">Rotina cadastrada com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Pet não encontrado.</response>
    [Authorize]
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastrar rotina",
        Description = "Registra uma nova atividade ou cuidado associado a um pet.",
        OperationId = "Routines_Create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(RoutineCreateDto dto)
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
                message = "Rotina criada com sucesso",
                data = created
            });
    }

    /// <summary>
    /// Atualiza uma rotina cadastrada.
    /// </summary>
    /// <remarks>
    /// Verifica se a rotina e o pet existem antes de realizar a atualização.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="id">Identificador da rotina.</param>
    /// <param name="dto">Dados atualizados da rotina.</param>
    /// <response code="200">Rotina atualizada com sucesso.</response>
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
    public async Task<IActionResult> Update(long id, RoutineCreateDto dto)
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
            message = "Rotina atualizada com sucesso"
        });
    }

    /// <summary>
    /// Exclui uma rotina cadastrada.
    /// </summary>
    /// <remarks>
    /// Remove uma rotina pelo identificador.
    /// Requer autenticação por token JWT.
    /// </remarks>
    /// <param name="id">Identificador da rotina.</param>
    /// <response code="200">Rotina excluída com sucesso.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Rotina não encontrada.</response>
    [Authorize]
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Excluir rotina",
        Description = "Remove um registro de rotina de cuidados do sistema.",
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
            message = "Rotina excluido com sucesso"
        });
    }
}