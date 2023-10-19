using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.IRepository;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repository;

namespace RecruitXpress_BE.Controllers;

//[Route("api/[controller]")]
[Route("api/JobPostingManagement/")]
[ApiController]
public class JobPostingController : ControllerBase
{
    private readonly IJobPostingRepository _jobPostingRepository = new JobPostingRepository();
    
    //GET: api/JobPostingManagement
    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobPosting>>> GetListJobPostings() => await _jobPostingRepository.GetListJobPostings();
    
    //GET: api/JobPostingManagement/{id}
    [HttpGet("id")]
    public async Task<ActionResult<JobPosting>> GetJobPosting(int id)
    {
        var jobPosting = await _jobPostingRepository.GetJobPosting(id);
        if (jobPosting == null)
        {
            return NotFound();
        }

        return jobPosting;
    }

    //POST: api/JobPostingManagement
    [HttpPost]
    public async Task<ActionResult<JobPosting>> AddJobPostings(JobPosting jobPosting)
    {
        try
        {
            var result = await _jobPostingRepository.AddJobPosting(jobPosting);
            return CreatedAtAction(nameof(AddJobPostings), result);
        }
        catch (Exception e)
        {
            return BadRequest();
        }
        
    }
    
    //PUT: api/JobPostingManagement/{id}
    [HttpPut("id")]
    public async Task<ActionResult<JobPosting>> UpdateJobPosting(int id, JobPosting jobPosting)
    {
        if (id != jobPosting.JobId)
        {
            return BadRequest();
        }
        try
        {
            var result = await _jobPostingRepository.UpdateJobPostings(id, jobPosting);
            return CreatedAtAction(nameof(UpdateJobPosting), result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }

    //DELETE: api/JobPostingManagement
    [HttpDelete("id")]
    public IActionResult DeleteJobPosting(int id)
    {
        try
        {
            _jobPostingRepository.DeleteJobPosting(id);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
}