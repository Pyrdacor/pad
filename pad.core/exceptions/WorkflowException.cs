namespace pad.core.exceptions
{
    internal class WorkflowException : ApplicationException
    {
        public WorkflowException(string? message)
            : base(message)
        {

        }

        public WorkflowException(string? message, Exception? innerException)
            : base(message, innerException)
        {

        }
    }
}
