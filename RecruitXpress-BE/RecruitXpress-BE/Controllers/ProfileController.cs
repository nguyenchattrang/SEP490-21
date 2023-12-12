using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using System.Security.Principal;
using Profile = RecruitXpress_BE.Models.Profile;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/Profile/")]
    [ApiController]

    public class ProfileController : ControllerBase
    {

        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;
        public ProfileController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            try
            {

                var profile = await _context.Profiles.Include(p=>p.Account).FirstOrDefaultAsync(x => x.AccountId == id);
                if (profile == null)
                {
                    return NotFound("Không kết quả");
                }
                var profileDTO= _mapper.Map<ProfileDTO>(profile);
                return Ok(profileDTO);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //POST: api/ProfileManagement
        [HttpPost]
        public async Task<IActionResult> AddProfile(ProfileDTO Profile, int accountId)
        {
            try
            {

                if (Profile != null && accountId != null)
                {
                    var profile = new Profile
                    {
                        AccountId = accountId,
                        StatusId = Profile.StatusId,
                        PhoneNumber = Profile.PhoneNumber,
                        Address = Profile.Address,
                        Avatar = Profile.Avatar,
                        Skills = Profile.Skills,
                        Accomplishment = Profile.Accomplishment,
                        Strength = Profile.Strength,
                        Imperfection = Profile.Imperfection,
                        ResearchWork = Profile.ResearchWork,
                        Article = Profile.Article
                    };
                    _context.Profiles.Add(profile);

                    var account = await _context.Accounts.FindAsync(accountId);

                    if (account != null)
                    {
                        account.FullName = Profile.Name;
                        account.Dob = Profile.Dob;
                        account.Gender = Profile.Gender;
 
                    }

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

        //PUT: api/ProfileManagement/{id}
        [HttpPut("id")]
        public async Task<IActionResult> UpdateProfile(int accountId, ProfileDTO Profile)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.ProfileId == Profile.ProfileId && x.AccountId == accountId);

                if (profile != null)
                {
                    profile.StatusId = Profile.StatusId;
                    profile.PhoneNumber = Profile.PhoneNumber;
                    profile.Address = Profile.Address;
                    profile.Avatar = Profile.Avatar;
                    profile.Skills = Profile.Skills;
                    profile.Accomplishment = Profile.Accomplishment;
                    profile.Strength = Profile.Strength;
                    profile.Imperfection = Profile.Imperfection;
                    profile.ResearchWork = Profile.ResearchWork;
                    profile.Article = Profile.Article;

                    var account = await _context.Accounts.FindAsync(accountId);

                    if (account != null)
                    {
                        account.FullName = Profile.Name;
                        account.Dob = Profile.Dob;
                        account.Gender = Profile.Gender;
                    }

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
        //DELETE: api/ProfileManagement


    }
}
