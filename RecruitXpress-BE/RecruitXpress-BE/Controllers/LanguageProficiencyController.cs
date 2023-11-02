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
    [Route("api/LanguageProficiency/")]
    [ApiController]

    public class LanguageProficiencyController : ControllerBase
    {
        
        private readonly RecruitXpressContext _context;
       
        public LanguageProficiencyController(RecruitXpressContext context)
        {
            _context = context;
        }
        [HttpGet("getbyId")]
        public async Task<IActionResult> GetLanguageProficiency(int accountId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                if (profile == null) return NotFound("Account chua co profile");

                var result = await _context.LanguageProficiencies.Where(x => x.ProfileId == profile.ProfileId).ToListAsync();
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

        //POST: api/LanguageProficiencyManagement
        [HttpPost]
        public async Task<IActionResult> AddLanguageProficiency(LanguageProficiency data, int accountId)
        {
            try
            {
                if (data != null && accountId != null)
                {
                    var profile =await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                    if (profile == null) return NotFound("Account chua co profile");

                    var computer = data;
                    data.ProfileId = profile.ProfileId;
                    _context.LanguageProficiencies.Add(data);
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

        //PUT: api/LanguageProficiencyManagement/{id}
        [HttpPut("id")]
        public async Task<IActionResult> UpdateLanguageProficiency( LanguageProficiency data)
        {
            try
            {
                var LanguageProficiencyUpdate = _context.LanguageProficiencies
                    .FirstOrDefault(x => x.LanguageProficiencyId == data.LanguageProficiencyId);

                if (LanguageProficiencyUpdate == null) return NotFound("Khong co du lieu");
                _context.Entry(data).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Cập nhật thành công");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //DELETE: api/LanguageProficiencyManagement
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteLanguageProficiency(int Id)
        {
            try
            {
                var result = await _context.LanguageProficiencies.FirstOrDefaultAsync(x => x.LanguageProficiencyId == Id);
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
