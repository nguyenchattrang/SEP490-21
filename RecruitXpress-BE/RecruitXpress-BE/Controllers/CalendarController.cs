using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Configuration;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;
using System.Security.Cryptography;
using RecruitXpress_BE.Helper;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using System.Reflection.Metadata;
using Constant = RecruitXpress_BE.Helper.Constant;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Repositories;
using System.ComponentModel.DataAnnotations;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/calendars")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarRepository _calendarRepository;

        public CalendarController(ICalendarRepository calendarRepository)
        {
            _calendarRepository = calendarRepository;
        }

        // GET api/calendars/list
        [HttpGet("ListCalendar")]
        public async Task<IActionResult> GetListByAccountIdDate([Required]int accountId, int? year, int? month, int? day)
        {
            try
            {
                var calendarDTOs = await _calendarRepository.GetListByAccountIdDate(accountId, year, month, day);
                return Ok(calendarDTOs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/calendars/{id}
        [HttpGet("GetCalendarById/{id}")]
        public async Task<IActionResult> GetCalendarById(int id)
        {
            try
            {
                var calendarDTO = await _calendarRepository.GetById(id);
                if (calendarDTO == null)
                    return NotFound("Không tìm thấy lịch");

                return Ok(calendarDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/calendars
        [HttpPost("Create")]
        public async Task<IActionResult> CreateCalendar(Calendar calendar)
        {
            try
            {
                var newCalendarId = await _calendarRepository.Create(calendar);
                return CreatedAtAction(nameof(GetCalendarById), new { id = newCalendarId }, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("CreateMultipleCandidates")]
        public async Task<IActionResult> CreateMultipleCandidatesCalendar(CalendarMultipleCandidatesRequest request)
        {
            try
            {
                var newCalendar = await _calendarRepository.CreateMultipleCandidates(request.CandidateIds,request.Calendar);
                return Ok(newCalendar);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/calendars/{id}
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateCalendar(Calendar calendar)
        {
            try
            {
                var updatedCalendarDTO = await _calendarRepository.Update(calendar);
                if (updatedCalendarDTO == null)
                    return NotFound("Không tìm thấy lịch");

                return Ok(updatedCalendarDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/calendars/{id}
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteCalendar(int id)
        {
            try
            {
                var deleted = await _calendarRepository.Delete(id);
                if (deleted==0)
                    return NotFound("Không tìm thấy lịch");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}