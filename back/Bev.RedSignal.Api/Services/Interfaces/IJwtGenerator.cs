
namespace Bev.RedSignal.Api.Services.Interfaces
{
    public interface IJwtGenerator
    {
        string Generate(string username);
    }
}