using System.Net;

namespace shoppingify_backend.Helpers.CustomExceptions
{
    public class BadRequestException : Exception
    {
        public static HttpStatusCode ErrorCode = HttpStatusCode.BadRequest;
        public BadRequestException()
        {
        }

        public BadRequestException(string message)
            : base(message)
        {
        }

        public BadRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
}
