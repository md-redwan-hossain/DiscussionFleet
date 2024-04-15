using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.MembershipFeatures.Utils;
using Hangfire;

namespace DiscussionFleet.EmailWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ICloudQueueService _cloudQueueService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IEmailService _emailService;
    private readonly IJsonSerializationService _jsonSerializationService;


    public Worker(ILogger<Worker> logger, ICloudQueueService cloudQueueService,
        IBackgroundJobClient backgroundJobClient, IEmailService emailService,
        IJsonSerializationService jsonSerializationService)
    {
        _logger = logger;
        _cloudQueueService = cloudQueueService;
        _backgroundJobClient = backgroundJobClient;
        _emailService = emailService;
        _jsonSerializationService = jsonSerializationService;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            var data = await _cloudQueueService.PullAsync(5);

            foreach (var (receiptHandle, body) in data)
            {
                var confirmationEmail = _jsonSerializationService.DeSerialize<MemberConfirmationEmail>(body);

                if (confirmationEmail is null) continue;

                _backgroundJobClient.Enqueue(() =>
                    SendVerificationMailAsync(confirmationEmail.FullName, confirmationEmail.Email,
                        confirmationEmail.VerificationCode));
                await _cloudQueueService.DeleteAsync(receiptHandle);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }


    // ReSharper disable once MemberCanBePrivate.Global
    public async Task SendVerificationMailAsync(string fullName, string email, string verificationCode)
    {
        const string subject = "Account Confirmation";

        var body = $"""
                    <html>
                        <body>
                            <h2>Welcome, {fullName}!</h2>
                            <p>Thanks for signing up. Please verify your email address by using the following verification code:</p>
                            <h2>{verificationCode}</h2>
                            <p>If you didn't request this, you can safely ignore this email.</p>
                            <p>Best Regards,</p>
                            <p>DiscussionFleet</p>
                        </body>
                    </html>
                    """;

        await _emailService.SendSingleEmailAsync(fullName, email, subject, body).ConfigureAwait(false);
    }
}