
namespace Register.Web.CustomExceptions
{
    public class ResultEmptyException:Exception
    {
        public ResultEmptyException() : base() {
           
        }
        public ResultEmptyException(string message) : base(message) {

           
        }
        public ResultEmptyException(string message, System.Exception inner) : base(message, inner) { }

    }
}
