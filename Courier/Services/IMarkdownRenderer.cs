namespace Courier.Services;

public interface IMarkdownRenderer
{
    Task<string> ToHtmlAsync(string source);
}