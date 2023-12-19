using System.Security.Claims;

namespace shoppingify_backend.Helpers
{

    // Custom class to extract the current user from Http Request
    public class UserResolverService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserResolverService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string GetCurrentUserId()
        {
            return _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

    }
}
