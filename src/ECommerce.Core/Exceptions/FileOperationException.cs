namespace ECommerce.Core.Exceptions;

public class FileOperationException : Exception
{
    public FileOperationException(string message) : base(message)
    {
    }

    public FileOperationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}