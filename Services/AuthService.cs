using Microsoft.JSInterop;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public string? Token { get; private set; }

    public AuthService(IHttpClientFactory factory, IJSRuntime js)
    {
        _http = factory.CreateClient("GloboClimaApi");
        _js = js;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await _http.PostAsJsonAsync("auth/login", new { Username = username, Password = password });

        if (!response.IsSuccessStatusCode)
            return false;

        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        Token = result?.Token;

        if (Token != null)
        {
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            await _js.InvokeVoidAsync("sessionStorageHelper.set", "authToken", Token);
            await _js.InvokeVoidAsync("sessionStorageHelper.set", "username", username);
            return true;
        }

        return false;
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        var response = await _http.PostAsJsonAsync("auth/register", new { Username = username, Password = password });
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> GetTokenAsync()
    {
        if (Token != null)
            return Token;

        Token = await _js.InvokeAsync<string>("sessionStorageHelper.get", "authToken");

        if (!string.IsNullOrWhiteSpace(Token))
        {
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
        }

        return Token;
    }

    public async Task LogoutAsync()
    {
        Token = null;
        _http.DefaultRequestHeaders.Authorization = null;
        await _js.InvokeVoidAsync("sessionStorageHelper.remove", "username");
    }

    private class AuthResult
    {
        public string Token { get; set; } = string.Empty;
    }
}
