using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieSecurity.Model;
using MovieSecurity.Security.Interface;
using MovieSecurity.Security.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieSecurity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {

        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("signin")]
        public async Task<ActionResult<dynamic>> SignIn([FromBody]LoginModel loginModel)
        {
            if (!string.IsNullOrEmpty(loginModel.Login) && !string.IsNullOrEmpty(loginModel.Password))
            {
                 IJwtModel model = GetJWTContainerModel(loginModel.Login, "someEmail");
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
        [Route("validate")]
        public async Task<ActionResult<dynamic>> Validate()
        {
            string token = Request.Form["token"].ToString();
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
