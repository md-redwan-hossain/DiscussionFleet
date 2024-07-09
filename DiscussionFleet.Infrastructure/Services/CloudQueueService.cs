using Amazon.SQS;
using Amazon.SQS.Model;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Services;
using Microsoft.Extensions.Options;

namespace DiscussionFleet.Infrastructure.Services;

public class CloudQueueService : ICloudQueueService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly CloudQueueOptions _cloudQueueOptions;

    public CloudQueueService(IAmazonSQS sqsClient, IOptions<CloudQueueOptions> cloudQueueOptions)
    {
        _sqsClient = sqsClient;
        _cloudQueueOptions = cloudQueueOptions.Value;
    }

    public async Task EnqueueAsync(string jsonPayload, string messageDeduplicationId,
        string messageGroupId = "DiscussionFleet")
    {
        var queueUrl = await GetFifoQueueUrl(_cloudQueueOptions.FifoQueueName);
        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = jsonPayload,
            MessageGroupId = messageGroupId,
            MessageDeduplicationId = messageDeduplicationId
        };

        await _sqsClient.SendMessageAsync(sendMessageRequest);
    }

    public async Task<ICollection<(string receiptHandle, string body)>> PullAsync(int maxData)
    {
        if (maxData < 0) maxData = 1;
        if (maxData > 10) maxData = 10;

        var queueUrl = await GetFifoQueueUrl(_cloudQueueOptions.FifoQueueName);
        var receiveRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            MaxNumberOfMessages = maxData
        };
        var receiveMessageResponse = await _sqsClient.ReceiveMessageAsync(receiveRequest);

        ICollection<(string receiptHandle, string body)> data = [];

        foreach (var message in receiveMessageResponse.Messages)
        {
            data.Add((message.ReceiptHandle, message.Body));
        }

        return data;
    }

    public async Task DeleteAsync(string receiptHandle)
    {
        var queueUrl = await GetFifoQueueUrl(_cloudQueueOptions.FifoQueueName);
        var deleteMessageRequest = new DeleteMessageRequest
        {
            QueueUrl = queueUrl,
            ReceiptHandle = receiptHandle
        };

        await _sqsClient.DeleteMessageAsync(deleteMessageRequest);
    }


    public async Task<string> GetFifoQueueUrl(string queueName)
    {
        if (queueName.EndsWith(".fifo") is false)
        {
            queueName = $"{queueName}.fifo";
        }

        try
        {
            var response = await _sqsClient.GetQueueUrlAsync(queueName);
            return response.QueueUrl;
        }
        catch (QueueDoesNotExistException)
        {
            var createQueueRequest = new CreateQueueRequest
            {
                QueueName = queueName,
                Attributes = new Dictionary<string, string>
                {
                    { QueueAttributeName.FifoQueue, "true" },
                    { QueueAttributeName.VisibilityTimeout, "30" },
                    { QueueAttributeName.DelaySeconds, "0" },
                    { QueueAttributeName.ReceiveMessageWaitTimeSeconds, "5" }
                }
            };
            var response = await _sqsClient.CreateQueueAsync(createQueueRequest);
            return response.QueueUrl;
        }
    }
}