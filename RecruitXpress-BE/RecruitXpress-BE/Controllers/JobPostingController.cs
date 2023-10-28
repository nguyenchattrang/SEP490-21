using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/JobPosting/")]
    [ApiController]
    public class JobPostingController : ControllerBase
    {
        private readonly IJobPostingRepository _jobPostingRepository = new JobPostingRepository();
        
        //GET: api/JobPosting
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetListJobPostings(string? searchString, string? orderBy, bool? isSortAscending, int page, int size) => await _jobPostingRepository.GetListJobPostings(searchString, orderBy, isSortAscending, page, size);
        
        //POST: api/JobPosting/AdvancedSearch?page={page}&size={size}
        [HttpPost("AdvancedSearch")]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetListJobPostingsAdvancedSearch(JobPostingSearchDTO jobPostingSearchDto, int page, int size)
        {
            var jobPostings = await _jobPostingRepository.GetListJobPostingAdvancedSearch(jobPostingSearchDto, page, size);
            return Ok(jobPostings);
        }

        //GET: api/JobPosting/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<JobPosting>> GetJobPosting(int id)
        {
            var jobPosting = await _jobPostingRepository.GetJobPosting(id);
            if (jobPosting == null)
            {
                return NotFound("Job posting not found!");
            }

            return jobPosting;
        }

        //POST: api/JobPosting
        [HttpPost]
        public async Task<ActionResult<JobPosting>> AddJobPosting(JobPosting jobPosting)
        {
            try
            {
                var result = await _jobPostingRepository.AddJobPosting(jobPosting);
                return CreatedAtAction(nameof(GetJobPosting), new { id = result.JobId }, result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        //PUT: api/JobPosting/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobPosting(int id, JobPosting jobPosting)
        {
            try
            {
                var result = await _jobPostingRepository.UpdateJobPostings(id, jobPosting);
                return CreatedAtAction(nameof(UpdateJobPosting), new { id = result.JobId }, result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        //DELETE: api/JobPosting/{id}
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteJobPosting(int id)
        {
            try
            {
                var deleted = await _jobPostingRepository.DeleteJobPosting(id);
                if (deleted)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }
    }
}
