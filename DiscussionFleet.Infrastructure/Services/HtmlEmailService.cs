using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DiscussionFleet.Infrastructure.Services;

public class HtmlEmailService : IEmailService
{
    private readonly SmtpOptions _emailSettings;

    public HtmlEmailService(IOptions<SmtpOptions> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }


    public async Task SendSingleEmailAsync(string receiverName, string receiverEmail, string subject, string body)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));

        message.To.Add(new MailboxAddress(receiverName, receiverEmail));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
        client.Timeout = 30000;
        await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}