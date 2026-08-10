namespace Application.Features.Auth.Me;

/// <summary>
/// Dados do usuário autenticado.
/// </summary>
public record MeResponse
{
    /// <summary>
    /// Identificador único do usuário.
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// E-mail do usuário autenticado.
    /// </summary>
    /// <example>giovana@email.com</example>
    public string Email { get; init; } = string.Empty;
}
