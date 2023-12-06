﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/MaritalStatus/")]
    [ApiController]

    public class MaritalStatusController : ControllerBase
    {
        private readonly RecruitXpressContext _context;
        public IMapper _mapper;
        public MaritalStatusController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //GET: api/MaritalStatusManagement
        [HttpGet("get")]
        public async Task<IActionResult> GetMaritalStatus()
        {
            try
            {
                var result = await _context.MaritalStatuses.ToArrayAsync();
                if (result == null)
                {
                    return NotFound( "Không có dữ liệu ");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Không có dữ liệu");
            }
           
        }

        //POST: api/MaritalStatusManagement
        [HttpPost]
        public async Task<IActionResult> AddMaritalStatus(int maritalStatus,int accountId)
        {
            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.AccountId == accountId);
                if (profile == null) return NotFound("Account chua co profile");
               
                profile.StatusId = maritalStatus;
                _context.SaveChanges();
                return Ok("Thêm thành công");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        //PUT: api/MaritalStatusManagement/{id}
        [HttpPut("id")]
        public async Task<IActionResult> UpdateMaritalStatus(MaritalStatus maritalStatus)
        {
           
            try
            {
                _context.Entry(maritalStatus).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok("Cập nhật thành công");
            }
            catch (Exception e)
            {
                
                return BadRequest(e.Message);
            }
        }

        //DELETE: api/MaritalStatusManagement
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteMaritalStatus(int martialStatusId)
        {
            try
            {
                var result = await _context.MaritalStatuses.FirstOrDefaultAsync(x => x.StatusId == martialStatusId);
                if (result != null)
                {
                    _context.Remove(result);
                    await _context.SaveChangesAsync();
                    return Ok("Thành công");
                }
                else
                {
                    return NotFound("Không tồn tại");
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
