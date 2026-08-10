namespace Application.Features.Auth.Logout;

/// <summary>
/// Token de renovação a ser revogado.
/// </summary>
public record LogoutRequest
{
    /// <summary>
    /// Refresh token da sessão a ser encerrada.
    /// </summary>
    /// <example>Zm9vYmFyLXJlZnJlc2gtdG9rZW4tZXhhbXBsZQ==</example>
    public string RefreshToken { get; init; } = string.Empty;
}
