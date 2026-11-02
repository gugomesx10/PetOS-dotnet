using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetOS.Dto.Auth;
using PetOS.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace PetOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Cadastra um usuário.
    /// </summary>
    /// <remarks>
    /// Cria uma conta com nome, e-mail e senha.
    /// O e-mail deve ser único e a senha é armazenada como hash.
    /// </remarks>
    /// <response code="201">Usuário cadastrado com sucesso.</response>
    /// <response code="400">Dados de cadastro inválidos.</response>
    /// <response code="409">E-mail já cadastrado.</response>
    [AllowAnonymous]
    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Cadastrar usuário",
        Description = "Cria uma conta no PetOS. Valida os dados informados, impede e-mails duplicados e armazena a senha de forma protegida.",
        OperationId = "Auth_Register"
    )]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto request)
    {
        var id = await _authService.RegisterAsync(request);

        if (id == null)
        {
            return Conflict(new
            {
                message = "Este e-mail já está cadastrado."
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                message = "Usuário cadastrado com sucesso.",
                id
            }
        );
    }

    /// <summary>
    /// Autentica um usuário.
    /// </summary>
    /// <remarks>
    /// Valida as credenciais e retorna um token JWT
    /// para acesso aos endpoints protegidos.
    /// </remarks>
    /// <response code="200">Login realizado e token JWT gerado.</response>
    /// <response code="400">Dados de login inválidos.</response>
    /// <response code="401">Credenciais incorretas.</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Autenticar usuário",
        Description = "Valida o e-mail e a senha do usuário. Quando as credenciais são válidas, retorna um token JWT e sua data de expiração.",
        OperationId = "Auth_Login"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);

        if (response == null)
        {
            return Unauthorized(new
            {
                message = "E-mail ou senha inválidos."
            });
        }

        return Ok(response);
    }

    /// <summary>
    /// Consulta o usuário autenticado.
    /// </summary>
    /// <remarks>
    /// Retorna os dados de identificação presentes no token JWT.
    /// Requer autenticação.
    /// </remarks>
    /// <response code="200">Dados do usuário autenticado.</response>
    /// <response code="401">Token ausente, inválido ou expirado.</response>
    [Authorize]
    [HttpGet("me")]
    [SwaggerOperation(
        Summary = "Consultar usuário autenticado",
        Description = "Retorna o identificador, o nome e o e-mail do usuário identificado pelo token JWT enviado na requisição.",
        OperationId = "Auth_GetCurrentUser"
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        return Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            name = User.Identity?.Name,
            email = User.FindFirstValue(ClaimTypes.Email)
        });
    }
}