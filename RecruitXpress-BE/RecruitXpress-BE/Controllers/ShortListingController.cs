using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using System.Security.Principal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/ShortListing/")]
    [ApiController]

    public class ShortListingController : ControllerBase
    {
        
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;

        public ShortListingController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("ShortListing")]
        public async Task<IActionResult> GetShortListing([FromQuery] ShortListingRequest request, int jobId)
        {
            try
            {
                if (jobId == null)
                {
                    return BadRequest("Thieu jobId");
                }

                //var profile = await _context.ShortListings.Include(x => x.Profile)
                //    .ThenInclude(x => x.Account).ThenInclude(x => x.CandidateCvs)
                //    .Include(x => x.Job).Where(x => x.Status == 1).ToListAsync();
                    var query = _context.ShortListings.Include(x => x.Profile)
                        .ThenInclude(x => x.Account).ThenInclude(x => x.CandidateCvs)
                        .Include(x => x.Job).Where(x => x.Status == 1 && x.JobId == jobId).AsQueryable();
                    if (request.JobTitile != null)
                    {
                        query = query.Where(s => s.Job != null && s.Job.Title != null && s.Job.Title.Equals(request.JobTitile));
                    }
                    if (request.Company != null)
                    {
                        query = query.Where(s => s.Job != null && s.Job.Company != null && s.Job.Company.Equals(request.Company));
                    }
                    if (request.NameCandidate != null)
                    {
                        query = query.Where(s => s.Profile != null && s.Profile.Account.FullName != null && s.Profile.Account.FullName.Contains(request.NameCandidate));
                    }
                    if (request.PhoneCandidate != null)
                    {
                        query = query.Where(s => s.Profile != null && s.Profile.PhoneNumber != null && s.Profile.PhoneNumber.Contains(request.PhoneCandidate));
                    }
                    if (request.EmailCandidate != null)
                    {
                        query = query.Where(s => s.Profile != null && s.Profile.Account.Account1 != null && s.Profile.Account.Account1.Contains(request.EmailCandidate));
                    }
                    if (request.SortBy != null)
                    {
                        switch (request.SortBy)
                        {
                            case "NameCandidate":
                                query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.Account.FullName)
                                    : query.OrderByDescending(j => j.Profile.Account.FullName);
                                break;
                            case "PhoneCandidate":
                                query = request.OrderByAscending
                                    ? query.OrderBy(j => j.Profile.PhoneNumber)
                                    : query.OrderByDescending(j => j.Profile.PhoneNumber);
                                break;
                            case "EmailCandidate":
                                query = request.OrderByAscending
                                    ? query.OrderBy(j => j.Profile.Account.Account1)
                                    : query.OrderByDescending(j => j.Profile.Account.Account1);
                                break;
                            case "Title":
                                query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Title)
                                    : query.OrderByDescending(j => j.Job.Title);
                                break;
                            case "Company":
                                query = request.OrderByAscending
                                    ? query.OrderBy(j => j.Job.Company)
                                    : query.OrderByDescending(j => j.Job.Company);
                                break;
                            default:
                                query = request.OrderByAscending
                                       ? query.OrderBy(j => j.ListId)
                                       : query.OrderByDescending(j => j.ListId);
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(request.SearchString))
                    {
                        query = query.Where(s => s.Profile.Account.Account1.Contains(request.SearchString) ||
                         s.Profile.PhoneNumber.Contains(request.SearchString) ||
                         s.Profile.Account.Account1.Contains(request.SearchString) ||
                         s.Job.Title.Contains(request.SearchString) ||
                         s.Job.Company.Contains(request.SearchString));

                    }

                    var totalCount = await query.CountAsync();
                    var pageNumber = request.Page > 0 ? request.Page : 1;
                    var pageSize = request.Size > 0 ? request.Size : 20;
                    var shortListings = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                    var shortListingDTOs = _mapper.Map<List<ShortListingDTO>>(shortListings);

                    var response = new ApiResponse<ShortListingDTO>
                    {
                        Items = shortListingDTOs,
                        TotalCount = totalCount,
                    };
                    return Ok(response);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("AllShortListing")]
        public async Task<IActionResult> GetAllShortListing( [FromQuery] ShortListingRequest request)
        {
            try
            {
                var query =  _context.ShortListings.Include(x => x.Profile)
                    .ThenInclude(x=>x.Account).ThenInclude(x=>x.CandidateCvs)
                    .Include(x => x.Job).Where(x => x.Status == 1).AsQueryable();
                if (request.JobTitile != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Title != null && s.Job.Title.Equals(request.JobTitile));
                }
                if (request.Company != null)
                {
                    query = query.Where(s => s.Job != null && s.Job.Company != null && s.Job.Company.Equals(request.Company));
                }
                if (request.NameCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.Account.FullName != null && s.Profile.Account.FullName.Contains(request.NameCandidate));
                }
                if (request.PhoneCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.PhoneNumber != null && s.Profile.PhoneNumber.Contains(request.PhoneCandidate));
                }
                if (request.EmailCandidate != null)
                {
                    query = query.Where(s => s.Profile != null && s.Profile.Account.Account1 != null && s.Profile.Account.Account1.Contains(request.EmailCandidate));
                }
                if (request.SortBy != null)
                {
                    switch (request.SortBy)
                    {
                        case "NameCandidate":
                            query = request.OrderByAscending
                            ? query.OrderBy(j => j.Profile.Account.FullName)
                                : query.OrderByDescending(j => j.Profile.Account.FullName);
                            break;
                        case "PhoneCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.PhoneNumber)
                                : query.OrderByDescending(j => j.Profile.PhoneNumber);
                            break;
                        case "EmailCandidate":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Profile.Account.Account1)
                                : query.OrderByDescending(j => j.Profile.Account.Account1);
                            break;
                        case "Title":
                            query = request.OrderByAscending
                            ? query.OrderBy(j => j.Job.Title)
                                : query.OrderByDescending(j => j.Job.Title);
                            break;
                        case "Company":
                            query = request.OrderByAscending
                                ? query.OrderBy(j => j.Job.Company)
                                : query.OrderByDescending(j => j.Job.Company);
                            break;
                        default:
                            query = request.OrderByAscending
                                   ? query.OrderBy(j => j.ListId)
                                   : query.OrderByDescending(j => j.ListId);
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(request.SearchString))
                {
                    query = query.Where(s => s.Profile.Account.FullName.Contains(request.SearchString) ||
                     s.Profile.PhoneNumber.Contains(request.SearchString) ||
                     s.Profile.Account.Account1.Contains(request.SearchString) ||
                     s.Job.Title.Contains(request.SearchString) ||
                     s.Job.Company.Contains(request.SearchString));

                }
                
                var totalCount = await query.CountAsync();
                var pageNumber = request.Page > 0 ? request.Page : 1;
                var pageSize = request.Size > 0 ? request.Size : 20;
                var shortListings = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var shortListingDTOs = _mapper.Map<List<ShortListingDTO>>(shortListings);

                var response = new ApiResponse<ShortListingDTO>
                {
                    Items = shortListingDTOs,
                    TotalCount = totalCount,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //POST: api/ProfileManagement
        [HttpPost]
        public async Task<IActionResult> AddToShortListing(int jobapplyId)
        {
            try
            {
                var olddata = await _context.JobApplications
                .FirstOrDefaultAsync(w => w.ApplicationId == jobapplyId);
                if (olddata.Shorted == null)
                {
                    olddata.Shorted = 1;
                }
                else if (olddata.Shorted == 1)
                {
                    olddata.Shorted = 0;
                }
                else if (olddata.Shorted == 0)
                {
                    olddata.Shorted = 1;
                }

                await _context.SaveChangesAsync();
                return Ok("Thành công");
                
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
   
       
    }
}
