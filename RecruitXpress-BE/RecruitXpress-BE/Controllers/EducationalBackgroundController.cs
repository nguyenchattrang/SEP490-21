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
    [Route("api/EducationBackground/")]
    [ApiController]

    public class EducationalBackgroundController : ControllerBase
    {
        
        private readonly RecruitXpressContext _context;
       
        public EducationalBackgroundController(RecruitXpressContext context)
        {
            _context = context;
        }
        [HttpGet("getbyId")]
        public async Task<IActionResult> GetEducationBackground(int accountId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                if (profile == null) return NotFound("Account chua co profile");
               
                var result = await _context.EducationalBackgrounds.Where(x => x.ProfileId == profile.ProfileId).ToListAsync();
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

        //POST: api/EducationBackgroundManagement
        [HttpPost]
        public async Task<IActionResult> AddEducationBackground(EducationalBackground educationBackground, int accountId)
        {
            try
            {
                if (educationBackground != null && accountId != null)
                {
                    var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                    if (profile == null) return NotFound("Account chua co profile");
                    
                    var computer = educationBackground;
                    educationBackground.ProfileId = profile.ProfileId;
                    _context.EducationalBackgrounds.Add(educationBackground);
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

        //PUT: api/EducationBackgroundManagement/{id}
        [HttpPut("id")]
        public async Task<IActionResult> UpdateEducationBackground(int accountId,EducationalBackground educationBackground)
        {
            try
            {
                var edu = _context.EducationalBackgrounds
                    .FirstOrDefault(x => x.EducationalBackgroundId == educationBackground.EducationalBackgroundId);
                
                if (edu == null) return NotFound("Khong co du lieu");
                _context.Entry(educationBackground).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Cập nhật thành công");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //DELETE: api/EducationBackgroundManagement
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteEducationBackground(int Id)
        {
            try
            {
                var result = await _context.EducationalBackgrounds.FirstOrDefaultAsync(x => x.EducationalBackgroundId == Id);
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
