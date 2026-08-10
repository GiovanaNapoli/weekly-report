using System.Security.Claims;
using Application.Common;
using Application.Features.Auth;
using Application.Features.Auth.Login;
using Application.Features.Auth.Logout;
using Application.Features.Auth.Me;
using Application.Features.Auth.Refresh;
using Application.Features.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/auth")]
public class AuthController : BaseController
{
    private IMediator _mediator;

    public AuthController (IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cadastra um novo usuário no sistema.
    /// </summary>
    /// <remarks>
    /// O e-mail informado deve ser único no sistema. A senha deve ter pelo menos 8 caracteres,
    /// 1 letra maiúscula e 1 dígito (política aplicada pelo ASP.NET Core Identity). O cadastro
    /// não efetua login automático — use <c>POST /api/auth/login</c> em seguida.
    /// </remarks>
    /// <param name="request">Nome, e-mail e senha do novo usuário.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">Usuário criado com sucesso.</response>
    /// <response code="400">E-mail já cadastrado ou dados inválidos (ex: senha fora da política).</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new RegisterCommand(request.Name, request.Email, request.Password), ct));

    /// <summary>
    /// Autentica um usuário e emite um par de tokens (access + refresh).
    /// </summary>
    /// <remarks>
    /// Por segurança, a mensagem de erro é sempre genérica ("e-mail ou senha inválidos") —
    /// não diferencia "e-mail não cadastrado" de "senha incorreta", para não revelar quais
    /// e-mails existem no sistema.
    /// </remarks>
    /// <param name="request">E-mail e senha cadastrados.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">Autenticado com sucesso; retorna access token e refresh token.</response>
    /// <response code="401">E-mail ou senha inválidos.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ResponseBase<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        => HandleResult(
            await _mediator.Send(new LoginCommand(request.Email, request.Password), ct),
            StatusCodes.Status401Unauthorized);

    /// <summary>
    /// Troca um refresh token válido por um novo par de tokens.
    /// </summary>
    /// <remarks>
    /// O refresh token usado nesta chamada é revogado (rotação): não pode ser reutilizado depois
    /// deste refresh, mesmo que ainda não tenha expirado. Use o novo refresh token retornado para
    /// a próxima renovação.
    /// </remarks>
    /// <param name="request">Refresh token obtido em um login (ou refresh) anterior.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">Novo par de tokens emitido com sucesso.</response>
    /// <response code="401">Refresh token inválido, expirado ou já revogado.</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ResponseBase<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
        => HandleResult(
            await _mediator.Send(new RefreshTokenCommand(request.RefreshToken), ct),
            StatusCodes.Status401Unauthorized);

    /// <summary>
    /// Encerra a sessão associada a um refresh token, revogando-o.
    /// </summary>
    /// <param name="request">Refresh token da sessão a ser encerrada.</param>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">Logout realizado com sucesso.</response>
    /// <response code="400">Refresh token inválido ou já revogado.</response>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
        => HandleResult(await _mediator.Send(new LogoutCommand(request.RefreshToken), ct));

    /// <summary>
    /// Retorna os dados do usuário autenticado.
    /// </summary>
    /// <remarks>
    /// Requer um access token válido no header <c>Authorization: Bearer</c>. Busca os dados
    /// atuais do usuário no banco a partir do identificador contido no token (não confia
    /// apenas nas claims, que podem estar desatualizadas em relação ao usuário real).
    /// </remarks>
    /// <param name="ct">Token de cancelamento da requisição.</param>
    /// <response code="200">Dados do usuário autenticado.</response>
    /// <response code="401">
    /// Token ausente ou inválido (resposta sem corpo, tratada pelo middleware de autenticação
    /// antes da action rodar) ou usuário do token não existe mais (resposta com corpo).
    /// </response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ResponseBase<MeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
        var result = await _mediator.Send(new GetCurrentUserQuery(userId), ct);

        return HandleResult(result, StatusCodes.Status401Unauthorized);
    }
}
