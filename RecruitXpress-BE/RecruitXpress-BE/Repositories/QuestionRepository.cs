using AutoMapper;
using AutoMapper.Internal;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using System.Linq;

namespace RecruitXpress_BE.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;
        public QuestionRepository(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QuestionDTO>> GetAllQuestions(QuestionRequest request)
        {
            var query = _context.Questions
                .Include(q => q.Options)
                .Where(q => q.QuestionId == q.QuestionId);

            var query1 = _context.Questions
               .Include(q => q.Options)
               .Where(q => q.Type.Contains(request.Type)).ToList();
            // Load the associated options

            if (request.QuestionId != 0)
            {
                query = query.Where(s => s.QuestionId == request.QuestionId);
            }
            if (request.Name != null)
            {
                var list = query1.Where(s => s.Question1.Contains(request.Name)).ToList();

            }
            if (request.Type != null)
            {
                query = query.Where(s => s.Type == request.Type);
            }
            if (request.CreatedBy != null)
            {
                query = query.Where(s => s.CreatedBy == request.CreatedBy);
            }
            if (request.Status != null)
            {
                query = query.Where(s => s.Status == request.Status);
            }


            if (request.SortOrder != null)
            {
                switch (request.SortOrder)
                {
                    case "name":
                        query = query.OrderBy(s => s.Question1);
                        break;
                    case "name_desc":
                        query = query.OrderByDescending(s => s.Question1);
                        break;
                    case "questionId_desc":
                        query = query.OrderByDescending(s => s.QuestionId);
                        break;
                    default:
                        query = query.OrderBy(s => s.QuestionId);
                        break;
                }
            }


            var questions = await query
             .Skip(request.Offset)
             .Take(request.Limit)
             .ToListAsync();
            var questionDTOs = _mapper.Map<List<QuestionDTO>>(questions);

            return questionDTOs;
        }






        public async Task<Question> CreateQuestion(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<Question> GetQuestionById(int questionId)
        {
            return await _context.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task<Question> UpdateQuestion(Question question)
        {
            _context.Entry(question).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<bool> DeleteQuestion(int questionId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question == null)
            {
                return false;
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
