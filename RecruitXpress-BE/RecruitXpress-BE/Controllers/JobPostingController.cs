using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/JobPosting/")]
    [ApiController]
    public class JobPostingController : ControllerBase
    {
        private readonly IJobPostingRepository _jobPostingRepository;

        // Inject the repository through constructor
        public JobPostingController(IJobPostingRepository jobPostingRepository)
        {
            _jobPostingRepository = jobPostingRepository;
        }

        //GET: api/JobPosting
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetListJobPostings()
        {
            var jobPostings = await _jobPostingRepository.GetListJobPostings();
            return Ok(jobPostings);
        }

        //GET: api/JobPosting?page={page}&size={size}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetListJobPostings(int page, int size)
        {
            var jobPostings = await _jobPostingRepository.GetListJobPostings(page, size);
            return Ok(jobPostings);
        }

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
                return NotFound();
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
                return BadRequest();
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
        [HttpDelete("{id}")]
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
