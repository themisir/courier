using Markdig;

namespace Courier.Services;

public class MarkdownRenderer : IMarkdownRenderer
{
    private readonly MarkdownPipeline _markdownPipeline;

    public MarkdownRenderer()
    {
        _markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
    }
        
    public Task<string> ToHtmlAsync(string source)
    {
        return Task.FromResult(Markdown.ToHtml(source, _markdownPipeline));
    }
}