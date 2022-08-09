using System.Text;
// using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddHttpClient();
        services.AddTransient<AzureTranslator>();
    })
    .Build();

try
{
    var azureTranslator = host.Services.GetRequiredService<AzureTranslator>();
    var results = await azureTranslator.TranslateTextAsync();
    Console.WriteLine($"Requested {results?.Count() ?? 0} translation:");
    if (results is not null)
    {
        foreach (var r in results)
        {
            Console.WriteLine($"\tReturned {r.Translations?.Count() ?? 0} texts:");
            if (r.Translations is not null)
            {
                foreach (var s in r.Translations)
                {
                    Console.WriteLine("{0}, {1}", s.To, s.Text);
                }
            }
        }
    }
}
catch (Exception ex)
{
    host.Services.GetRequiredService<ILogger<Program>>()
        .LogError(ex, "Unable to load branches from GitHub.");
}

public class AzureTranslator
{
    private static readonly string? subscriptionKey = System.Environment.GetEnvironmentVariable("SUBSCRIPTION_KEY");
    private static readonly string region = "eastus2";
    private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
    private readonly IHttpClientFactory _httpClientFactory;
    public AzureTranslator(IHttpClientFactory httpClientFactory) =>
        _httpClientFactory = httpClientFactory;
    public async Task<IEnumerable<Results>?> TranslateTextAsync()
    {
        string route = "/translate?api-version=3.0&from=en&to=ar&to=it";
        string textToTranslate = "Hello, world!";
        object[] body = new object[] { new { Text = textToTranslate } };
        var requestBody = JsonSerializer.Serialize(body);
        var httpClient = _httpClientFactory.CreateClient();
        using (var request = new HttpRequestMessage())
        {
            // Build the request.
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(endpoint + route);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("Accept-Encoding", "UTF-8");
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", region);

            // Send the request and get response.
            HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync();

            return await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<Results>?>(contentStream);
        }

    }
}

public record Results(
    [property : JsonPropertyName ("translations")]
    Translation[] Translations
);

public record Translation(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("to")] string To
);