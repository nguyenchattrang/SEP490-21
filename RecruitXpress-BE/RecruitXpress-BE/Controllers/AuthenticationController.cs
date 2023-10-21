using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Configuration;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;
using System.Security.Cryptography;
using RecruitXpress_BE.Helper;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace RecruitXpress_BE.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly RecruitXpressContext _context;
        public AuthenticationController(RecruitXpressContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }




        /*        [HttpPost("Login")]
                public async Task<IActionResult> Login(LoginModel model)
                {
                    try
                    {
                        Account u = new Account();
                        if (model.Account == null)
                        {
                            u = await _context.Accounts.SingleOrDefaultAsync(u => u.Token == model.Token);
                        }
                        else
                        {
                            u = await _context.Accounts.SingleOrDefaultAsync(u => u.Account1 == model.Account);
                        }
                        if (u != null && HashHelper.Decrypt(u.Password, _configuration) == model.Password)
                        {
                            return Ok(new
                            {
                                Token = GenerateToken(u),
                                UserName = u.Account1,
                                RoleId = u.RoleId
                            });
                        }
                        else
                        {
                            return BadRequest("InvalidCredential");
                        }
                    }
                    catch
                    {
                        return StatusCode(500);
                    }

                }




                private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
                {
                    using (var hmac = new HMACSHA512())
                    {
                        passwordSalt = hmac.Key;
                        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    }
                }

                private TokenModel GenerateToken(Account user)
                {
                    var access = GenerateAccessToken(user);
                    var refresh = TokenHelper.GenerateRandomToken();
                    var tokenhandler = new JwtSecurityTokenHandler();
                    var refreshEntity = new RefreshToken
                    {
                        UserId = user.UserId,
                        Token = refresh,
                        Created = DateTime.UtcNow,
                        JwtId = tokenhandler.ReadJwtToken(access).Id,
                        ExpiredAt = DateTime.UtcNow.AddMonths(1)
                    };
                    if (_context.RefreshTokens.SingleOrDefault(x => x.UserId == user.UserId) != null)
                    {
                        _context.Update(refreshEntity);
                    }
                    else
                    {
                        _context.Add(refreshEntity);
                    }
                    _context.SaveChanges();
                    return new TokenModel(access, refresh);
                }*/

        [HttpGet("get")]
        public async Task<IActionResult> ListAccount()
        {


            return Ok(_context.Accounts.ToList());
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (model is null)
            {
                return BadRequest("Invalid user request!!!");
            }

            var user = await _context.Accounts.SingleOrDefaultAsync(u => u.Account1 == model.Username);

            if (user != null && CheckPassword(user.Password, model.Password))
            {
            
                // Retrieve the secret key from appsettings.json
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Audience"],
                    audience: _configuration["Jwt:Issuer"],
                    claims: new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, user.Account1)
                    },
                    expires: DateTime.Now.AddHours(6),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new
                {
                    Token = tokenString,
                    UserName = user.Account1,
                    RoleId = user.RoleId
                });
            }

            return Unauthorized();
        }


        private bool CheckPassword(string? user, string? login)
        {
            if (user.Equals(login))
                return true;
            else return false;
        }

}
}
