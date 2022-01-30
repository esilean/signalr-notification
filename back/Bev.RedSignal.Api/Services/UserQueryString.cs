using System.Collections.Generic;
using Bev.RedSignal.Api.Services.Interfaces;

namespace Bev.RedSignal.Api.Services
{
    public class UserQueryString : IUserQueryString
    {
        public Dictionary<string, string> DictUsers { get; set; } = new();
    }
}