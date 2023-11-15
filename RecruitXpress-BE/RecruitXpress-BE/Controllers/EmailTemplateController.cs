using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/emailtemplates")]
    [ApiController]
    public class EmailTemplateController : ControllerBase
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository;

        public EmailTemplateController(IEmailTemplateRepository emailTemplateRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
        }

        // Define your controller actions here.
        // For example, you can define actions to get all email templates, get an email template by ID, create, update, and delete email templates.

        [HttpGet]
        public async Task<IActionResult> GetAllEmailTemplates([FromQuery] EmailTemplateRequest request)
        {
            var emailTemplates = await _emailTemplateRepository.GetAllEmailTemplates(request);
            return Ok(emailTemplates);
        }

        [HttpGet("{templateId}")]
        public async Task<IActionResult> GetEmailTemplateById(int templateId)
        {
            var emailTemplate = await _emailTemplateRepository.GetEmailTemplateById(templateId);
            if (emailTemplate == null)
            {
                return NotFound();
            }
            return Ok(emailTemplate);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmailTemplate([FromBody] EmailTemplate emailTemplate)
        {
            await _emailTemplateRepository.CreateEmailTemplate(emailTemplate);
            return CreatedAtAction("GetEmailTemplateById", new { templateId = emailTemplate.TemplateId }, emailTemplate);
        }

        [HttpPut("{templateId}")]
        public async Task<IActionResult> UpdateEmailTemplate(int templateId, [FromBody] EmailTemplate emailTemplate)
        {
            if (templateId != emailTemplate.TemplateId)
            {
                return BadRequest();
            }

            await _emailTemplateRepository.UpdateEmailTemplate(emailTemplate);
            return NoContent();
        }

        [HttpDelete("{templateId}")]
        public async Task<IActionResult> DeleteEmailTemplate(int templateId)
        {
            var emailTemplate = await _emailTemplateRepository.GetEmailTemplateById(templateId);
            if (emailTemplate == null)
            {
                return NotFound();
            }

            await _emailTemplateRepository.DeleteEmailTemplate(templateId);
            return NoContent();
        }


        [HttpPost("refuse")]
        public async Task<IActionResult> SendEmailRefuse([FromQuery] int mailtype, [FromQuery] string email, [FromQuery] string name)
        {
            await _emailTemplateRepository.SendEmailRefuse(mailtype, email, name);
            return Ok("Email sent successfully");
        }

        [HttpPost("interview")]
        public async Task<IActionResult> SendEmailInterview([FromQuery] int mailtype, [FromQuery] string email, [FromQuery] string name, [FromQuery] string time, [FromQuery] string location, [FromQuery] string interviewer)
        {
            await _emailTemplateRepository.SendEmailInterview(mailtype, email, name, time, location, interviewer);
            return Ok("Email sent successfully");
        }

        [HttpPost("exam-schedule")]
        public async Task<IActionResult> SendEmailExamSchedule([FromQuery] int mailtype, [FromQuery] string email, [FromQuery] string name, [FromQuery] string time, [FromQuery] string location)
        {
            await _emailTemplateRepository.SendEmailExamSchedule(mailtype, email, name, time, location);
            return Ok("Email sent successfully");
        }

        [HttpPost("update-profile")]
        public async Task<IActionResult> SendEmailUpdateProfile([FromQuery] int mailtype, [FromQuery] string email, [FromQuery] string name)
        {
            await _emailTemplateRepository.SendEmailUpdateProfile(mailtype, email, name);
            return Ok("Email sent successfully");
        }

        [HttpPost("accepted")]
        public async Task<IActionResult> SendEmailAccepted([FromQuery] int mailtype, [FromQuery] string email, [FromQuery] string name)
        {
            await _emailTemplateRepository.SendEmailAccepted(mailtype, email, name);
            return Ok("Email sent successfully");
        }

        [HttpPost("canceled")]
        public async Task<IActionResult> SendEmailCanceled([FromQuery] int mailtype, [FromQuery] string email, [FromQuery] string name)
        {
            await _emailTemplateRepository.SendEmailCanceled(mailtype, email, name);
            return Ok("Email sent successfully");
        }
    }
}
