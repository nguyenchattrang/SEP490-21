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
    [Route("api/Training/")]
    [ApiController]

    public class TrainingController : ControllerBase
    {
        
        private readonly RecruitXpressContext _context;
       
        public TrainingController(RecruitXpressContext context)
        {
            _context = context;
        }
        [HttpGet("getbyId")]
        public async Task<IActionResult> GetTraining(int accountId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                if (profile == null) return NotFound("Account chua co profile");

                var result = await _context.training.Where(x => x.ProfileId == profile.ProfileId).ToListAsync();
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

        //POST: api/TrainingManagement
        [HttpPost]
        public async Task<IActionResult> AddTraining(training data, int accountId)
        {
            try
            {
                if (data != null && accountId != null)
                {
                    var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                    if (profile == null) return NotFound("Account chua co profile");

                    var training1 = data;
                    training1.ProfileId = profile.ProfileId;
                    _context.training.Add(training1);
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

        //PUT: api/TrainingManagement/{id}
        [HttpPut("id")]
        public async Task<IActionResult> UpdateTraining( training data)
        {
            try
            {
                var TrainingUpdate = _context.training
                    .FirstOrDefault(x => x.TrainingId == data.TrainingId);

                if (TrainingUpdate == null) return NotFound("Khong co du lieu");
                _context.Entry(data).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Cập nhật thành công");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //DELETE: api/TrainingManagement
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteTraining(int Id)
        {
            try
            {
                var result = await _context.training.FirstOrDefaultAsync(x => x.TrainingId == Id);
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
