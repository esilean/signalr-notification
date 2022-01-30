using System.Collections.Generic;

namespace Bev.RedSignal.Api.Services.Interfaces
{
    public interface IUserQueryString
    {
        Dictionary<string, string> DictUsers { get; set; }
    }
}