using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using System.Security.Principal;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/WorkExperience/")]
    [ApiController]

    public class WorkExperienceController : ControllerBase
    {
        
        private readonly RecruitXpressContext _context;
       
        public WorkExperienceController(RecruitXpressContext context)
        {
            _context = context;
        }
        [HttpGet("getbyId")]
        public async Task<IActionResult> GetWorkExperience(int accountId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                if (profile == null) return NotFound("Account chua co profile");

                var result = await _context.WorkExperiences.Where(x => x.ProfileId == profile.ProfileId).ToListAsync();
                if (result == null)
                {
                    return NotFound("Không kết quả");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //POST: api/WorkExperienceManagement
        [HttpPost]
        public async Task<IActionResult> AddWorkExperience(WorkExperience data, int accountId)
        {
            try
            {
                if (data != null && accountId != null)
                {
                    var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                    if (profile == null) return NotFound("Account chua co profile");

                    var WorkExperience1 = data;
                    WorkExperience1.ProfileId = profile.ProfileId;
                    _context.WorkExperiences.Add(WorkExperience1);
                    await _context.SaveChangesAsync();
                    return Ok("Thành công");
                }
                else
                {
                    return BadRequest("Không có dữ liệu");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //PUT: api/WorkExperienceManagement/{id}
        [HttpPut("id")]
        public async Task<IActionResult> UpdateWorkExperience( WorkExperience data)
        {
            try
            {
                _context.Entry(data).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Cập nhật thành công");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //DELETE: api/WorkExperienceManagement
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteWorkExperience(int Id)
        {
            try
            {
                var result = await _context.WorkExperiences.FirstOrDefaultAsync(x => x.WorkExperienceId == Id);
                if (result != null)
                {
                    _context.Remove(result);
                    await _context.SaveChangesAsync();
                    return Ok("Thành công");
                }
                else
                {
                    return NotFound("Không tồn tại");
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    }
}
