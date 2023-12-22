using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using System.Diagnostics;
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
                var jobpost = await _context.JobPostings.FirstOrDefaultAsync(x => x.JobId == jobId);

                if (jobpost != null)
                {
                    if (jobpost.NumOfCandidate == 0) return BadRequest("Nộp hồ sơ thất bại, công việc này đã tuyển đủ số lượng ứng viên");
                }
                if (profile == null)
                {
                    return BadRequest("Hãy cập nhật thông tin cá nhân đầy đủ trước khi nộp hồ sơ");
                }

                var check = await _context.JobApplications.FirstOrDefaultAsync(x => x.ProfileId == profile.ProfileId && x.JobId == jobId);
                if (check != null)
                {
                    return BadRequest("Công việc này đã được bạn ứng tuyển");
                }

                var CV = _context.CandidateCvs.FirstOrDefault(x => x.AccountId == accountId);

                if (CV == null)
                {
                    return BadRequest("Hãy cập nhật thông tin CV đầy đủ trước khi nộp hồ sơ");
                }

                var jobApp = new JobApplication
                {
                    JobId = jobId,
                    ProfileId = profile.ProfileId,
                    TemplateId = CV.TemplateId,
                    UrlCandidateCV = CV.Url,
                    Status = 1,
                    CreatedAt = DateTime.Now
                };
                try
                {
                    string sourceFolderPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));
                    var filePath = CV.Url;
                    string destinationFolderPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\JobApplicationsCV"));
                    string fileName = CV.Url; // replace with the actual file name

                    // Combine source folder path and file name
                    string sourceFilePath = sourceFolderPath + fileName;

                    // Combine destination folder path and file name
                    string destinationFilePath = destinationFolderPath + fileName;

                    if (!Directory.Exists(sourceFolderPath))
                    {
                        Directory.CreateDirectory(sourceFolderPath);
                    }

                    if (!Directory.Exists(destinationFolderPath))
                    {
                        Directory.CreateDirectory(destinationFolderPath);
                    }
                    // Check if the source file exists
                    if (System.IO.File.Exists(sourceFilePath))
                    {
                        try
                        {
                            // Copy the file to the destination folder
                            System.IO.File.Copy(sourceFilePath, destinationFilePath, true);

                        }
                        catch (Exception ex)
                        {
                            return BadRequest($"Lỗi khi copying file: {ex.Message}");
                        }
                    }
                    else
                    {
                        return BadRequest("Lỗi khi copying file");
                    }
                }
                catch (Exception ex)
                {

                }

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
                        case "CreatedAt":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.CreatedAt)
                                : query.OrderByDescending(j => j.CreatedAt);
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
                    var getAccountId = await _context.Profiles.Where(x => x.AccountId == accountId).FirstOrDefaultAsync();
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
                .Include(q => q.Template).Where(x => x.ProfileId == profileId)
                .AsQueryable();

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
                        case "CreatedAt":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.CreatedAt)
                                : query.OrderByDescending(j => j.CreatedAt);
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
        public async Task<IActionResult> UpdatejobApplicationStatus(int jobApplyId, int? accountId, int? status, string? commentHr)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var detailJob = await _context.JobApplications
                    .Include(ja => ja.Profile)
                    .ThenInclude(p => p.Account)
                    .FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
                if (detailJob == null)
                {
                    return NotFound("Không có kết quả");
                }
                if (status != null)
                {
                    var oldStatus = detailJob.Status ?? -1;
                    detailJob.Status = status;
                    if (accountId != null)
                    {
                        detailJob.AssignedFor = accountId;
                        await _emailTemplateRepository.SendEmailCVToInterviewer(jobApplyId);
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
                    if (status == 8)
                    {
                        var jobpost = await _context.JobPostings.FirstOrDefaultAsync(x => x.JobId == detailJob.JobId);
                        if (jobpost != null)
                        {
                            if (jobpost.NumOfCandidate > 0) jobpost.NumOfCandidate = jobpost.NumOfCandidate - 1;
                            if (jobpost.NumOfCandidate == 0)
                            {
                                var listApply = await _context.JobApplications.Where(x => x.JobId == detailJob.JobId && x.Status != 8).ToListAsync();
                                foreach (var candidateApply in listApply)
                                {
                                    candidateApply.Status = 0;
                                }
                            }
                        }
                    }
                    if(commentHr != null)
                    {
                        detailJob.CommentHR = commentHr;
                    }
                    _context.Update(detailJob);
                    await _context.SaveChangesAsync();
                    await _applicationHubContext.NotifyStatusUpgrade(detailJob, (int)status, oldStatus);
                    stopwatch.Stop();
                    Console.WriteLine($"Thời gian thực hiện: {stopwatch.ElapsedMilliseconds} ms");
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
        public async Task<IActionResult> ListByJobPosting(int jobId, int status)
        {
            try
            {
                var query = _context.JobApplications
                    .Include(q => q.Profile).ThenInclude(x => x.Account)
                    .Where(ja => ja.JobId == jobId).AsQueryable();
                query = status switch
                {
                    2 => query.Where(ja => ja.Status == 2 || ja.Status == 3),
                    5 => query.Where(ja => ja.Status == 5 || ja.Status == 6),
                    _ => query.Where(ja => true)
                };

                // var query = await _context.JobApplications
                // .Include(q => q.Profile).ThenInclude(x => x.Account)
                // .Where(j=> j.JobId==jobId && (status != null ? j.Status == status : j.Status == j.Status))
                // .ToListAsync();
                var result = await query.ToListAsync();
                if (result.Count < 0)
                    return NotFound("Không kết quả");
                var jobApplicationDTOs = _mapper.Map<List<ShortJobApp>>(result);

                var listExams = await _context.SpecializedExams
                    .Where(j => j.JobId == jobId && j.Status == 1)
                    .OrderByDescending(j => j.ExamId)
                    .ToListAsync();

                var listMap = _mapper.Map<List<SpecializedExamDTO>>(listExams);

                var location = await _context.JobPostings.Where(j => j.JobId == jobId).Select(j => j.DetailLocation)
                    .FirstOrDefaultAsync();

                foreach (var jobApplicationDTO in jobApplicationDTOs)
                {
                    var acc = jobApplicationDTO.AssignedFor;
                    if (acc != null)
                    {
                        var profile = _context.Accounts.FirstOrDefault(x => x.AccountId == acc);
                        if (profile != null)
                        {
                            var profileDTO = new InterviewerProfile
                            {
                                AccountId = (int)acc,
                                Name = profile.FullName,
                                Email = profile.Account1
                            };
                            jobApplicationDTO.AssignedForInfor = profileDTO;
                        }
                    }
                }

                return Ok(new JobSearchReponse
                {
                    ListCandidates = jobApplicationDTOs,
                    ListSpecializedExams = listMap,
                    Location = location,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("TopJobPosting")]
        public async Task<IActionResult> TopJobPosting(DateTime? from, DateTime? to, int? accountId)
        {
            try
            {
                var query = _context.JobApplications.Include(j => j.Job).AsQueryable();

                if (from.HasValue)
                {
                    query = query.Where(j => j.CreatedAt > from);
                }
                if (to.HasValue)
                {
                    query = query.Where(j => j.CreatedAt < to);
                }

                var result = query
      .GroupBy(j => j.JobId)
      .Select(g => new
      {
          JobId = g.Key,
          JobTitle = g.First().Job.Title,
          Company = g.First().Job.Company,
          Description = g.First().Job.Description,
          Location = g.First().Job.Location,
          EmploymentType = g.First().Job.EmploymentType,
          Industry = g.First().Job.Industry,
          DetailLocation = g.First().Job.DetailLocation,
          Requirements = g.First().Job.Requirements,
          Benefit = g.First().Job.Benefit,
          NumOfCandidate = g.First().Job.NumOfCandidate,
          ApplicationDeadline = g.First().Job.ApplicationDeadline,
          DatePosted = g.First().Job.DatePosted,
          ContactPerson = g.First().Job.ContactPerson,
          ApplicationInstructions = g.First().Job.ApplicationInstructions,
          Status = g.First().Job.Status,
          MinSalary = g.First().Job.MinSalary,
          MaxSalary = g.First().Job.MaxSalary,
          EmploymentTypeNavigation = g.First().Job.EmploymentTypeNavigation,
          IndustryNavigation = g.First().Job.IndustryNavigation,
          LocationNavigation = g.First().Job.LocationNavigation,
          IsPreferred = accountId != null && g.First().Job.WishLists.Any(w => w.AccountId == accountId),
          Count = g.Count()
      })
      .OrderByDescending(j => j.Count)
      .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
