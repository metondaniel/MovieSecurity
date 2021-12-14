using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieSecurity.Security.Interface
{
    public interface IAuthService
    {
        string SecretKey { get; set; }

        bool IsTokenValid(string token);
        string GenerateToken(IJwtModel model);
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
