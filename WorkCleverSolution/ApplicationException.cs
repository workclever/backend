namespace WorkCleverSolution;

public class ApplicationException : Exception
{
    public ApplicationException(string message)
        : base(message)
    {
    }
}