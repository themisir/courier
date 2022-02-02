namespace Courier.Models;

public abstract class MessageResponse
{
    public class MessageContents
    {
        public MessageContents(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }

    public static SuccessMessageResponse Success(string message) => new(message);
    public static ErrorMessageResponse Error(string message) => new(message);
}

public class SuccessMessageResponse : MessageResponse
{
    public SuccessMessageResponse(string message) 
    {
        Success = new MessageContents(message);
    }

    public new MessageContents Success { get; set; }
}
    
public class ErrorMessageResponse : MessageResponse
{
    public ErrorMessageResponse(string message) 
    {
        Error = new MessageContents(message);
    }

    public new MessageContents Error { get; set; }
}