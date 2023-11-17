using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories
{
    public class EmailTemplateRepository: IEmailTemplateRepository
    {
        private readonly RecruitXpressContext _context;
        private readonly IEmailSender _sender;

        public EmailTemplateRepository(RecruitXpressContext context, IEmailSender sender)
        {
            _context = context;
            _sender = sender;
        }

        public async Task<ApiResponse<EmailTemplate>> GetAllEmailTemplates(EmailTemplateRequest request)
        {
            IQueryable<EmailTemplate> query = _context.EmailTemplates;

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                query = query.Where(e => e.Title.Contains(request.Title));
            }

            if (!string.IsNullOrWhiteSpace(request.SendTo))
            {
                query = query.Where(e => e.SendTo == request.SendTo);
            }

            if (request.CreatedBy.HasValue)
            {
                query = query.Where(e => e.CreatedBy == request.CreatedBy);
            }
            if (request.Status.HasValue)
            {
                query = query.Where(e => e.MailType == request.MailType);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(e => e.Status == request.Status);
            }
            if (!string.IsNullOrWhiteSpace(request.SearchAll))
            {
                query = query.Where(e =>
            e.Title.Contains(request.SearchAll) ||
            e.Header.Contains(request.SearchAll) ||
            e.Body.Contains(request.SearchAll) ||
            e.SendTo.Contains(request.SearchAll) ||
            e.CreatedBy.ToString().Contains(request.SearchAll) ||
            e.Status.ToString().Contains(request.SearchAll));
            }

            if (request.SortBy != null)
            {
                switch (request.SortBy)
                {
                    case "title":
                        query = request.OrderByAscending
                            ? query.OrderBy(j => j.Title)
                            : query.OrderByDescending(j => j.Title);
                        break;

                    case "header":
                        query = request.OrderByAscending
                            ? query.OrderBy(j => j.Header)
                            : query.OrderByDescending(j => j.Header);
                        break;

                    default:
                        query = request.OrderByAscending
                               ? query.OrderBy(j => j.TemplateId)
                               : query.OrderByDescending(j => j.TemplateId);
                        break;
                }
            }
            var totalCount = await query.CountAsync();

            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;
            var emails = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();



            var response = new ApiResponse<EmailTemplate>
            {
                Items = emails,
                TotalCount = totalCount,
            };
            return response;
        }

        public async Task<EmailTemplate> GetEmailTemplateById(int templateId)
        {
            return await _context.EmailTemplates.FirstOrDefaultAsync(e => e.TemplateId == templateId);
        }

        //Email template
        public async Task SendEmailRefuse(int mailtype, string email, string name)
        {
          var emailTemplate=  _context.EmailTemplates.Where(e => e.MailType == mailtype).FirstOrDefault();
            if (emailTemplate != null)
            {
                emailTemplate.Body = emailTemplate.Body.Replace("@name", name);
                _sender.Send(email, emailTemplate.Header, emailTemplate.Body);
            }
        }
        public async Task SendEmailInterview(int mailtype, string email, string name, string time, string location, string interviewer)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == mailtype).FirstOrDefault();
            if (emailTemplate != null)
            {
                emailTemplate.Body = emailTemplate.Body.Replace("@name", name);
                emailTemplate.Body = emailTemplate.Body.Replace("@time", time);
                emailTemplate.Body = emailTemplate.Body.Replace("@location", location);
                emailTemplate.Body = emailTemplate.Body.Replace("@interviewer", interviewer);

                _sender.Send(email, emailTemplate.Header, emailTemplate.Body);
            }
        }
        public async Task SendEmailExamSchedule(int mailtype, string email, string name, string time, string location)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == mailtype).FirstOrDefault();
            if (emailTemplate != null)
            {
                emailTemplate.Body = emailTemplate.Body.Replace("@name", name);
                emailTemplate.Body = emailTemplate.Body.Replace("@time", time);
                emailTemplate.Body = emailTemplate.Body.Replace("@location", location);

                _sender.Send(email, emailTemplate.Header, emailTemplate.Body);
            }
        }
        public async Task SendEmailUpdateProfile(int mailtype, string email, string name)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == mailtype).FirstOrDefault();
            if (emailTemplate != null)
            {
                emailTemplate.Body = emailTemplate.Body.Replace("@name", name);

                _sender.Send(email, emailTemplate.Header, emailTemplate.Body);
            }
        }
        public async Task SendEmailAccepted(int mailtype, string email, string name)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == mailtype).FirstOrDefault();
            if (emailTemplate != null)
            {
                emailTemplate.Body = emailTemplate.Body.Replace("@name", name);

                _sender.Send(email, emailTemplate.Header, emailTemplate.Body);
            }
        }
        public async Task SendEmailCanceled(int mailtype, string email, string name)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == mailtype).FirstOrDefault();
            if (emailTemplate != null)
            {
                emailTemplate.Body = emailTemplate.Body.Replace("@name", name);

                _sender.Send(email, emailTemplate.Header, emailTemplate.Body);
            }
        }
        //
        public async Task CreateEmailTemplate(EmailTemplate emailTemplate)
        {
            _context.EmailTemplates.Add(emailTemplate);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmailTemplate(EmailTemplate emailTemplate)
        {
            _context.Entry(emailTemplate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmailTemplate(int templateId)
        {
            var emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(e => e.TemplateId == templateId);
            if (emailTemplate != null)
            {
                _context.EmailTemplates.Remove(emailTemplate);
                await _context.SaveChangesAsync();
            }
        }
    }
}
