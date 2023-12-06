using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using System.Net.NetworkInformation;

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
        public async Task SendEmailRefuse(int jobApplicationID, string reason)
        {
          var emailTemplate=  _context.EmailTemplates.Where(e => e.MailType == Constant.MailType.REFUSED).FirstOrDefault();
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile).FirstOrDefault();
            var account = _context.Accounts.Where(a => a.AccountId == user.Profile.AccountId).FirstOrDefault();
            var job = _context.JobPostings.Where(j => j.JobId == user.JobId).FirstOrDefault();
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };
                emailTemplate.Body = emailTemplate.Body.Replace("@jobTitle", job.Title);
                emailTemplate.Body = emailTemplate.Body.Replace("@company", job.Company);
                emailTemplate.Body = emailTemplate.Body.Replace("@name", account.FullName);
                emailTemplate.Body = emailTemplate.Body.Replace("@reason", reason);
                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }
        public async Task SendEmailSubmitJob(int jobApplicationID)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == Constant.MailType.SUBMIT).FirstOrDefault();
            if (emailTemplate == null)
            {
                throw new ArgumentException("Email hiện tại chưa sẵn sàng");
            }
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile).FirstOrDefault();
            if(user==null)
            {
                throw new ArgumentException("Không tìm thấy hồ sơ công việc tương ứng");
            }    
            var account = _context.Accounts.Where(a => a.AccountId == user.Profile.AccountId).FirstOrDefault();
            if (account == null)
            {
                throw new ArgumentException("Không tìm thấy tài khoản");
            }
            var job = _context.JobPostings.Where(j => j.JobId == user.JobId).FirstOrDefault();
            if (job == null)
            {
                throw new ArgumentException("Không tìm thấy công việc tương ứng");
            }
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };
                emailTemplate.Body = emailTemplate.Body.Replace("@jobTitle", job.Title);
                emailTemplate.Body = emailTemplate.Body.Replace("@company", job.Company);
                emailTemplate.Body = emailTemplate.Body.Replace("@name", account.FullName);
                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }
        public async Task SendEmailCVToInterviewer(int jobApplicationID)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == Constant.MailType.HRINTERVIEWCV).FirstOrDefault();
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile).FirstOrDefault();
            var account = _context.Accounts.Where(a => a.AccountId == user.Profile.AccountId).FirstOrDefault();
            var job = _context.JobPostings.Where(j => j.JobId == (int)user.JobId).FirstOrDefault();
            Account interviewer = null;
            var interviewerName = "nhà phỏng vấn";
            var cvName = "CV_" + account.FullName;
            if (user.AssignedFor != null)
            {
                interviewer = _context.Accounts.Where(a => a.AccountId == user.AssignedFor).FirstOrDefault();
                if(interviewer!=null && interviewer.FullName!=null)
                {
                    interviewerName= interviewer.FullName;
                }    
            }
            else
            {
                throw new Exception("Chưa có interviewer");
            }
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };
                emailTemplate.Body = emailTemplate.Body.Replace("@jobTitle", job.Title);
                emailTemplate.Body = emailTemplate.Body.Replace("@company", job.Company);
                emailTemplate.Body = emailTemplate.Body.Replace("@name", interviewerName);
                emailTemplate.Body = emailTemplate.Body.Replace("@candidatename", account.FullName);
                emailTemplate.Body = emailTemplate.Body.Replace("@cv", cvName);

                //take CV
                var result = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.TemplateId == user.TemplateId);
                if (result == null)
                {
                    throw new Exception("Không tìm thấy CV");
                }
                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));

                var filePath = path + result.Url;

                if (!System.IO.File.Exists(filePath))
                {
                    throw new Exception("Không tìm thấy địa chỉ CV");
                }
                emailTemplate.Body = emailTemplate.Body.Replace("@cv", result.Url);
                _sender.SendWithAttach(interviewer.Account1, emailCopy.Header, emailCopy.Body, filePath, cvName);
            }
        }
        public async Task SendEmailExamSchedule(int jobApplicationID, string time, string location)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == Constant.MailType.EXAMSCHEDULE).FirstOrDefault();
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile).FirstOrDefault();
            var account = _context.Accounts.Where(a => a.AccountId == user.Profile.AccountId).FirstOrDefault();
            var job = _context.JobPostings.Where(j => j.JobId == user.JobId).FirstOrDefault();
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };
                emailTemplate.Body = emailTemplate.Body.Replace("@jobTitle", job.Title);
                emailTemplate.Body = emailTemplate.Body.Replace("@company", job.Company);
                emailTemplate.Body = emailTemplate.Body.Replace("@name", account.FullName);
                emailTemplate.Body = emailTemplate.Body.Replace("@time", time);
                emailTemplate.Body = emailTemplate.Body.Replace("@location", location);

                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }
        public async Task SendEmailInterviewSchedule(int jobApplicationID, string time, string location, string? interviewer)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == Constant.MailType.INTERVIEWSCHEDULE).FirstOrDefault();
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile).FirstOrDefault();
            var account = _context.Accounts.Where(a => a.AccountId == user.Profile.AccountId).FirstOrDefault();
            var job = _context.JobPostings.Where(j => j.JobId == user.JobId).FirstOrDefault();
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };
                emailTemplate.Body = emailTemplate.Body.Replace("@jobTitle", job.Title);
                emailTemplate.Body = emailTemplate.Body.Replace("@company", job.Company);
                emailTemplate.Body = emailTemplate.Body.Replace("@name", account.FullName);
                emailTemplate.Body = emailTemplate.Body.Replace("@time", time);
                emailTemplate.Body = emailTemplate.Body.Replace("@location", location);
                emailTemplate.Body = emailTemplate.Body.Replace("@interviewer", interviewer);

                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }
        public async Task SendEmailScheduleForInterviewer(int jobApplicationID, string time, string location)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == Constant.MailType.HRINTERVIEWSCHEDULE).FirstOrDefault();
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile).FirstOrDefault();
            var account = _context.Accounts.Where(a => a.AccountId == user.Profile.AccountId).FirstOrDefault();
            var job = _context.JobPostings.Where(j => j.JobId == user.JobId).FirstOrDefault();
            Account interviewer = null;
            var interviewerName = "nhà phỏng vấn";
            var cvName = "CV_" + account.FullName;
            if (user.AssignedFor != null)
            {
                interviewer = _context.Accounts.Where(a => a.AccountId == user.AssignedFor).FirstOrDefault();
                if (interviewer != null && interviewer.FullName != null)
                {
                    interviewerName = interviewer.FullName;
                }
            }
            else
            {
                throw new Exception("Chưa có interviewer");
            }
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };

                emailTemplate.Body = emailTemplate.Body.Replace("@name", interviewerName);
                emailTemplate.Body = emailTemplate.Body.Replace("@jobTitle", job.Title);
                emailTemplate.Body = emailTemplate.Body.Replace("@company", job.Company);
                emailTemplate.Body = emailTemplate.Body.Replace("@candidatename", account.FullName);
                emailTemplate.Body = emailTemplate.Body.Replace("@time", time);
                emailTemplate.Body = emailTemplate.Body.Replace("@location", location);

                 _sender.Send(interviewer.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailUpdateProfile(int jobApplicationID)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == Constant.MailType.PASSINTERVIEW).FirstOrDefault();
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile).FirstOrDefault();
            var account = _context.Accounts.Where(a => a.AccountId == user.Profile.AccountId).FirstOrDefault();
            var job = _context.JobPostings.Where(j => j.JobId == user.JobId).FirstOrDefault();
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };
                
                emailTemplate.Body = emailTemplate.Body.Replace("@jobTitle", job.Title);
                emailTemplate.Body = emailTemplate.Body.Replace("@company", job.Company);
                emailTemplate.Body = emailTemplate.Body.Replace("@name", account.FullName);
                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }
        public async Task SendEmailAccepted(int jobApplicationID)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == Constant.MailType.ACCEPTED).FirstOrDefault();
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile).FirstOrDefault();
            var account = _context.Accounts.Where(a => a.AccountId == user.Profile.AccountId).FirstOrDefault();
            var job = _context.JobPostings.Where(j => j.JobId == user.JobId).FirstOrDefault();
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };

                emailTemplate.Body = emailTemplate.Body.Replace("@jobTitle", job.Title);
                emailTemplate.Body = emailTemplate.Body.Replace("@company", job.Company);
                emailTemplate.Body = emailTemplate.Body.Replace("@name", account.FullName);

                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }
        public async Task SendEmailCanceled(int jobApplicationID)
        {
            var emailTemplate = _context.EmailTemplates.Where(e => e.MailType == Constant.MailType.CANCEL).FirstOrDefault();
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile).FirstOrDefault();
            var account = _context.Accounts.Where(a => a.AccountId == user.Profile.AccountId).FirstOrDefault();
            var job = _context.JobPostings.Where(j => j.JobId == user.JobId).FirstOrDefault();
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };

                // Modify the copy for sending purposes
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@name", account.FullName);

                // Send the email using the modified copy
                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task CreateEmailTemplate(EmailTemplate emailTemplate)
        {
            _context.EmailTemplates.Add(emailTemplate);
            await _context.SaveChangesAsync();
        }

/*        public async Task UpdateEmailTemplate(EmailTemplate emailTemplate)
        {
            _context.Entry(emailTemplate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }*/

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





