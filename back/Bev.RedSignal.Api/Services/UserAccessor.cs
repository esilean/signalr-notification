using System.Linq;
using System.Security.Claims;
using Bev.RedSignal.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Bev.RedSignal.Api.Services
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentUsername()
        {
            var username = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x =>
                    x.Type == ClaimTypes.NameIdentifier)?.Value;

            return username;
        }
    }
}