using System.Text.Json;

namespace POS.Api.Services;

public class KeycloakServiceTokenProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private string? _token;
    private DateTime _expiresAtUtc = DateTime.MinValue;

    public KeycloakServiceTokenProvider(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public async Task<string> GetTokenAsync(CancellationToken ct = default)
    {
        if (_token is not null && DateTime.UtcNow < _expiresAtUtc)
            return _token;

        await _lock.WaitAsync(ct);
        try
        {
            // Double-check: another request may have refreshed while we waited
            if (_token is not null && DateTime.UtcNow < _expiresAtUtc)
                return _token;

            var client = _httpClientFactory.CreateClient("keycloak");
            var response = await client.PostAsync(
                _config["Keycloak:TokenEndpoint"],
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = _config["Keycloak:ClientId"]!,
                    ["client_secret"] = _config["Keycloak:ClientSecret"]!
                }), ct);

            response.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
            _token = doc.RootElement.GetProperty("access_token").GetString()!;
            var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();

            // Refresh 30s early so a token never expires mid-call
            _expiresAtUtc = DateTime.UtcNow.AddSeconds(expiresIn - 30);
            return _token;
        }
        finally
        {
            _lock.Release();
        }
    }
}