namespace Application.Features.Auth.Refresh;

/// <summary>
/// Token de renovação utilizado para obter um novo par de tokens.
/// </summary>
public record RefreshRequest
{
    /// <summary>
    /// Refresh token obtido em um login (ou refresh) anterior.
    /// </summary>
    /// <example>Zm9vYmFyLXJlZnJlc2gtdG9rZW4tZXhhbXBsZQ==</example>
    public string RefreshToken { get; init; } = string.Empty;
}
