using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Helper;
using Constant = RecruitXpress_BE.Helper.Constant;
using RecruitXpress_BE.DTO;

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
        [HttpGet("GetGeneralSettings")]
        public async Task<IActionResult> GetGeneralSettings()
        {
            try
            {

                return Ok(new
                {
                    NumberOfEasyQuestion = ConstantQuestion.easy,
                    NumberOfMediumQuestion = ConstantQuestion.medium,
                    NumberOfHardQuestion = ConstantQuestion.hard,
                    TotalQuestion = ConstantQuestion.easy + ConstantQuestion.medium + ConstantQuestion.hard,
                    MaxFileSize = Constant.MaxFileSize/1024/1024,
                    ExpireForgotPasswordDays = Constant.ExpireForgotPasswordDays,
                    ExpireRegisterAccountDays = Constant.ExpireRegisterAccountDays,
                    ExpireExamTokenExpert = Constant.ExpireExamDays,
                   
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("UpdateGeneralSettings")]
        public async Task<IActionResult> UpdateGeneralSettings(SettingDTO request)
        {
            try
            {
                if (request.NumberOfEasyQuestion < 0 || request.NumberOfMediumQuestion < 0 || request.NumberOfHardQuestion < 0)
                {
                    throw new ArgumentException("Số lượng câu hỏi phải là số dương");
                }
                if (request.MaxFileSize < 0)
                {
                    throw new ArgumentException("Kích cỡ file (mb) phải là một số dương");
                }
                if (request.ExpireExamTokenExpert < 0 || request.ExpireRegisterAccountToken < 0 || request.ExpireForgotPasswordAccountToken < 0 )
                {
                    throw new ArgumentException("Số ngày hết hạn token phải là một số dương");
                }
                ConstantQuestion.easy = request.NumberOfEasyQuestion;
                ConstantQuestion.medium = request.NumberOfMediumQuestion;
                ConstantQuestion.hard = request.NumberOfHardQuestion;
                Constant.MaxFileSize = request.MaxFileSize*1024*1024;
                Constant.ExpireForgotPasswordDays = request.ExpireForgotPasswordAccountToken;
                Constant.ExpireRegisterAccountDays = request.ExpireRegisterAccountToken;
                Constant.ExpireExamDays = request.ExpireExamTokenExpert;
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
