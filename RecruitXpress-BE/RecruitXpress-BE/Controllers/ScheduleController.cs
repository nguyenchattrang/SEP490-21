﻿using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly IGoogleService _googleService;
    private readonly IScheduleRepository _scheduleRepository;

    public ScheduleController(IGoogleService googleService, IScheduleRepository scheduleRepository)
    {
        _googleService = googleService;
        _scheduleRepository = scheduleRepository;
    }
    
     //GET: api/Schedule
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetListSchedules() => await _scheduleRepository.GetListSchedules();

        // //GET: api/Schedule/{id}
        // [HttpGet("id")]
        // public async Task<ActionResult<Schedule>> GetSchedule(int id)
        // {
        //     var schedule = await _scheduleRepository.GetSchedule(id);
        //     if (schedule == null)
        //     {
        //         return NotFound("Job posting not found!");
        //     }
        //
        //     return schedule;
        // }

        //POST: api/Schedule
        [HttpPost]
        public async Task<ActionResult<Schedule>> AddSchedule(Schedule? schedule)
        {
            try
            {
                var result = await _scheduleRepository.AddSchedule(schedule);
                return CreatedAtAction(nameof(AddSchedule), new { id = result.ScheduleId }, result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Add Schedule failed!");
            }
        }

        // //PUT: api/Schedule/{id}
        // [HttpPut("id")]
        // public async Task<IActionResult> UpdateSchedule(int id, Schedule Schedule)
        // {
        //     try
        //     {
        //         var result = await _scheduleRepository.UpdateSchedules(id, Schedule);
        //         return CreatedAtAction(nameof(UpdateSchedule), new { id = result.JobId }, result);
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //         return BadRequest("Update Job Posting failed!");
        //     }
        // }
        //
        // //DELETE: api/Schedule/{id}
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteSchedule(int id)
        // {
        //     try
        //     {
        //         var deleted = await _scheduleRepository.DeleteSchedule(id);
        //         if (deleted)
        //         {
        //             return Ok();
        //         }
        //         else
        //         {
        //             return NotFound("Job Posting not found!");
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //         return StatusCode(500, "Delete Job Posting failed!");
        //     }
        // }
        //
    
}