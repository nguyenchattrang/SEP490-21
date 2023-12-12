using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly RecruitXpressContext _context;
        private readonly IEmailSender _sender;
        private static Dictionary<int, EmailTemplate?> _emailTemplates = new();
        private readonly IConfiguration _configuration;

        public EmailTemplateRepository(RecruitXpressContext context, IEmailSender sender, IConfiguration configuration)
        {
            _context = context;
            _sender = sender;
            _configuration = configuration;
        }

        public async Task<ApiResponse<EmailTemplate>> GetAllEmailTemplates(EmailTemplateRequest request)
        {
            IQueryable<EmailTemplate?> query = _context.EmailTemplates;

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
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.REFUSED, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.REFUSED);
                _emailTemplates.Add(Constant.MailType.REFUSED, emailTemplate);
            }
            // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.REFUSED);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@name", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@reason", reason);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);
                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailSubmitJob(int jobApplicationID)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.SUBMIT, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.SUBMIT);
                _emailTemplates.Add(Constant.MailType.SUBMIT, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.SUBMIT);
            if (emailTemplate == null)
            {
                throw new ArgumentException("Email hiện tại chưa sẵn sàng");
            }

            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            if (user == null)
            {
                throw new ArgumentException("Không tìm thấy hồ sơ công việc tương ứng");
            }

            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            if (account == null)
            {
                throw new ArgumentException("Không tìm thấy tài khoản");
            }

            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);
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
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@name", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);
                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailCVToInterviewer(int jobApplicationID)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.HRINTERVIEWCV, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.HRINTERVIEWCV);
                _emailTemplates.Add(Constant.MailType.HRINTERVIEWCV, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.HRINTERVIEWCV);
            var application = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID)
                .Include(j => j.Profile).ThenInclude(p => p.Account)
                .Include(jobApplication => jobApplication.Job)
                .FirstOrDefault();
            Account? interviewer;
            var interviewerName = "nhà phỏng vấn";
            var cvName = "CV_" + application?.Profile?.Account?.FullName;
            if (application?.AssignedFor != null)
            {
                interviewer = _context.Accounts.FirstOrDefault(a => a.AccountId == application.AssignedFor);
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
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", application.Job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", application.Job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@name", interviewerName);
                emailCopy.Body = emailCopy.Body.Replace("@candidatename", application?.Profile?.Account?.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@cv", cvName);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);
                //take CV
                var result =
                    await _context.CandidateCvs.FirstOrDefaultAsync(x => x.TemplateId == application.TemplateId);
                if (result == null)
                {
                    throw new ArgumentException("Không tìm thấy CV");
                }

                var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));

                var filePath = path + result.Url;
                if (!File.Exists(filePath))
                {
                    throw new ArgumentException("Không tìm thấy địa chỉ CV");
                }

                emailTemplate.Body = emailTemplate.Body.Replace("@cv", result.Url);
                _sender.SendWithAttach(interviewer.Account1, emailCopy.Header, emailCopy.Body, filePath, cvName);
            }
        }

        public async Task SendEmailExamSchedule(int jobApplicationID, string time, string location)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.EXAMSCHEDULE, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.EXAMSCHEDULE);
                _emailTemplates.Add(Constant.MailType.EXAMSCHEDULE, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.EXAMSCHEDULE);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@name", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@time", time);
                emailCopy.Body = emailCopy.Body.Replace("@location", location);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);

                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailInterviewSchedule(int jobApplicationID, string time, string location,
            string? interviewer)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.INTERVIEWSCHEDULE, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.INTERVIEWSCHEDULE);
                _emailTemplates.Add(Constant.MailType.INTERVIEWSCHEDULE, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.INTERVIEWSCHEDULE);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@name", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@time", time);
                emailCopy.Body = emailCopy.Body.Replace("@location", location);
                emailCopy.Body = emailCopy.Body.Replace("@interviewer", interviewer);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);

                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailScheduleForInterviewer(int jobApplicationID, string time, string location)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.HRINTERVIEWSCHEDULE, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.HRINTERVIEWSCHEDULE);
                _emailTemplates.Add(Constant.MailType.HRINTERVIEWSCHEDULE, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.HRINTERVIEWSCHEDULE);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);
            Account interviewer = null;
            var interviewerName = "nhà phỏng vấn";
            var cvName = "CV_" + account.FullName;
            if (user.AssignedFor != null)
            {
                interviewer = _context.Accounts.FirstOrDefault(a => a.AccountId == user.AssignedFor);
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

                emailCopy.Body = emailCopy.Body.Replace("@name", interviewerName);
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@candidatename", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@time", time);
                emailCopy.Body = emailCopy.Body.Replace("@location", location);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);

                _sender.Send(interviewer.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailScheduleForInterviewer2(int scheduleId, string time, string location)
        {
            try
            {
                EmailTemplate? emailTemplate;
                if (!_emailTemplates.TryGetValue(Constant.MailType.HRINTERVIEWSCHEDULE, out emailTemplate))
                {
                    emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.HRINTERVIEWSCHEDULE);
                    _emailTemplates.Add(Constant.MailType.HRINTERVIEWSCHEDULE, emailTemplate);
                }
                    // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.HRINTERVIEWSCHEDULE);
                if (emailTemplate == null) throw new Exception("Email template null");
                var scheduleDetails = _context.ScheduleDetails
                    .Include(sd => sd.Candidate).ThenInclude(jobApplication => jobApplication.Profile)
                    .ThenInclude(profile => profile.Account).Include(scheduleDetail => scheduleDetail.Candidate)
                    .ThenInclude(jobApplication => jobApplication.Job)
                    .Where(sd => sd.ScheduleId == scheduleId).ToList();
                if (scheduleDetails.Count <= 0) throw new Exception("Candidates null");
                var candidateNames = scheduleDetails.Aggregate<ScheduleDetail?, string?>(null,
                    (current, scheduleDetail) =>
                        current + (scheduleDetail?.Candidate?.Profile?.Account?.FullName + ", "));
                var interviewer = await _context.Accounts.FirstOrDefaultAsync(a =>
                    scheduleDetails.First().Candidate != null &&
                    a.AccountId == scheduleDetails.First().Candidate.AssignedFor);
                var job = scheduleDetails.First().Candidate?.Job;
                var interviewerName = "nhà phỏng vấn";
                if (interviewer is { FullName: not null })
                {
                    interviewerName = interviewer.FullName;
                }

                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };

                emailCopy.Body = emailCopy.Body.Replace("@name", interviewerName);
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@candidatename", candidateNames);
                emailCopy.Body = emailCopy.Body.Replace("@time", time);
                emailCopy.Body = emailCopy.Body.Replace("@location", location);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);

                if (interviewer?.Account1 == null)
                    throw new Exception("Interviewer account null");
                _sender.Send(interviewer.Account1, emailCopy.Header, emailCopy.Body);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task CandidateCancelJobApplicationToHR(int jobApplicationID)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.UpdateExamSchedule, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.UpdateExamSchedule);
                _emailTemplates.Add(Constant.MailType.UpdateExamSchedule, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.UpdateExamSchedule);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);

            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };

                emailCopy.Body = emailCopy.Body.Replace("@name", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);


                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailUpdateExamScheduleToCandidate(int jobApplicationID, string time, string location)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.UpdateExamSchedule, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.UpdateExamSchedule);
                _emailTemplates.Add(Constant.MailType.UpdateExamSchedule, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.UpdateExamSchedule);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);

            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };

                emailCopy.Body = emailCopy.Body.Replace("@name", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@time", time);
                emailCopy.Body = emailCopy.Body.Replace("@location", location);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);

                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailDeleteExamScheduleToCandidate(int jobApplicationID, string reason)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.DeleteExamSchedule, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.DeleteExamSchedule);
                _emailTemplates.Add(Constant.MailType.DeleteExamSchedule, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.DeleteExamSchedule);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);

            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };

                emailCopy.Body = emailCopy.Body.Replace("@name", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@reason", reason);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);


                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailUpdateInterviewScheduleToCandidate(int jobApplicationID, string time,
            string location)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.UpdateInterviewScheduleForCandidate, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.UpdateInterviewScheduleForCandidate);
                _emailTemplates.Add(Constant.MailType.UpdateInterviewScheduleForCandidate, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.UpdateInterviewScheduleForCandidate);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);

            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };

                emailCopy.Body = emailCopy.Body.Replace("@name", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@time", time);
                emailCopy.Body = emailCopy.Body.Replace("@location", location);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);

                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailDeleteInterviewScheduleToCandidate(int jobApplicationID, string reason)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.DeleteInterviewcheduleForCandidate, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.DeleteInterviewcheduleForCandidate);
                _emailTemplates.Add(Constant.MailType.DeleteInterviewcheduleForCandidate, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.DeleteInterviewcheduleForCandidate);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);

            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };

                emailCopy.Body = emailCopy.Body.Replace("@name", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@reason", reason);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);

                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailUpdateScheduleForInterviewer(int jobApplicationID, string time, string location)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.UpdateInterviewScheduleForInterviewer, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.UpdateInterviewScheduleForInterviewer);
                _emailTemplates.Add(Constant.MailType.UpdateInterviewScheduleForInterviewer, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.UpdateInterviewScheduleForInterviewer);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);
            Account interviewer = null;
            var interviewerName = "nhà phỏng vấn";
            var cvName = "CV_" + account.FullName;
            if (user.AssignedFor != null)
            {
                interviewer = _context.Accounts.FirstOrDefault(a => a.AccountId == user.AssignedFor);
                if (interviewer is { FullName: not null })
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

                emailCopy.Body = emailCopy.Body.Replace("@name", interviewerName);
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@candidatename", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@time", time);
                emailCopy.Body = emailCopy.Body.Replace("@location", location);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);

                _sender.Send(interviewer.Account1, emailCopy.Header, emailCopy.Body);
            }
        }
        
        public async Task SendEmailUpdateScheduleForInterviewer2(int scheduleId, string time, string location)
        {
            try
            {
                EmailTemplate? emailTemplate;
                if (!_emailTemplates.TryGetValue(Constant.MailType.UpdateInterviewScheduleForInterviewer, out emailTemplate))
                {
                    emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.UpdateInterviewScheduleForInterviewer);
                    _emailTemplates.Add(Constant.MailType.UpdateInterviewScheduleForInterviewer, emailTemplate);
                }
                if (emailTemplate == null) throw new Exception("Email template null");
                var scheduleDetails = _context.ScheduleDetails
                    .Include(sd => sd.Candidate).ThenInclude(jobApplication => jobApplication.Profile)
                    .ThenInclude(profile => profile.Account).Include(scheduleDetail => scheduleDetail.Candidate)
                    .ThenInclude(jobApplication => jobApplication.Job)
                    .Where(sd => sd.ScheduleId == scheduleId).ToList();
                if (scheduleDetails.Count <= 0) throw new Exception("Interviewers or candidates null");
                var candidateNames = scheduleDetails.Aggregate<ScheduleDetail?, string?>(null,
                    (current, scheduleDetail) =>
                        current + (scheduleDetail?.Candidate?.Profile?.Account?.FullName + ", "));
                var interviewer = await _context.Accounts.FirstOrDefaultAsync(a =>
                    scheduleDetails.First().Candidate != null &&
                    a.AccountId == scheduleDetails.First().Candidate.AssignedFor);
                var job = scheduleDetails.First().Candidate?.Job;
                var interviewerName = "nhà phỏng vấn";
                if (interviewer is { FullName: not null })
                {
                    interviewerName = interviewer.FullName;
                }

                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };

                emailCopy.Body = emailCopy.Body.Replace("@name", interviewerName);
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@candidatename", candidateNames);
                emailCopy.Body = emailCopy.Body.Replace("@time", time);
                emailCopy.Body = emailCopy.Body.Replace("@location", location);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);

                if (interviewer?.Account1 == null)
                    throw new Exception("Interviewer account null");
                _sender.Send(interviewer.Account1, emailCopy.Header, emailCopy.Body);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task SendEmailDeleteScheduleForInterviewer(int jobApplicationID, string reason)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.DeleteInterviewcheduleForInterviewer, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.DeleteInterviewcheduleForInterviewer);
                _emailTemplates.Add(Constant.MailType.DeleteInterviewcheduleForInterviewer, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.DeleteInterviewcheduleForInterviewer);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);
            Account interviewer = null;
            var interviewerName = "nhà phỏng vấn";
            var cvName = "CV_" + account.FullName;
            if (user.AssignedFor != null)
            {
                interviewer = _context.Accounts.FirstOrDefault(a => a.AccountId == user.AssignedFor);
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

                emailCopy.Body = emailCopy.Body.Replace("@name", interviewerName);
                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@candidatename", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@reason", reason);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);

                _sender.Send(interviewer.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailUpdateProfile(int jobApplicationID)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.PASSINTERVIEW, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.PASSINTERVIEW);
                _emailTemplates.Add(Constant.MailType.PASSINTERVIEW, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.PASSINTERVIEW);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };

                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@name", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);
                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailAccepted(int jobApplicationID)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.ACCEPTED, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.ACCEPTED);
                _emailTemplates.Add(Constant.MailType.ACCEPTED, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.ACCEPTED);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);
            if (emailTemplate != null)
            {
                // Create a copy of the email template
                var emailCopy = new EmailTemplate
                {
                    MailType = emailTemplate.MailType,
                    Header = emailTemplate.Header,
                    Body = emailTemplate.Body
                };

                emailCopy.Body = emailCopy.Body.Replace("@jobTitle", job.Title);
                emailCopy.Body = emailCopy.Body.Replace("@company", job.Company);
                emailCopy.Body = emailCopy.Body.Replace("@name", account.FullName);
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);

                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task SendEmailCanceled(int jobApplicationID)
        {
            EmailTemplate? emailTemplate;
            if (!_emailTemplates.TryGetValue(Constant.MailType.CANCEL, out emailTemplate))
            {
                emailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.CANCEL);
                _emailTemplates.Add(Constant.MailType.CANCEL, emailTemplate);
            }
                // _context.EmailTemplates.FirstOrDefault(e => e.MailType == Constant.MailType.CANCEL);
            var user = _context.JobApplications.Where(j => j.ApplicationId == jobApplicationID).Include(j => j.Profile)
                .FirstOrDefault();
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == user.Profile.AccountId);
            var job = _context.JobPostings.FirstOrDefault(j => j.JobId == user.JobId);
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
                emailCopy.Body = emailCopy.Body.Replace("@link", _configuration["Website:ClientUrl"]);

                // Send the email using the modified copy
                _sender.Send(account.Account1, emailCopy.Header, emailCopy.Body);
            }
        }

        public async Task CreateEmailTemplate(EmailTemplate? emailTemplate)
        {
            _context.EmailTemplates.Add(emailTemplate);
            await _context.SaveChangesAsync();
            _emailTemplates.Add(emailTemplate.TemplateId, emailTemplate);
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

            _emailTemplates.Remove(templateId);
        }
    }
}