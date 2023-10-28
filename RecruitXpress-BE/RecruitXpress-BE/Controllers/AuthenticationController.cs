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
using System.Net.Mail;
using System.Net;
using System.Reflection.Metadata;
using Constant = RecruitXpress_BE.Helper.Constant;

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

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            try
            {

                var user = await _context.Accounts.SingleOrDefaultAsync(u => u.Account1 == model.Email);
                if ((user == null) && Constant.validateGuidRegex.IsMatch(model.Password))
                {
                    SendEmail();

        user = new Account
                    {
                        Account1 = model.Email,
                        Password = HashHelper.Encrypt(model.Password, _configuration),
                        RoleId = 1,
                    };
                    _context.Accounts.Add(user);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("This Email is already existed");
                }
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }


    public async Task<IActionResult> SendEmail()
        {
            return Ok();
        }


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

            if (user != null && CheckPassword(user, model))
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


        private bool CheckPassword(Account? user, LoginModel? login)
        {
            if (user != null && HashHelper.Decrypt(user.Password, _configuration) == login.Password)
            {
                return true;
            }

             
            else return false;
        }

    }
}
