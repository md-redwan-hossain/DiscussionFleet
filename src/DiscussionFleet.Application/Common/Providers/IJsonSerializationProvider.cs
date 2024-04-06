namespace DiscussionFleet.Application.Common.Providers;

public interface IJsonSerializationProvider
{
    string Serialize<T>(T complexData, bool isCaseSensitive = true);
    T? DeSerialize<T>(string jsonData, bool isCaseSensitive = true);
}