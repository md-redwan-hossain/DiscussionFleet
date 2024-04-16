using DiscussionFleet.Application.Common.Services;
using Ganss.Xss;
using Markdig;

namespace DiscussionFleet.Infrastructure.Services;

public class MarkdownService : IMarkdownService
{
    private static readonly HtmlSanitizer Sanitizer = new();

    private static readonly MarkdownPipeline Pipeline =
        new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    public Task<string> MarkdownToHtml(string markdownText)
    {
        return Task.Run(() => Markdown.ToHtml(markdownText, Pipeline));
    }

    public Task<string> MarkdownToPlainText(string markdownText)
    {
        return Task.Run(() => Markdown.ToPlainText(markdownText, Pipeline));
    }

    public Task<string> SanitizeConvertedHtml(string unsafeHtmlText)
    {
        return Task.Run(() => Sanitizer.Sanitize(unsafeHtmlText));
    }
}