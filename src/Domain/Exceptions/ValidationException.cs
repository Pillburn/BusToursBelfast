namespace ToursApp.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }
        public ValidationException()
            : base("One or more validation failures have occurred")
        {
             Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message) : base(message)
        {
             Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message, Exception innerException)
            :base(message, innerException)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string name, object key) 
            : base($"Entity'{name}' ({key}) was not found.")
        {
            Errors = new Dictionary<string, string[]>();
        }
        public ValidationException(IDictionary<string, string[]> errors)
            : base("One or more validation failures have occurred.")
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }
}