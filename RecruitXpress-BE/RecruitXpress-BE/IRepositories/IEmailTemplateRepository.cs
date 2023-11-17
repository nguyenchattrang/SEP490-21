using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories
{
    public interface IEmailTemplateRepository
    {
        Task<ApiResponse<EmailTemplate>> GetAllEmailTemplates(EmailTemplateRequest request);

        Task<EmailTemplate> GetEmailTemplateById(int templateId);
        Task CreateEmailTemplate(EmailTemplate emailTemplate);

        Task UpdateEmailTemplate(EmailTemplate emailTemplate);

        Task DeleteEmailTemplate(int templateId);
        Task SendEmailRefuse(int mailtype, string email, string name);
        Task SendEmailInterview(int mailtype, string email, string name, string time, string location, string interviewer);
        Task SendEmailExamSchedule(int mailtype, string email, string name, string time, string location);
        Task SendEmailUpdateProfile(int mailtype, string email, string name);
        Task SendEmailAccepted(int mailtype, string email, string name);
        Task SendEmailCanceled(int mailtype, string email, string name);

    }
}
