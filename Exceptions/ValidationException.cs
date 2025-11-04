namespace ContosoPizza.Exceptions;

public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException(List<string> errors) 
        : base("验证失败")
    {
        Errors = errors;
    }
}