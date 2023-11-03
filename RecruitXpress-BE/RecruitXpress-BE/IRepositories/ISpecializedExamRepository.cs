using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories
{
    public interface ISpecializedExamRepository
    {

        Task<IEnumerable<SpecializedExamDTO>> GetAllSpecializedExams(SpecializedExamRequest specializedExamRequest);
        Task<SpecializedExamDTO> GetSpecializedExamById(int examId);
        Task<SpecializedExamDTO> GetSpecializedExamByCode(string code);
        Task AddSpecializedExam(SpecializedExam exam);
        Task UpdateSpecializedExam(SpecializedExam exam);
        Task<bool> DeleteSpecializedExam(int examId);
    }
}
