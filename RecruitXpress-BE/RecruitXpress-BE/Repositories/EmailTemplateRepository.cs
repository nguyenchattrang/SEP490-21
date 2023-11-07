using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories
{
    public class EmailTemplateRepository
    {
        private readonly RecruitXpressContext _context;

        public EmailTemplateRepository(RecruitXpressContext context)
        {
            _context = context;
        }

        public async Task<List<EmailTemplate>> GetAllEmailTemplates(EmailTemplateRequest request)
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

             /*       case "questionId":
                        query = request.OrderByAscending
                            ? query.OrderBy(j => j.QuestionId)
                            : query.OrderByDescending(j => j.QuestionId);
                        break;

                    default:
                        query = request.OrderByAscending
                               ? query.OrderBy(j => j.QuestionId)
                               : query.OrderByDescending(j => j.QuestionId);
                        break;*/
                }
            }

            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;
            var questions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return await query.ToListAsync();
        }

        public async Task<EmailTemplate> GetEmailTemplateById(int templateId)
        {
            return await _context.EmailTemplates.FirstOrDefaultAsync(e => e.TemplateId == templateId);
        }

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
