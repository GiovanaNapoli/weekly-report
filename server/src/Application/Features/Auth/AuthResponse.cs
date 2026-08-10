namespace Application.Features.Auth;

/// <summary>
/// Par de tokens emitido após autenticação bem-sucedida.
/// </summary>
public record AuthResponse
{
    /// <summary>
    /// JWT de acesso, enviado no header <c>Authorization: Bearer</c> das próximas requisições.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.dozjgNryP4J3jVmNHl0w5N_XgL0n3I9PlFUP0THsR8U</example>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Token opaco usado para obter um novo access token sem exigir login novamente.
    /// </summary>
    /// <example>Zm9vYmFyLXJlZnJlc2gtdG9rZW4tZXhhbXBsZQ==</example>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>
    /// Data/hora (UTC) em que o access token expira.
    /// </summary>
    /// <example>2026-08-10T15:30:00Z</example>
    public DateTime ExpiresAt { get; init; }
}
