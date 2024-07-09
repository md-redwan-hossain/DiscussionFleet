namespace DiscussionFleet.Application.Common.Services;

public interface ICloudQueueService
{
    Task EnqueueAsync(string jsonPayload, string messageDeduplicationId, string messageGroupId = "DiscussionFleet");
    Task<ICollection<(string receiptHandle, string body)>> PullAsync(int maxData);
    Task DeleteAsync(string receiptHandle);
    Task<string> GetFifoQueueUrl(string queueName);
}