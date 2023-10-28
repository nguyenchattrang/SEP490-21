using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<QuestionDTO>> GetAllQuestions(QuestionRequest request);

        Task CreateQuestion(Question question);

        Task<QuestionDTO> GetQuestionById(int questionId);

        Task<Question> UpdateQuestion(Question question);

        Task<bool> DeleteQuestion(int questionId);
    }
}
