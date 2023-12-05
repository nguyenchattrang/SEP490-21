using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScheduleDetailController : ControllerBase
{
    private IScheduleDetailRepository _scheduleDetailRepository;

    public ScheduleDetailController(IScheduleDetailRepository scheduleDetailRepository)
    {
        _scheduleDetailRepository = scheduleDetailRepository;
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetScheduleEvaluate(int id)
    {
        try
        {
            var result = await _scheduleDetailRepository.GetScheduleDetail(id);
            return CreatedAtAction(nameof(GetScheduleEvaluate), new { id = result.ScheduleId }, result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }
    
    [HttpPut("id")]
    public async Task<IActionResult> UpdateScheduleEvaluate(int id, ScheduleDetail scheduleDetail)
    {
        try
        {
            var result = await _scheduleDetailRepository.UpdateScheduleDetail(id, scheduleDetail);
            return CreatedAtAction(nameof(UpdateScheduleEvaluate), new { id = result.ScheduleId }, result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }
    
    [HttpDelete("id")]
    public async Task<IActionResult> DeleteScheduleEvaluate(int id)
    {
        try
        {
            await _scheduleDetailRepository.DeleteScheduleDetail(id);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
    }
}