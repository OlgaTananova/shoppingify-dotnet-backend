using System.Net;

namespace shoppingify_backend.Helpers.CustomExceptions
{
    public class NotFoundException : Exception
    {
        public static HttpStatusCode ErrorCode = HttpStatusCode.NotFound;

        public NotFoundException()
        {
        }
        public NotFoundException(string message) : base(message)
        {
        }
        public NotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
