using System.Net;

namespace shoppingify_backend.Helpers.CustomExceptions
{
    public class ConflictException: Exception
    {
        public static HttpStatusCode ErrorCode = HttpStatusCode.Conflict;

        public ConflictException()
        {
            
        }
        public ConflictException(string message) : base(message)
        {
            
        }

        public ConflictException(string message, Exception inner): base(message, inner)
        {
            
        }
    }
}
