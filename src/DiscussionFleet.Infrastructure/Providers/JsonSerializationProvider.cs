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


    public void Serialize<T>(T complexData, bool isCaseSensitive = true)
    {
        JsonSerializer.Serialize(complexData, options: isCaseSensitive ? DefaultOptions : CaseInsensitiveOptions);
    }

    public async Task SerializeAsync<T>(Stream stream, T complexData, bool isCaseSensitive = true,
        CancellationToken cancellationToken = default)
    {
        await JsonSerializer.SerializeAsync(
            stream, complexData,
            options: isCaseSensitive ? DefaultOptions : CaseInsensitiveOptions,
            cancellationToken: cancellationToken);
    }

    public async Task<T?> DeserializeAsync<T>(Stream simpleData, bool isCaseSensitive = true,
        CancellationToken cancellationToken = default)
    {
        return await JsonSerializer.DeserializeAsync<T>(
            simpleData,
            options: isCaseSensitive ? DefaultOptions : CaseInsensitiveOptions,
            cancellationToken: cancellationToken
        );
    }
}