namespace Application.Features.Auth.Register;

/// <summary>
/// Dados necessários para cadastrar um novo usuário.
/// </summary>
public record RegisterRequest
{
    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    /// <example>Giovana Napoli</example>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// E-mail utilizado para autenticação. Deve ser único no sistema.
    /// </summary>
    /// <example>giovana@email.com</example>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Senha utilizada para autenticação. Mínimo de 8 caracteres, pelo menos
    /// 1 letra maiúscula e 1 dígito.
    /// </summary>
    /// <example>MinhaSenha123</example>
    public string Password { get; init; } = string.Empty;
}
