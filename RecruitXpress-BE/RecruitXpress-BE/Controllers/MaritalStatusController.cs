using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.IRepository;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repository;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/MaritalStatus/")]
    [ApiController]

    public class MaritalStatusController : ControllerBase
    {
        public readonly IMaritalStatusRepository _maritalStatusRepository;

        //GET: api/MaritalStatusManagement
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaritalStatus>>> GetListMaritalStatus() => await _maritalStatusRepository.GetListMaritalStatus();

        //GET: api/MaritalStatusManagement/{id}
        [HttpGet("id")]
        public async Task<ActionResult<MaritalStatus>> GetMaritalStatus(int id)
        {
            var maritalStatus = await _maritalStatusRepository.GetMaritalStatus(id);
            if (maritalStatus == null)
            {
                return NotFound();
            }

            return maritalStatus;
        }

        //POST: api/MaritalStatusManagement
        [HttpPost]
        public async Task<ActionResult<MaritalStatus>> AddMaritalStatus(MaritalStatus maritalStatus)
        {
            try
            {
                var result = await _maritalStatusRepository.AddMaritalStatus(maritalStatus);
                return CreatedAtAction(nameof(AddMaritalStatus), result);
            }
            catch (Exception e)
            {
                return BadRequest();
            }

        }

        //PUT: api/MaritalStatusManagement/{id}
        [HttpPut("id")]
        public async Task<ActionResult<MaritalStatus>> UpdateMaritalStatus(int id, MaritalStatus maritalStatus)
        {
            //if (id != maritalStatus.StatusId)
            //{
            //    return BadRequest();
            //}
            try
            {
                var result = await _maritalStatusRepository.UpdateMaritalStatus(id, maritalStatus);
                return CreatedAtAction(nameof(UpdateMaritalStatus), result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        //DELETE: api/MaritalStatusManagement
        [HttpDelete("id")]
        public IActionResult DeleteMaritalStatus(int id)
        {
            try
            {
                _maritalStatusRepository.DeleteMaritalStatus(id);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }
    }
}
