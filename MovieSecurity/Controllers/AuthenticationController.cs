using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieSecurity.Security.Interface;
using MovieSecurity.Security.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {

        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<dynamic>> SignIn(string login, string password)
        {
            if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
            {
                 IJwtModel model = GetJWTContainerModel(login, "someEmail");
                IAuthService authService = new JWTService(model.SecretKey);

                string token = authService.GenerateToken(model);

                if (!authService.IsTokenValid(token))
                    throw new UnauthorizedAccessException();

                return new { jwtToken = token };
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }
        [HttpPost]
        public async Task<ActionResult<dynamic>> Validate(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                IJwtModel model = GetJWTContainerModel("", "");
                IAuthService authService = new JWTService(model.SecretKey);
                if (!authService.IsTokenValid(token))
                    throw new UnauthorizedAccessException();

                return Ok();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }
        private static JWTAuthModel GetJWTContainerModel(string name, string email)
        {
            return new JWTAuthModel()
            {
                Claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Email, email)
                }
            };
        }
    }
}
