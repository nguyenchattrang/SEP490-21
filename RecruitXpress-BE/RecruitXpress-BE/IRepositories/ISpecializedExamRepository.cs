using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories
{
    public interface ISpecializedExamRepository
    {

        Task<ApiResponse<SpecializedExamDTO>> GetAllSpecializedExams(SpecializedExamRequest request);
        Task<SpecializedExamDTO> GetSpecializedExamById(int examId);
        Task<SpecializedExamDTO> GetSpecializedExamByCode(string code, int accountId);
        Task AddSpecializedExam(SpecializedExam exam);
        Task<SpecializedExam> UpdateSpecializedExam(SpecializedExam exam);
        Task<bool> DeleteSpecializedExam(int examId);
    }
}
