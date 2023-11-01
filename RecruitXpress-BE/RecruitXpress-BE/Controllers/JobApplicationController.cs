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
using RecruitXpress_BE.DTO;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/JobApplication/")]
    [ApiController]

    public class JobApplicationController : ControllerBase
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;

        public JobApplicationController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpPost("PostJobApplication")]
        public async Task<IActionResult> submitJobApplication(int jobId, int accountId)
        {
            try
            {
                if(accountId == null) return BadRequest("Account is not null");
                var profile = _context.Profiles.FirstOrDefault(x => x.AccountId == accountId);
                var CV = _context.Cvtemplates.FirstOrDefault(x => x.AccountId == accountId);
                if (profile == null)
                {
                    return BadRequest("Hãy cập nhật thông tin cá nhân đầy đủ trước khi nộp hồ sơ");
                }
                if (CV == null)
                {
                    return BadRequest("Hãy cập nhật thông tin CV đầy đủ trước khi nộp hồ sơ ");
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
        public async Task<IActionResult> listJobApplication([FromQuery] JobApplicationRequest request)
        {
            try
            {
                //var listJob = await _context.JobApplications.Include(x => x.Profile)
                //    .Include(x => x.Job).Include(x=>x.Template).Where(x=> x.Status ==1 ).ToListAsync();
                //if (listJob == null) return NotFound("Khong tim thay ban ghi ");
                //return Ok(listJob);
                var query =  _context.JobApplications
                .Include(q => q.Profile)
                .Include(q => q.Job)
                .Include(q => q.Template).AsQueryable();

                if (request.Location != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Location != null && s.Job.Location.Contains(request.Location));
                }

                if (request.EmploymentType != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.EmploymentType != null && s.Job.EmploymentType.Contains(request.EmploymentType));
                }

                if (request.Industry != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Industry != null && s.Job.Industry.Contains(request.Industry));
                }

                if (request.SalaryRange != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.SalaryRange != null && s.Job.SalaryRange.Contains(request.SalaryRange));
                }  

                if (request.ApplicationDeadline != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.ApplicationDeadline != null && s.Job.ApplicationDeadline.Equals(request.ApplicationDeadline));
                }
                if (request.NameCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.Name != null && s.Profile.Name.Contains(request.NameCandidate));
                }
                if (request.PhoneCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.PhoneNumber != null && s.Profile.PhoneNumber.Contains(request.PhoneCandidate));
                }
                if (request.EmailCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.Email != null && s.Profile.Email.Contains(request.EmailCandidate));
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
                                ? query.OrderBy(j => j.Job.SalaryRange)
                                : query.OrderByDescending(j => j.Job.SalaryRange);
                            break;
                        case "ApplicationDeadline":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.ApplicationDeadline)
                                : query.OrderByDescending(j => j.Job.ApplicationDeadline);
                            break;
                        case "NameCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.Name)
                                : query.OrderByDescending(j => j.Profile.Name);
                            break;
                        case "PhoneCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.PhoneNumber)
                                : query.OrderByDescending(j => j.Profile.PhoneNumber);
                            break;
                        case "EmailCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.Email)
                                : query.OrderByDescending(j => j.Profile.Email);
                            break;

                        default:
                            query = request.OrderByAscending
                                   ? query.OrderBy(j => j.Job.ApplicationDeadline)
                                   : query.OrderByDescending(j => j.Job.ApplicationDeadline);
                            break;
                    }
                }
                var jobApplications = await query
                .Skip(request.Offset)
                 .Take(request.Limit)
                 .ToListAsync();
                var jobApplicationDTOs = _mapper.Map<List<JobApplicationDTO>>(jobApplications);

                return Ok(jobApplicationDTOs);

            }
            catch (Exception ex)
            {
               return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetSumited")]
        public async Task<IActionResult>getJobApplicationSubmitted(int accountId)
        {
            try
            {
                var account = _context.Profiles.FirstOrDefault (x=> x.AccountId== accountId);
                if(account == null)
                {
                    return BadRequest("Account khong co profile");
                }
                var listJob = await _context.JobApplications.Include(x=> x.Job).Where(x => x.ProfileId == account.ProfileId).ToListAsync();
                if(listJob == null)
                {
                    return NotFound("Khong co thong tin");
                }
                return Ok(listJob);

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
                var detailJob = await _context.JobApplications.Include(x => x.Job).Include(x=> x.Template)
                    .FirstOrDefaultAsync(x=> x.ApplicationId == jobApplyId);
                if(detailJob == null)
                {
                    return NotFound("Khong co ket qua");
                }
                return Ok(detailJob);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("PutJobApp")]
        public async Task<IActionResult> UpdatejobApplicationStatus(int jobApplyId, int? profileId,int? Status)
        {
            try
            {
                var detailJob = await _context.JobApplications.FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
                if (detailJob == null)
                {
                    return NotFound("Khong co ket qua");
                }
                if(Status != null)
                {
                    detailJob.Status = 2;
                     _context.Update(detailJob);
                    await _context.SaveChangesAsync();
                    return Ok("Cập nhật trạng thái thành công");
                }

                return Ok(detailJob);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("PutJobApp1")]
        public async Task<IActionResult> ResubmitjobApplication(int jobApplyId, int profileId)
        {
            try
            {
                var detailJob = await _context.JobApplications.FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
                if (detailJob == null)
                {
                    return NotFound("Khong tim thay ban ghi submit");
                }
                var account = _context.Profiles.FirstOrDefaultAsync(x => x.ProfileId == profileId);
                if(account == null)
                {
                    return BadRequest("Account khong co profile ");
                }
                var check = await _context.Cvtemplates.FirstOrDefaultAsync(x => x.AccountId == account.Id);
                if (check == null)
                {
                    if(check.TemplateId == detailJob.TemplateId)
                    {
                        return BadRequest("Ban da submit job nay roi, de submit lai voi CV moi hay update CV cua ban");
                    }
                    else
                    {
                        await submitJobApplication(jobApplyId, account.Id);
                        return Ok("Cap nhat CV cho job thanh cong");
                    }
                }
                return Ok("Cap nhat CV cho job thanh cong");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
