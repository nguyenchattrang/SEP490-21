using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Helper;
using Constant = RecruitXpress_BE.Helper.Constant;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/Settings/")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly RecruitXpressContext _context;
        public SettingsController(RecruitXpressContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpGet("GetLevelOfTests")]
        public async Task<IActionResult> GetLevelOfTests()
        {
            try
            {

                return Ok(new
                {
                    easy = ConstantQuestion.easy,
                    medium = ConstantQuestion.medium,
                    hard = ConstantQuestion.hard,
                    totalQuestion = ConstantQuestion.easy + ConstantQuestion.medium + ConstantQuestion.hard,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("SetLevelOfTests")]
        public async Task<IActionResult> SetUpLevelOfTests(int easy, int medium, int hard)
        {
            try
            {
                if (easy < 0 || medium < 0 || hard < 0)
                {
                    throw new ArgumentException("Số lượng câu hỏi phải là số dương");
                }
                ConstantQuestion.easy = easy;
                ConstantQuestion.medium = medium;
                ConstantQuestion.hard = hard;
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetMaxFileSize")]
        public async Task<IActionResult> GetMaxFileSize()
        {
            try
            {
                return Ok(Constant.MaxFileSize);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("SetMaxFileSize")]
        public async Task<IActionResult> SetUpMaxFileSize(int mb)
        {
            try
            {

                if (mb < 0)
                {
                    throw new ArgumentException("Kích cỡ file (mb) phải là một số dương");
                }
                Constant.MaxFileSize = mb;
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetExpireExamTokenExpert")]
        public async Task<IActionResult> GetExpireTokenExpert()
        {
            try
            {
                {
                    return Ok(Constant.MaxFileSize);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("SetExpireExamTokenExpert")]
        public async Task<IActionResult> SetUpExpireTokenExpert(int days)
        {
            try
            {

                {
                    if (days < 0)
                    {
                        throw new ArgumentException("Số ngày hết hạn token phải là một số dương");
                    }
                    Constant.MaxFileSize = days;
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetExpireRegisterAccountToken")]
        public async Task<IActionResult> GetExpireRegisterAccountToken()
        {
            try
            {

                {
                    return Ok(Constant.ExpireRegisterAccountDays);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("SetExpireRegisterAccountToken")]
        public async Task<IActionResult> SetUpExpireRegisterAccountToken(int days)
        {
            try
            {

                {
                    if (days < 0)
                    {
                        throw new ArgumentException("Số ngày hết hạn token phải là một số dương");
                    }
                    Constant.ExpireRegisterAccountDays = days;
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetExpireForgotPasswordAccountToken")]
        public async Task<IActionResult> GetExpireForgotPasswordAccountToken()
        {
            try
            {

                {
                    return Ok(Constant.ExpireForgotPasswordDays);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("SetExpireForgotPasswordAccountToken")]
        public async Task<IActionResult> SetUpExpireForgotPasswordAccountToken(int days)
        {
            try
            {

                {
                    if (days < 0)
                    {
                        throw new ArgumentException("Số ngày hết hạn token phải là một số dương");
                    }
                    Constant.ExpireForgotPasswordDays = days;
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
