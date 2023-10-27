using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepository;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace RecruitXpress_BE.Controllers
{
    [Route("api/JobApplication/")]
    [ApiController]

    public class JobApplicationController : ControllerBase
    {
        private readonly RecruitXpressContext _context;

        public JobApplicationController(RecruitXpressContext context)
        {
            _context = context;
        }
        [HttpPost("PostJobApplication")]
        public async Task<IActionResult> submitJobApplication(int jobId, int accountId)
        {
            try
            {
                if(accountId == null) return BadRequest("Account is not null");
                var profile = _context.Profiles.SingleOrDefault(x => x.AccountId == accountId);
                var CV = _context.Cvtemplates.SingleOrDefault(x => x.AccountId == accountId);
                if (profile == null)
                {
                    return BadRequest("Hãy cập nhật thông tin cá nhân đầy đủ trước khi nộp hồ sơ");
                }
                if (CV == null)
                {
                    return BadRequest("Hãy cập nhật thông tin CV đầy đủ trước khi nộp hồ sơ ");
                }
                else
                {
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

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet("Get")]
        public async Task<IActionResult> listJobApplication(string? search = "")
        {
            try
            {
                var listJob = await _context.JobApplications.Include(x => x.Profile).Where(x=> x.Status ==1 ).ToListAsync();
                if (listJob == null) return NotFound("Khong tim thay ban ghi ");
                return Ok(listJob);

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
                var account = await _context.Profiles.SingleOrDefaultAsync(x=> x.AccountId== accountId);
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
                var detailJob = await _context.JobApplications.FirstOrDefaultAsync(x=> x.ApplicationId == jobApplyId);
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
        [HttpPut("PutJobApp")]
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
