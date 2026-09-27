using RetailCompare.API.Data;

namespace RetailCompare.API.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}