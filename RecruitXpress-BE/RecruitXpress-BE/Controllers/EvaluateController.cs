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

namespace RecruitXpress_BE.Controllers
{
    [Route("api/Evaluate/")]
    [ApiController]

    public class EvaluateController : ControllerBase
    {

        private readonly RecruitXpressContext _context;
        public IMapper _mapper;

        public EvaluateController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("ListEvaluate")]
        public async Task<IActionResult> ListEvaluate(GetListEvaluateRequest request)
        {
            try
            {

                var query = _context.Evaluates.AsQueryable(); // Update with your actual DbSet

                if (request.JobApplicationId != 0)
                {
                    query = query.Where(e => e.JobApplicationId == request.JobApplicationId);
                }

                if (request.CalendarId != null)
                {
                    query = query.Where(e => e.CalendarId == request.CalendarId);
                }

                if (request.ProfileId != null)
                {
                    query = query.Where(e => e.ProfileId == request.ProfileId);
                }

                if (!string.IsNullOrEmpty(request.Comments))
                {
                    query = query.Where(e => e.Comments.Contains(request.Comments));
                }

                if (!string.IsNullOrEmpty(request.Strengths))
                {
                    query = query.Where(e => e.Strengths.Contains(request.Strengths));
                }

                if (!string.IsNullOrEmpty(request.Weaknesses))
                {
                    query = query.Where(e => e.Weaknesses.Contains(request.Weaknesses));
                }

                if (request.Score != null)
                {
                    query = query.Where(e => e.Score == request.Score);
                }

                if (request.CreatedAt != null)
                {
                    query = query.Where(e => e.CreatedAt == request.CreatedAt);
                }

                if (request.Status != null)
                {
                    query = query.Where(e => e.Status == request.Status);
                }

                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    switch (request.SortBy.ToLower())
                    {
                        case "jobapplicationid":
                            query = request.OrderByAscending
                                ? query.OrderBy(e => e.JobApplicationId)
                                : query.OrderByDescending(e => e.JobApplicationId);
                            break;

                        case "calendarid":
                            query = request.OrderByAscending
                                ? query.OrderBy(e => e.CalendarId)
                                : query.OrderByDescending(e => e.CalendarId);
                            break;

                        case "profileid":
                            query = request.OrderByAscending
                                ? query.OrderBy(e => e.ProfileId)
                                : query.OrderByDescending(e => e.ProfileId);
                            break;

                        default:
                            // Default sorting by JobApplicationId if no valid field is specified
                            query = request.OrderByAscending
                                ? query.OrderBy(e => e.JobApplicationId)
                                : query.OrderByDescending(e => e.JobApplicationId);
                            break;
                    }
                }

                var totalCount = await query.CountAsync();

                var pageNumber = request.Page > 0 ? request.Page : 1;
                var pageSize = request.Size > 0 ? request.Size : 20;
                var evaluates = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var evaluateDTOs = _mapper.Map<List<EvaluateDTO>>(evaluates);

                var response = new ApiResponse<EvaluateDTO>
                {
                    Items = evaluateDTOs,
                    TotalCount = totalCount,
                };

                return Ok(response);
            }
            catch
            {
                return StatusCode(500);
            }
        }


        [HttpGet("getbyid")]
        public async Task<IActionResult> GetDetailEvaluate(int evaluateId)
        {
            try
            {

                var evaluate = await _context.Evaluates.FirstOrDefaultAsync(x => x.EvaluateId == evaluateId);
                if (evaluate == null)
                {
                    return NotFound("Không kết quả");
                }
                var data = _mapper.Map<EvaluateDTO>(evaluate);
                return Ok(data);
            }
            catch
            {
                return StatusCode(500);
            }
        }
        //GET: api/ProfileManagement/{id}
        [HttpGet("evaluateByAccount")]
        public async Task<IActionResult> GetAllEvaluateOfAccount(int accountId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(x => x.AccountId == accountId);
                if (profile == null)
                {
                    return BadRequest("Account chua co du profile");
                }
                var evaluate = await _context.Evaluates.Where(x => x.ProfileId == profile.ProfileId).ToListAsync();
                if (evaluate == null)
                {
                    return NotFound("Không kết quả");
                }
                var data = _mapper.Map<List<EvaluateDTO>>(evaluate);
                return Ok(data);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEvaluate(EvaluateDTO evaluate)
        {
            try
            {

                if (evaluate != null)
                {
                    if (evaluate.CalendarId == null)
                        return BadRequest("Không có dữ liệu lịch tương ứng");
                    var calendar = await _context.Calendars.Where(j => j.Id == evaluate.CalendarId).FirstOrDefaultAsync();
                    var jobApp = await _context.JobApplications.Where(j => j.ApplicationId == calendar.JobApplicationId).FirstOrDefaultAsync();

                    var data = evaluate;
                    data.JobApplicationId = jobApp.ApplicationId;
                    data.ProfileId = jobApp.ProfileId;
                    data.CreatedAt = DateTime.Now;
                    data.Status = 1;
                    var create = _mapper.Map<Evaluate>(data);
                    _context.Evaluates.Add(create);
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
        [HttpPut("updateEvaluate")]
        public async Task<IActionResult> UpdateEvaluate(EvaluateDTO evaluateDTO)
        {
            try
            {
                var existingEvaluate = await _context.Evaluates.FindAsync(evaluateDTO.EvaluateId);

                if (existingEvaluate == null)
                {
                    return NotFound("Không tìm thấy đánh giá");
                }

                // Check if CalendarId is provided and exists
                if (evaluateDTO.CalendarId != null)
                {
                    var calendar = await _context.Calendars
                        .Where(c => c.Id == evaluateDTO.CalendarId)
                        .FirstOrDefaultAsync();

                    if (calendar == null)
                    {
                        return BadRequest("Không tìm thấy lịch tương ứng");
                    }

                    // Update fields based on the existing JobApplication
                    var jobApp = await _context.JobApplications
                        .Where(j => j.ApplicationId == calendar.JobApplicationId)
                        .FirstOrDefaultAsync();

                    existingEvaluate.JobApplicationId = jobApp.ApplicationId;
                    existingEvaluate.ProfileId = jobApp.ProfileId;
                    if (evaluateDTO.Status != null)
                        existingEvaluate.Status = evaluateDTO.Status;
                    if (!string.IsNullOrEmpty(evaluateDTO.Comments))
                        existingEvaluate.Comments = evaluateDTO.Comments;
                    if (!string.IsNullOrEmpty(evaluateDTO.Strengths))
                        existingEvaluate.Strengths = evaluateDTO.Strengths;
                    if (!string.IsNullOrEmpty(evaluateDTO.Weaknesses))
                        existingEvaluate.Weaknesses = evaluateDTO.Weaknesses;
                }

                _mapper.Map(evaluateDTO, existingEvaluate);

                _context.Entry(existingEvaluate).State = EntityState.Modified;
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
