using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlightSchedule.Api.IntegrationTests.Infrastructure;

public static class AppJsonHelper
{
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        
    };
    public static StringContent ToAppJsonContext(this object obj, JsonSerializerOptions? options = null) => new (JsonSerializer.Serialize(obj, options??DefaultJsonSerializerOptions), Encoding.UTF8, "application/json");

    public static T? FromAppJson<T>(this string jsonString, JsonSerializerOptions? options = null) => JsonSerializer.Deserialize<T>(jsonString, options??DefaultJsonSerializerOptions);

    public static async Task<T?> FromAppJsonAsync<T>(this HttpContent content, JsonSerializerOptions? options = null) => FromAppJson<T>(await content.ReadAsStringAsync(), options);
}