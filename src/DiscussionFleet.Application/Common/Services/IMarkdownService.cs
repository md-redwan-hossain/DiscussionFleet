namespace DiscussionFleet.Application.Common.Services;

public interface IMarkdownService
{
    Task<string> MarkdownToHtml(string markdownText);
    
    Task<string> MarkdownToPlainText(string markdownText);
    Task<string> SanitizeConvertedHtml(string unsafeHtmlText);
}