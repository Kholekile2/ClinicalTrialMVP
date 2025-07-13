using System.Net.Http.Headers;
using System.Text.Json;

namespace ClinicalTrial2._0.Services
{
    /// <summary>
    /// Interface for translation services
    /// </summary>
    public interface ITranslationService
    {
        Task<string?> TranslateTextAsync(string inputText, string targetLanguage);
    }

    /// <summary>
    /// Service for translating text using Microsoft Azure Cognitive Services Translator
    /// Supports multilingual trial descriptions for accessibility
    /// </summary>
    public class TranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _subscriptionKey;
        private readonly string _endpoint;

        public TranslationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            
            // Get configuration from appsettings
            _subscriptionKey = _configuration["AzureTranslator:SubscriptionKey"] ?? "9fbcf417abc545e183727944343a0d06";
            _endpoint = _configuration["AzureTranslator:Endpoint"] ?? "https://api.cognitive.microsofttranslator.com/";
        }

        /// <summary>
        /// Translates input text to the specified target language
        /// </summary>
        /// <param name="inputText">Text to translate</param>
        /// <param name="targetLanguage">Target language code (e.g., "xh", "zu", "af")</param>
        /// <returns>Translated text or null if translation fails</returns>
        public async Task<string?> TranslateTextAsync(string inputText, string targetLanguage)
        {
            if (string.IsNullOrEmpty(inputText) || string.IsNullOrEmpty(targetLanguage))
                return null;

            try
            {
                string route = $"/translate?api-version=3.0&to={targetLanguage}";
                object[] body = new object[] { new { Text = inputText } };
                var requestBody = new StringContent(JsonSerializer.Serialize(body));
                requestBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", "eastus");

                var response = await _httpClient.PostAsync(_endpoint + route, requestBody);
                
                if (!response.IsSuccessStatusCode)
                    return null;

                string result = await response.Content.ReadAsStringAsync();
                
                using (JsonDocument doc = JsonDocument.Parse(result))
                {
                    var translations = doc.RootElement[0].GetProperty("translations");
                    var translation = translations[0].GetProperty("text").GetString();
                    return translation;
                }
            }
            catch (Exception ex)
            {
                // Log the exception in a real application
                Console.WriteLine($"Translation error: {ex.Message}");
                return null;
            }
        }
    }
}
