using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace LocalizationDemo.BlazorFrontend.Services;

public class LanguageService
{
    private readonly IJSRuntime _jsRuntime;
    private string _currentLanguage = "en-US";

    public event Action<string>? LanguageChanged;

    public LanguageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string CurrentLanguage => _currentLanguage;

    public async Task SetLanguageAsync(string language)
    {
        _currentLanguage = language;
        LanguageChanged?.Invoke(language);
        
        // Store in localStorage for persistence
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "selectedLanguage", language);
        }
        catch
        {
            // Ignore localStorage errors in case it's not available
        }
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Try to load saved language from localStorage
            var savedLanguage = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "selectedLanguage");
            if (!string.IsNullOrEmpty(savedLanguage) && SupportedLanguages.ContainsKey(savedLanguage))
            {
                _currentLanguage = savedLanguage;
            }
        }
        catch
        {
            // Ignore localStorage errors, use default language
        }
    }

    public void ConfigureHttpClient(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        httpClient.DefaultRequestHeaders.AcceptLanguage.Add(
            new StringWithQualityHeaderValue(_currentLanguage));
    }

    public static readonly Dictionary<string, string> SupportedLanguages = new()
    {
        { "en-US", "English" },
        { "hu", "Magyar" }
    };
}