using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using Org.BouncyCastle.Asn1.Ocsp;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Hub;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/JobApplication/")]
    [ApiController]

    public class JobApplicationController : ControllerBase
    {
        private readonly RecruitXpressContext _context;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IMapper _mapper;
        // private readonly IHubContext<JobApplicationStatusHub> _hubContext;
        private readonly JobApplicationStatusHub _applicationHubContext;

        public JobApplicationController(RecruitXpressContext context, IMapper mapper, IEmailTemplateRepository emailTemplateRepository, JobApplicationStatusHub applicationHubContext)
        {
            _context = context;
            _mapper = mapper;
            _emailTemplateRepository = emailTemplateRepository;
            // _hubContext = hubContext;
            _applicationHubContext = applicationHubContext;
        }
        [HttpPost("PostJobApplication")]
        public async Task<IActionResult> submitJobApplication(int jobId, int accountId)
        {
            try
            {
                if (accountId == null) return BadRequest("Account is not null");
                var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.AccountId == accountId);

                var check = await _context.JobApplications.FirstOrDefaultAsync(x => x.ProfileId == profile.ProfileId && x.JobId==jobId);
                if(check != null)
                {
                    return BadRequest("Job này đã được bạn ứng tuyển");
                }

                var CV = _context.CandidateCvs.FirstOrDefault(x => x.AccountId == accountId);
                if (profile == null)
                {
                    return BadRequest("Hãy cập nhật thông tin cá nhân đầy đủ trước khi nộp hồ sơ");
                }
                if (CV == null)
                {
                    return BadRequest("Hãy cập nhật thông tin CV đầy đủ trước khi nộp hồ sơ");
                }

                var jobApp = new JobApplication
                {
                    JobId = jobId,
                    ProfileId = profile.ProfileId,
                    TemplateId = CV.TemplateId,
                    Status = 1
                };
                _context.Add(jobApp);
                await _context.SaveChangesAsync();
                return Ok("Nộp hồ sơ thành công");


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet("AllJobApplication")]
        public async Task<IActionResult> listJobApplication([FromQuery] JobApplicationRequest request, int? accountId)
        {
            try
            {

                var query = _context.JobApplications
                .Include(q=> q.Evaluates)
                .Include(q => q.Exams)
                .Include(q => q.Profile).ThenInclude(x => x.Account)
                //.Include(q => q.Profile).ThenInclude(x=> x.Schedules).ThenInclude(x => x.ScheduleDetails)
                .Include(q => q.Profile.Evaluates)
                .Include(q => q.Profile.Account.SpecializedExams)
                .Include(ja => ja.ScheduleDetails)
                .Include(q => q.Profile).ThenInclude(x => x.GeneralTests).ThenInclude(x => x.GeneralTestDetails)
                .Include(q => q.Job).ThenInclude(j => j.IndustryNavigation)
                .Include(q => q.Job).ThenInclude(j => j.LocationNavigation)
                .Include(q => q.Job).ThenInclude(j => j.EmploymentTypeNavigation)
                .Include(q => q.Template).AsQueryable();

                if (accountId != null)
                {
                    query = query.Where(x => x.AssignedFor == accountId);
                }

                if (request.LocationId != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Location != null && s.Job.Location == request.LocationId);
                }

                if (request.EmploymentTypeId != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.EmploymentType != null && s.Job.EmploymentType == request.EmploymentTypeId);
                }

                if (request.IndustryId != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Industry != null && s.Job.Industry == request.IndustryId);
                }

                if (request is { MinSalary: not null, MaxSalary: not null })
                {
                    query = query.Where(s => s.Job != null && s.Job.MinSalary >= request.MinSalary && s.Job.MaxSalary <= request.MaxSalary);
                }

                if (request.ApplicationDeadline != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.ApplicationDeadline != null && s.Job.ApplicationDeadline.Equals(request.ApplicationDeadline));
                }
                if (request.Title != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Title != null && s.Job.Title.Equals(request.Title));
                }
                if (request.Company != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Company != null && s.Job.Company.Equals(request.Company));
                }
                if (request.NameCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.Account.FullName != null && s.Profile.Account.FullName.Contains(request.NameCandidate));
                }
                if (request.PhoneCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.PhoneNumber != null && s.Profile.PhoneNumber.Contains(request.PhoneCandidate));
                }
                if (request.EmailCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.Account.Account1 != null && s.Profile.Account.Account1.Contains(request.EmailCandidate));
                }
                if (request.Status != null)
                {
                    query = query.Where(s => s.Status != null && s.Status == (request.Status));
                }
                if (request.SortBy != null)
                {
                    switch (request.SortBy)
                    {
                        case "Location":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Location)
                                : query.OrderByDescending(j => j.Job.Location);
                            break;
                        case "Shorted":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Shorted)
                                : query.OrderByDescending(j => j.Shorted);
                            break;
                        case "Status":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Status)
                                : query.OrderByDescending(j => j.Status);
                            break;
                        case "EmploymentType":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.EmploymentType)
                                : query.OrderByDescending(j => j.Job.EmploymentType);
                            break;
                        case "Industry":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Industry)
                                : query.OrderByDescending(j => j.Job.Industry);
                            break;
                        case "SalaryRange":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.MinSalary)
                                : query.OrderByDescending(j => j.Job.MinSalary);
                            break;
                        case "ApplicationDeadline":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.ApplicationDeadline)
                                : query.OrderByDescending(j => j.Job.ApplicationDeadline);
                            break;
                        case "NameCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.Account.FullName)
                                : query.OrderByDescending(j => j.Profile.Account.FullName);
                            break;
                        case "PhoneCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.PhoneNumber)
                                : query.OrderByDescending(j => j.Profile.PhoneNumber);
                            break;
                        case "EmailCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.Account.Account1)
                                : query.OrderByDescending(j => j.Profile.Account.Account1);
                            break;
                        case "Title":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Title)
                                : query.OrderByDescending(j => j.Job.Title);
                            break;
                        case "Company":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Company)
                                : query.OrderByDescending(j => j.Job.Company);
                            break;
                        case "DatePosted":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.DatePosted)
                                : query.OrderByDescending(j => j.Job.DatePosted);
                            break;
                        default:
                            query = request.OrderByAscending
                                   ? query.OrderBy(j => j.Job.ApplicationDeadline)
                                   : query.OrderByDescending(j => j.Job.ApplicationDeadline);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(request.SearchString))
                {
                    query = query.Where(s => s.Profile.Account.Account1.Contains(request.SearchString) ||
                     s.Profile.PhoneNumber.Contains(request.SearchString) ||
                     s.Profile.Account.FullName.Contains(request.SearchString) ||
                     // s.Job.SalaryRange.Contains(request.SearchString) ||
                     s.Job.IndustryNavigation.IndustryName.Contains(request.SearchString) ||
                     s.Job.LocationNavigation.CityName.Contains(request.SearchString) ||
                     s.Job.Title.Contains(request.SearchString) ||
                     s.Job.Company.Contains(request.SearchString));

                }
                var totalCount = await query.CountAsync();
                var pageNumber = request.Page > 0 ? request.Page : 1;
                var pageSize = request.Size > 0 ? request.Size : 20;
                var jobApplications = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var jobApplicationDTOs = _mapper.Map<List<JobApplicationDTO>>(jobApplications);
                foreach (var jobApplicationDTO in jobApplicationDTOs)
                {
                    var acc = jobApplicationDTO.AssignedFor;
                    if (acc != null)
                    {

                        var profile = _context.Accounts.FirstOrDefault(x => x.AccountId == acc);
                        if (profile != null)
                        {
                            var profileDTO = new AssignedProfileDTO
                            {
                                accountId = (int)acc,
                                Name = profile.FullName,
                            };
                            jobApplicationDTO.AssignedForInfor = profileDTO;
                        }
                    }

                }
                var response = new ApiResponse<JobApplicationDTO>
                {
                    Items = jobApplicationDTOs,
                    TotalCount = totalCount,
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetSumited")]
        public async Task<IActionResult> getJobApplicationSubmitted([FromQuery] JobApplicationRequest request, int accountId)
        {
            try
            {
                var profileId = 0;
                if (accountId != null && accountId != 0)
                {
                    var getAccountId = _context.Profiles.Where(x => x.AccountId == accountId).FirstOrDefault();
                    if (getAccountId != null)
                    {
                        profileId = getAccountId.ProfileId;
                    }
                    else return BadRequest("Không tìm thấy hồ sơ ứng viên");
                }
                var query = _context.JobApplications
                .Include(q => q.Evaluates)
                .Include(q => q.Exams)
                .Include(q => q.Profile).ThenInclude(x => x.Account)
                //.Include(q => q.Profile).ThenInclude(x=> x.Schedules).ThenInclude(x => x.ScheduleDetails)
                .Include(q => q.Profile.Evaluates)
                .Include(q => q.Profile.Account.SpecializedExams)
                .Include(ja => ja.ScheduleDetails)
                .Include(q => q.Profile).ThenInclude(x => x.GeneralTests).ThenInclude(x => x.GeneralTestDetails)
                .Include(q => q.Job).ThenInclude(j => j.IndustryNavigation)
                .Include(q => q.Job).ThenInclude(j => j.LocationNavigation)
                .Include(q => q.Job).ThenInclude(j => j.EmploymentTypeNavigation)
                .Include(q => q.Template).AsQueryable();

                if (request.LocationId != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Location != null && s.Job.Location == request.LocationId);
                }

                if (request.EmploymentTypeId != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.EmploymentType != null && s.Job.EmploymentType == request.EmploymentTypeId);
                }

                if (request.IndustryId != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Industry != null && s.Job.Industry == request.IndustryId);
                }

                if (request is { MinSalary: not null, MaxSalary: not null })
                {
                    query = query.Where(s => s.Job != null && s.Job.MinSalary >= request.MinSalary && s.Job.MaxSalary <= request.MaxSalary);
                }

                if (request.ApplicationDeadline != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.ApplicationDeadline != null && s.Job.ApplicationDeadline.Equals(request.ApplicationDeadline));
                }
                if (request.Title != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Title != null && s.Job.Title.Equals(request.Title));
                }
                if (request.Company != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Company != null && s.Job.Company.Equals(request.Company));
                }
                if (request.NameCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.Account.FullName != null && s.Profile.Account.FullName.Contains(request.NameCandidate));
                }
                if (request.PhoneCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.PhoneNumber != null && s.Profile.PhoneNumber.Contains(request.PhoneCandidate));
                }
                if (request.EmailCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.Account.Account1 != null && s.Profile.Account.Account1.Contains(request.EmailCandidate));
                }
                if (request.Status != null)
                {
                    query = query.Where(s => s.Status != null && s.Status == (request.Status));
                }
                if (request.SortBy != null)
                {
                    switch (request.SortBy)
                    {
                        case "Location":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Location)
                                : query.OrderByDescending(j => j.Job.Location);
                            break;
                        case "Shorted":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Shorted)
                                : query.OrderByDescending(j => j.Shorted);
                            break;
                        case "Status":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Status)
                                : query.OrderByDescending(j => j.Status);
                            break;
                        case "EmploymentType":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.EmploymentType)
                                : query.OrderByDescending(j => j.Job.EmploymentType);
                            break;
                        case "Industry":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Industry)
                                : query.OrderByDescending(j => j.Job.Industry);
                            break;
                        case "SalaryRange":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.MinSalary)
                                : query.OrderByDescending(j => j.Job.MinSalary);
                            break;
                        case "ApplicationDeadline":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.ApplicationDeadline)
                                : query.OrderByDescending(j => j.Job.ApplicationDeadline);
                            break;
                        case "NameCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.Account.FullName)
                                : query.OrderByDescending(j => j.Profile.Account.FullName);
                            break;
                        case "PhoneCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.PhoneNumber)
                                : query.OrderByDescending(j => j.Profile.PhoneNumber);
                            break;
                        case "EmailCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.Account.Account1)
                                : query.OrderByDescending(j => j.Profile.Account.Account1);
                            break;
                        case "Title":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Title)
                                : query.OrderByDescending(j => j.Job.Title);
                            break;
                        case "Company":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Company)
                                : query.OrderByDescending(j => j.Job.Company);
                            break;
                        case "DatePosted":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.DatePosted)
                                : query.OrderByDescending(j => j.Job.DatePosted);
                            break;
                        default:
                            query = request.OrderByAscending
                                   ? query.OrderBy(j => j.Job.ApplicationDeadline)
                                   : query.OrderByDescending(j => j.Job.ApplicationDeadline);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(request.SearchString))
                {
                    query = query.Where(s => s.Profile.Account.Account1.Contains(request.SearchString) ||
                     s.Profile.PhoneNumber.Contains(request.SearchString) ||
                     s.Profile.Account.FullName.Contains(request.SearchString) ||
                     // s.Job.SalaryRange.Contains(request.SearchString) ||
                     s.Job.IndustryNavigation.IndustryName.Contains(request.SearchString) ||
                     s.Job.LocationNavigation.CityName.Contains(request.SearchString) ||
                     s.Job.Title.Contains(request.SearchString) ||
                     s.Job.Company.Contains(request.SearchString));

                }
                var totalCount = await query.CountAsync();
                var pageNumber = request.Page > 0 ? request.Page : 1;
                var pageSize = request.Size > 0 ? request.Size : 20;
                var jobApplications = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var jobApplicationDTOs = _mapper.Map<List<JobApplicationDTO>>(jobApplications);
                foreach (var jobApplicationDTO in jobApplicationDTOs)
                {
                    var acc = jobApplicationDTO.AssignedFor;
                    if (acc != null)
                    {

                        var profile = _context.Accounts.FirstOrDefault(x => x.AccountId == acc);
                        if (profile != null)
                        {
                            var profileDTO = new AssignedProfileDTO
                            {
                                accountId = (int)acc,
                                Name = profile.FullName,
                            };
                            jobApplicationDTO.AssignedForInfor = profileDTO;
                        }
                    }

                }
                var response = new ApiResponse<JobApplicationDTO>
                {
                    Items = jobApplicationDTOs,
                    TotalCount = totalCount,
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetDetail")]
        public async Task<IActionResult> jobApplicationDetail(int jobApplyId)
        {
            try
            {
                var detailJob = await _context.JobApplications.Include(x => x.Job).Include(x => x.Template)
                    .FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);

                var query = await _context.JobApplications
                .Include(q => q.Profile).ThenInclude(x => x.Account)
                //.Include(q => q.Profile).ThenInclude(x=> x.Schedules).ThenInclude(x => x.ScheduleDetails)
                .Include(q => q.Profile.Evaluates)
                .Include(q => q.Profile.Account.SpecializedExams)
                .Include(ja => ja.ScheduleDetails)
                .Include(q => q.Profile).ThenInclude(x => x.GeneralTests).ThenInclude(x => x.GeneralTestDetails)
                .Include(q => q.Job).ThenInclude(j => j.IndustryNavigation)
                .Include(q => q.Job).ThenInclude(j => j.LocationNavigation)
                .Include(q => q.Job).ThenInclude(j => j.EmploymentTypeNavigation)
                .Include(q => q.Template).FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
                if (query == null)
                {
                    return NotFound("Không có kết quả");
                }
                return Ok(query);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> UpdatejobApplicationStatus(int jobApplyId, int? accountId, int? status)
        {
            try
            {
                var detailJob = await _context.JobApplications.FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
                if (detailJob == null)
                {
                    return NotFound("Không có kết quả");
                }
                if (status != null)
                {
                    detailJob.Status = status;
                    if (accountId != null)
                    {
                        detailJob.AssignedFor = accountId;
                      await  _emailTemplateRepository.SendEmailCVToInterviewer(jobApplyId);
                    }

                    switch (status)
                    {

                        case 1:
                            await _emailTemplateRepository.SendEmailSubmitJob(jobApplyId);
                            break;
                        case 7:
                            await _emailTemplateRepository.SendEmailUpdateProfile(jobApplyId);
                            break;
                        case 8:
                            await _emailTemplateRepository.SendEmailAccepted(jobApplyId);
                            break;
                        case 9:
                            await _emailTemplateRepository.SendEmailCanceled(jobApplyId);
                            break;
                    }

                    _context.Update(detailJob);
                    await _context.SaveChangesAsync();
                    // await _hubContext.Clients.All.SendAsync("StatusChanged", jobApplyId, Status);
                    await _applicationHubContext.NotifyStatusChange(jobApplyId, (int)status);
                    return Ok("Cập nhật trạng thái thành công");
                }

                return Ok(detailJob);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Resubmit")]
        public async Task<IActionResult> ResubmitjobApplication(int jobApplyId, int profileId)
        {
            try
            {
                var detailJob = await _context.JobApplications.FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
                if (detailJob == null)
                {
                    return NotFound("Không tìm thấy bản ghi submit");
                }
                var account = _context.Profiles.FirstOrDefaultAsync(x => x.ProfileId == profileId);
                if (account == null)
                {
                    return BadRequest("Tài khoản không có profile");
                }
                var check = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.AccountId == account.Id);
                if (check == null)
                {
                    if (check.TemplateId == detailJob.TemplateId)
                    {
                        return BadRequest("Bạn đã submit job này rồi, để submit lại với CV mới hãy update CV của bạn");
                    }
                    else
                    {
                        await submitJobApplication(jobApplyId, account.Id);
                        return Ok("Cập nhật CV cho job thành công");
                    }
                }
                return Ok("Cập nhật CV cho job thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ListByJobPosting")]
        public async Task<IActionResult> ListByJobPosting(int jobId)
        {
            try
            {

                var query = await _context.JobApplications
                .Include(q => q.Profile).ThenInclude(x => x.Account)
                .Where(j=> j.JobId==jobId && j.Status==2)
                .ToListAsync();
                if (query == null)
                    return NotFound("Không kết quả");
                var jobApplicationDTOs = _mapper.Map<List<ShortJobApp>>(query);
                foreach (var jobApplicationDTO in jobApplicationDTOs)
                {
                    var acc = jobApplicationDTO.AssignedFor;
                    if (acc != null)
                    {

                        var profile = _context.Accounts.FirstOrDefault(x => x.AccountId == acc);
                        if (profile != null)
                        {
                            var profileDTO = new AssignedProfileDTO
                            {
                                accountId = (int)acc,
                                Name = profile.FullName,
                            };
                            jobApplicationDTO.AssignedForInfor = profileDTO;
                        }
                    }

                }
                return Ok(jobApplicationDTOs);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
