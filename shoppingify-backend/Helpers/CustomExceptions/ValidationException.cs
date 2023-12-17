using System.Net;

namespace shoppingify_backend.Helpers.CustomExceptions
{
    public class ValidationException : Exception
    {
        static public HttpStatusCode ErrorCode = HttpStatusCode.BadRequest;
        public ValidationException()
        {

        }
        public ValidationException(string message) : base(message)
        {

        }
        public ValidationException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
