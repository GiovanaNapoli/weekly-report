namespace Application.Features.Auth.Login;

/// <summary>
/// Credenciais para autenticação.
/// </summary>
public record LoginRequest
{
    /// <summary>
    /// E-mail cadastrado do usuário.
    /// </summary>
    /// <example>giovana@email.com</example>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Senha do usuário.
    /// </summary>
    /// <example>MinhaSenha123</example>
    public string Password { get; init; } = string.Empty;
}
