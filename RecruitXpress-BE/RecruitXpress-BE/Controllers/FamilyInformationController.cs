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
    [Route("api/FamilyInformation/")]
    [ApiController]

    public class FamilyInformationController : ControllerBase
    {
        
        private readonly RecruitXpressContext _context;
       
        public FamilyInformationController(RecruitXpressContext context)
        {
            _context = context;
        }
        [HttpGet("getbyId")]
        public async Task<IActionResult> GetFamilyInformation(int accountId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                if (profile == null) return NotFound("Account chua co profile");

                var result = await _context.FamilyInformations.Where(x => x.ProfileId == profile.ProfileId).ToListAsync();
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

        //POST: api/FamilyInformationManagement
        [HttpPost]
        public async Task<IActionResult> AddFamilyInformation(FamilyInformation familyInformation, int accountId)
        {
            try
            {
                if (familyInformation != null && accountId != null)
                {
                    var profile =await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                    if (profile == null) return NotFound("Account chua co profile");

                    var computer = familyInformation;
                    familyInformation.ProfileId = profile.ProfileId;
                    _context.FamilyInformations.Add(familyInformation);
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

        //PUT: api/FamilyInformationManagement/{id}
        [HttpPut("id")]
        public async Task<IActionResult> UpdateFamilyInformation(FamilyInformation data)
        {
            try
            {
                _context.Entry(data).State = EntityState.Detached;
               
                _context.Update(data);
                await _context.SaveChangesAsync();
                return Ok("Cập nhật thành công");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //DELETE: api/FamilyInformationManagement
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteFamilyInformation(int Id)
        {
            try
            {
                var result = await _context.FamilyInformations.FirstOrDefaultAsync(x => x.FamilyId == Id);
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
