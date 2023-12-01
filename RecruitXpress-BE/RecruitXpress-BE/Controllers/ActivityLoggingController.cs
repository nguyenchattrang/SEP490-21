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
        public async Task<IActionResult> listAll([FromQuery]GetListActivityRequest request)
        {
            try
            {
                var query = _context.Profiles
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
                    .AsQueryable(); // Convert to queryable for dynamic filtering

                if (request.Email != null)
                {
                    query = query.Where(p => p.Account.Account1 == request.Email);
                }

                if (request.FullName != null)
                {
                    query = query.Where(p => p.Account.FullName == request.FullName);
                }

                if (request.Type != null)
                {
                 
                    if (request.Type == 1)
                    {
                        query = query.Where(p => p.JobApplications.Any());
                    }
                }

                if (request.SortBy != null)
                {
                    switch (request.SortBy)
                    {
                        case "fullName":
                            query = request.OrderByAscending
                                ? query.OrderBy(p => p.Account.FullName)
                                : query.OrderByDescending(p => p.Account.FullName);
                            break;

                        // Add more cases for other sortable properties

                        default:
                            query = request.OrderByAscending
                                    ? query.OrderBy(p => p.ProfileId)
                                    : query.OrderByDescending(p => p.ProfileId);
                            break;
                    }
                }

                var totalCount = await query.CountAsync();

                var pageNumber = request.Page > 0 ? request.Page : 1;
                var pageSize = request.Size > 0 ? request.Size : 20;
                var profiles = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var activityLoggingDTOs = _mapper.Map<List<ActivityLoggingDTO>>(profiles);

                var response = new ApiResponse<ActivityLoggingDTO>
                {
                    Items = activityLoggingDTOs,
                    TotalCount = totalCount,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
