using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Security.Cryptography.Xml;

using webapi.Heplers;
using webapi.Jwt;

namespace webapi.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IJwtAuthentication jwtAuthentication;
        public AuthenticationController(IJwtAuthentication _jwtAuthentication)
        {
            jwtAuthentication = _jwtAuthentication;
        }
        [HttpPost]
        [Route("api/authentication/login")]
        public IActionResult Auth(EncryptedAuthRequest auth)
        {

            string authRequest = DecryptionHelper.DecryptData(auth.EncryptedData,auth.EncryptionKey);

            AuthRequest _authRequest = JsonConvert.DeserializeObject<AuthRequest>(authRequest);

            // Check CardNo and Password from DB
            if (_authRequest.CardNo == 4712345601012222 && _authRequest.Password == 1324)
            {
                var token = jwtAuthentication.Authenticate(_authRequest.CardNo);
                return Ok(new { token });
            }
            else
            {
                string message = "Please Enter Valid CardNo && Password";
                return BadRequest(new { message });
            }
        }
    }
}
