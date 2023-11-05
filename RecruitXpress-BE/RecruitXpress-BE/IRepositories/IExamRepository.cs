using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories
{
    public interface IExamRepository
    {
        Task<List<ExamDTO>> GetAllExams(ExamRequest request);
        Task<List<ExamDTO>> GetListExamWithSpecializedExamId(ExamRequest request, int sid);
        Task<ExamDTO> GetExamById(int examId);
        Task<Exam> CreateExamWithFile(ExamRequestClass exam, IFormFile fileData);
        Task UpdateExam(Exam exam);
        Task GradeExam(GradeExamRequest e);
        Task<bool> DeleteExam(int examId);
    }
}
