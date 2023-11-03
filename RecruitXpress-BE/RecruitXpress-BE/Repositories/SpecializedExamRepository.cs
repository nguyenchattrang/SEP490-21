using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories
{
    public class SpecializedExamRepository: ISpecializedExamRepository
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;

        public SpecializedExamRepository(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SpecializedExamDTO>> GetAllSpecializedExams(SpecializedExamRequest request)
        {
            var query = _context.SpecializedExams.Include(s=> s.CreatedByNavigation).AsQueryable();

            if (!string.IsNullOrEmpty(request.ExamName))
            {
                query = query.Where(e => e.ExamName.Contains(request.ExamName));
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                query = query.Where(e => e.Description != null && e.Description.Contains(request.Description));
            }

            if (request.CreatedBy != null)
            {
                query = query.Where(e => e.CreatedBy == request.CreatedBy);
            }

            if (request.Status != null)
            {
                query = query.Where(e => e.Status == request.Status);
            }

            if (request.StartDate.HasValue)
            {
                var startDate = request.StartDate.Value.Date;
                query = query.Where(e => e.StartDate != null && e.StartDate.Value.Date == startDate);
            }

            if (request.EndDate.HasValue)
            {
                var endDate = request.EndDate.Value.Date;
                query = query.Where(e => e.EndDate != null && e.EndDate.Value.Date == endDate);
            }
            if (request.CreatedAt.HasValue)
            {
                var createAt = request.CreatedAt.Value.Date;
                query = query.Where(e => e.CreatedAt != null && e.CreatedAt.Value.Date == createAt);
            }

            if (request.SortBy != null)
            {
                switch (request.SortBy)
                {
                    case "examName":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.ExamName)
                            : query.OrderByDescending(e => e.ExamName);
                        break;

                    case "startDate":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.StartDate)
                            : query.OrderByDescending(e => e.StartDate);
                        break;
                    case "createdAt":
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.StartDate)
                            : query.OrderByDescending(e => e.StartDate);
                        break;
                    // Add other sorting options if needed.
                    default:
                        query = request.OrderByAscending
                            ? query.OrderBy(e => e.ExamId)
                            : query.OrderByDescending(e => e.ExamId);
                        break;
                }
            }

            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;

            var specializedExams = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var specializedExamDTOs = _mapper.Map<List<SpecializedExamDTO>>(specializedExams);
            return specializedExamDTOs;
        }

        public async Task<SpecializedExamDTO> GetSpecializedExamById(int examId)
        {
            var specializedExam = await _context.SpecializedExams.FindAsync(examId);

            var specializedExamDTO = _mapper.Map<SpecializedExamDTO>(specializedExam);
            return specializedExamDTO;
        }

        public async Task AddSpecializedExam(SpecializedExam exam)
        {
            exam.CreatedAt = DateTime.Now;
            _context.SpecializedExams.Add(exam);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSpecializedExam(SpecializedExam exam)
        {
            _context.Entry(exam).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteSpecializedExam(int examId)
        {

            var specializedExam =  await _context.SpecializedExams.FindAsync(examId);

            if (specializedExam == null)
            {
                return false;
            }

            _context.SpecializedExams.Remove(specializedExam);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
