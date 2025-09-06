namespace DefectListDomain.EventArgs
{
    public class ErrorEventArgs : System.EventArgs
    {
        public string Message { get; }

        public ErrorEventArgs(string message)
        {
            Message = message;
        }
    }
}