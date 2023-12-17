namespace shoppingify_backend.Helpers.CustomExceptions
{
    public class ForbiddenException : Exception
    {
        static public System.Net.HttpStatusCode ErrorCode = System.Net.HttpStatusCode.Forbidden;

        public ForbiddenException()
        {

        }

        public ForbiddenException(string message) : base(message)
        {

        }
        public ForbiddenException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
