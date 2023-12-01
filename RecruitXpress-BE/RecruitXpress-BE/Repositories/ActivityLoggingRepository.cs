using AutoMapper;
using AutoMapper.Internal;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using System.Collections.Generic;
using System.Linq;

namespace RecruitXpress_BE.Repositories
{
    public class ActivityLoggingRepository
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;
        public ActivityLoggingRepository(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<QuestionDTO>> GetListActivity(QuestionRequest request)
        {
            var query = _context.Questions
                .Include(q => q.Options)
                .Include(q => q.CreatedByNavigation)
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

            var totalCount = await query.CountAsync();

            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;
            var questions = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var questionDTOs = _mapper.Map<List<QuestionDTO>>(questions);

            var response = new ApiResponse<QuestionDTO>
            {
                Items = questionDTOs,
                TotalCount = totalCount,
            };
            return response;
        }

    }
}
