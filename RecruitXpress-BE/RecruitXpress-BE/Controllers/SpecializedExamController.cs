using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/SpecializedExam")]
    [ApiController]
    public class SpecializedExamController : ControllerBase
    {
        private readonly ISpecializedExamRepository _repository;

        public SpecializedExamController(ISpecializedExamRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecializedExams([FromQuery] SpecializedExamRequest request)
        {
            var exams = await _repository.GetAllSpecializedExams(request);
            return Ok(exams);
        }

        [HttpGet("GetSpecializedExamById/{examId}")]
        public async Task<IActionResult> GetSpecializedExam(int examId)
        {
            var exam = await _repository.GetSpecializedExamById(examId);
            if (exam == null)
            {
                return NotFound();
            }
            return Ok(exam);
        }

        [HttpPost("CreateSpecializedExam")]
        public async Task<IActionResult> CreateSpecializedExam(SpecializedExam exam)
        {
            await _repository.AddSpecializedExam(exam);
            return Ok(exam);
        }

        [HttpPut("UpdateSpecializedExam/{examId}")]
        public async Task<IActionResult> UpdateSpecializedExam(int examId, SpecializedExam exam)
        {
            if (examId != exam.ExamId)
            {
                return BadRequest("Examid không khớp");
            }

            await _repository.UpdateSpecializedExam(exam);
            return Ok(exam);
        }

        [HttpDelete("DeleteSpecializedExam/{examId}")]
        public async Task<IActionResult> DeleteSpecializedExam(int examId)
        {

            var deleted = await _repository.DeleteSpecializedExam(examId);

            if (!deleted)
            {
                return NotFound("Không tìm thấy");
            }

            return NoContent();

        }
    }
}
