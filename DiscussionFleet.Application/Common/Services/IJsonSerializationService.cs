namespace DiscussionFleet.Application.Common.Services;

public interface IJsonSerializationService
{
    string Serialize<T>(T complexData, bool isCaseSensitive = true);
    T? DeSerialize<T>(string jsonData, bool isCaseSensitive = true);
}