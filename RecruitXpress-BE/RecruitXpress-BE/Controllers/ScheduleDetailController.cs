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

    //PUT: api/Schedule/{id}
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
    //PUT: api/Schedule/{id}
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