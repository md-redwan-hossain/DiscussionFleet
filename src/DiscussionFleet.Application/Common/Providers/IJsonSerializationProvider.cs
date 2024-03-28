namespace DiscussionFleet.Application.Common.Providers;

public interface IJsonSerializationProvider
{
    void Serialize<T>(T complexData, bool isCaseSensitive = true);

    Task SerializeAsync<T>(Stream stream, T complexData, bool isCaseSensitive = true,
        CancellationToken cancellationToken = default);

    Task<T?> DeserializeAsync<T>(Stream simpleData, bool isCaseSensitive = true,
        CancellationToken cancellationToken = default);
}