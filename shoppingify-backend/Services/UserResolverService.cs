using System.Security.Claims;

namespace shoppingify_backend.Services
{
    public interface IUserResolverService
    {
        string GetCurrentUserId();
    }

    // Custom class to extract the current user from Http Request
    public class UserResolverService: IUserResolverService
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
