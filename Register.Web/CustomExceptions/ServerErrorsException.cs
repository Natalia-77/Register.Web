namespace Register.Web.CustomExceptions
{
    public class ServerErrorsException:Exception
    {
        public ServerErrorsException() : base() { }
        public ServerErrorsException(string message) : base(message) { }
        public ServerErrorsException(string message, System.Exception inner) : base(message, inner) { }
    }
}
