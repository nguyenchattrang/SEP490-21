using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.IRepository;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repository;
using System.Collections.Generic;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/Profile/")]
    [ApiController]

    public class ProfileController : ControllerBase
    {
        
        private readonly RecruitXpressContext _context;
       
        public ProfileController(RecruitXpressContext context)
        {
            _context = context;
        }
        //GET: api/ProfileManagement
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Profile>>> GetListProfile()
        //{
        //     var result =  await _context.GetListProfile();
        //   return Ok(result);
        //    return Ok(new ResponseData<Models.Profile>
        //    {

        //        Data = result.Skip(offset).Take(result.Count()).ToList(),
        //        Offset = offset,
        //        Limit = limit,
        //        Total = result.Count()

        //    });
        //}

        //GET: api/ProfileManagement/{id}
        [HttpGet("id")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound("Không kết quả");
            }

            return Ok(profile);
        }

        //POST: api/ProfileManagement
        [HttpPost]
        public async Task<ActionResult<Profile>> AddProfile(Profile Profile, int accountId)
        {
            try
            {
                var profile = Profile;
                profile.AccountId = accountId;
                await _context.Profiles.AddAsync(Profile);
                await _context.SaveChangesAsync();
                return Ok("Thành công");
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }

        }

        //PUT: api/ProfileManagement/{id}
        [HttpPut("id")]
        public async Task<IActionResult> UpdateProfile(int accountId, Profile Profile)
        {
            //if (id != Profile.StatusId)
            //{
            //    return BadRequest();
            //}
            try
            {
                var profile = await _context.Profiles.FindAsync(accountId);
                if(profile != null)
                {
                    _context.Update(profile);
                    await _context.SaveChangesAsync();
                }
                else
                {
                        return BadRequest("Không có dữ liệu");
                }
                return Ok("Thành công");
            }
            catch (Exception e)
            {

                return NotFound("Không kết quả");
            }
        }

        //DELETE: api/ProfileManagement
   
       
    }
}
