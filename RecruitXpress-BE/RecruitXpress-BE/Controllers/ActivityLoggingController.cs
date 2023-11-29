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
    [Route("api/ActivityLogging/")]
    [ApiController]

    public class ActivityLoggingController : ControllerBase
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;

        public ActivityLoggingController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
       
        [HttpGet("activityLogging")]
        public async Task<IActionResult> listAll(int? accountId)
        {
            try
            {
                var query = await _context.Profiles
                    .Include(x => x.Account).ThenInclude(x => x.SpecializedExams)
                    .Include(x => x.Account).ThenInclude(x => x.CandidateCvs)
                    .Include(x => x.Evaluates)
                    .Include(x => x.ComputerProficiencies)
                    .Include(x => x.training)
                    .Include(x => x.LanguageProficiencies)
                    .Include(x => x.EducationalBackgrounds)
                    .Include(x => x.FamilyInformations)
                    .Include(x => x.WorkExperiences)
                    .Include(x => x.JobApplications)
                    .Where(x => x.AccountId == accountId)
                    .FirstOrDefaultAsync();
               

                var activityLoggingDTO = _mapper.Map<ActivityLoggingDTO>(query);
                if(activityLoggingDTO == null)
                {
                    return BadRequest("không có kêt quả ")
                }
                return Ok(activityLoggingDTO);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
    }
}
