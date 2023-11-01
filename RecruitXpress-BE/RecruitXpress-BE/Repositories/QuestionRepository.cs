using AutoMapper;
using AutoMapper.Internal;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using System.Collections.Generic;
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
                .Include(q=> q.CreatedByNavigation)
                .Where(q => q.QuestionId == q.QuestionId);

            if (request.Name != null)
            {
                query = query.Where(s => s.Question1.Contains(request.Name));

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


            if (request.SortBy != null)
            {
                switch (request.SortBy)
                {
                    case "name":
                        query = request.OrderByAscending
                            ? query.OrderBy(j => j.Question1)
                            : query.OrderByDescending(j => j.Question1);
                        break;

                    case "questionId":
                        query = request.OrderByAscending
                            ? query.OrderBy(j => j.QuestionId)
                            : query.OrderByDescending(j => j.QuestionId);
                        break;

                    default:
                        query = request.OrderByAscending
                               ? query.OrderBy(j => j.QuestionId)
                               : query.OrderByDescending(j => j.QuestionId);
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






        public async Task CreateQuestion(Question questiondto)
        {


            /*var questions = _mapper.Map<Question>(questiondto);*/
            _context.Questions.Add(questiondto);
            await _context.SaveChangesAsync();

        }

        public async Task<QuestionDTO> GetQuestionById(int questionId)
        {
            var question = await _context.Questions
                     .Include(q => q.Options)
                     .FirstOrDefaultAsync(q => q.QuestionId == questionId);
            return _mapper.Map<QuestionDTO>(question);
        }

        public async Task<Question> UpdateQuestion(Question question)
        {
            _context.Entry(question).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<bool> DeleteQuestion(int questionId)
        {

            var options = _context.Options.Where(o => o.QuestionId == questionId);

            // Delete options first
            _context.Options.RemoveRange(options);

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
