namespace DiscussionFleet.Application.Common.Services;

public interface IMarkdownService
{
    Task<string> MarkdownToHtmlAsync(string markdownText);
    
    Task<string> MarkdownToPlainTextAsync(string markdownText);
    Task<string> SanitizeConvertedHtmlAsync(string unsafeHtmlText);
}