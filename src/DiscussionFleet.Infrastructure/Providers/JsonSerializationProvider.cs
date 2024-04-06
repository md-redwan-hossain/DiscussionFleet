using System.Text.Json;
using System.Text.Json.Serialization;
using DiscussionFleet.Application.Common.Providers;

namespace DiscussionFleet.Infrastructure.Providers;

public class JsonSerializationProvider : IJsonSerializationProvider
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = false,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    private static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };


    public string Serialize<T>(T complexData, bool isCaseSensitive = true)
    {
        return JsonSerializer.Serialize(complexData,
            options: isCaseSensitive ? DefaultOptions : CaseInsensitiveOptions
        );
    }


    public T? DeSerialize<T>(string jsonData, bool isCaseSensitive = true)
    {
        return JsonSerializer.Deserialize<T>(jsonData,
            options: isCaseSensitive ? DefaultOptions : CaseInsensitiveOptions
        );
    }
}