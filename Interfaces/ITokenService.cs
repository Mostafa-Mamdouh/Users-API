using System.Security.Claims;

namespace UserAPI.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken();

    }
}
