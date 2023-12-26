using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories
{
    public class SpecializedExamRepository : ISpecializedExamRepository
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;

        public SpecializedExamRepository(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<SpecializedExamDTO>> GetAllSpecializedExams(SpecializedExamRequest request)
        {
            var query = _context.SpecializedExams.Include(s => s.CreatedByNavigation).Include(s => s.Job).AsQueryable();

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
            if (request.JobId != null)
            {
                query = query.Where(e => e.JobId == request.JobId);
            }
            if (!string.IsNullOrEmpty(request.ExpertEmail))
            {
                query = query.Where(e => e.ExpertEmail.Contains(request.ExpertEmail));
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
            if (!string.IsNullOrEmpty(request.Code))
            {
                query = query.Where(e => e.Code.Contains(request.Code));
            }

            if (!string.IsNullOrEmpty(request.SearchAll))
            {
                // Assuming that searchAll is a string that the user provides for a general search
                query = query.Where(e =>
                    e.ExamName.Contains(request.SearchAll) ||
                    (e.Description != null && e.Description.Contains(request.SearchAll)) ||
                    e.ExpertEmail.Contains(request.SearchAll) ||
                    e.Code.Contains(request.SearchAll)
                );
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
            //dem
            var totalCount = await query.CountAsync();

            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;

            var specializedExams = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var specializedExamDTOs = _mapper.Map<List<SpecializedExamDTO>>(specializedExams);



            var response = new ApiResponse<SpecializedExamDTO>
            {
                Items = specializedExamDTOs,
                TotalCount = totalCount,
            };
            return response;
        }

        public async Task<SpecializedExamDTO> GetSpecializedExamById(int examId)
        {
            var specializedExam = await _context.SpecializedExams.Include(s => s.CreatedByNavigation).Where(s => s.ExamId == examId).FirstOrDefaultAsync();

            var specializedExamDTO = _mapper.Map<SpecializedExamDTO>(specializedExam);
            return specializedExamDTO;
        }
        public async Task<SpecializedExamDTO> GetSpecializedExamByCode(string code, int accountId)
        {

            if (accountId == 0 || accountId == null)
            {
                throw new ArgumentException("Không có tài khoản");
            }

            var profile = _context.Profiles.FirstOrDefault(p => p.AccountId == accountId);
            if (profile == null)
            {
                throw new ArgumentException("Bạn vẫn chưa cập nhật hồ sơ của mình");
            }

            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("Bắt buộc phải nhập code cho bài thi");
            }


            var specExam = await _context.SpecializedExams
                    .Include(s => s.CreatedByNavigation)
                    .FirstOrDefaultAsync(s => s.Code.Contains(code));

            if (specExam == null || !specExam.Code.Equals(code))
            {
                throw new ArgumentException("Code không hợp lệ. Không tìm thấy bài thi");
            }
            if (specExam.Status == 2)
                throw new ArgumentException("Code bài thi không hợp lệ");

            if (specExam.JobId == null)
                throw new ArgumentException("Không tìm thấy công việc tương ứng gắn với bài thi này");

            var jobApplication = _context.JobApplications.FirstOrDefault(j => j.JobId == specExam.JobId && j.ProfileId == profile.ProfileId);
            if (jobApplication == null)
            {
                throw new ArgumentException("Bạn chưa đăng kí công việc này");
            }

            if (DateTime.Now < specExam.StartDate)
            {
                throw new ArgumentException("Chưa tới thời gian làm bài thi");
            }
            if (DateTime.Now > specExam.EndDate)
            {
                throw new ArgumentException("Đã vượt quá hạn nộp bài thi");
            }

            var existExams = _context.Exams.Where(e => e.SpecializedExamId == specExam.ExamId && e.AccountId == accountId).ToList();
            if (existExams != null && existExams.Any())
            {
                throw new ArgumentException("Bạn chỉ có thể làm bài thi một lần");
            }
            var specializedExamDTO = _mapper.Map<SpecializedExamDTO>(specExam);
            return specializedExamDTO;

        }

        public async Task AddSpecializedExam(SpecializedExamDTO exam)
        {
            if (exam.JobId == null || exam.JobId == 0)
            {
                throw new ArgumentException("Công việc là bắt buộc nhập");
            }
            var job = await _context.JobPostings.Where(j => j.JobId == exam.JobId).FirstOrDefaultAsync();
            if (job == null)
                throw new ArgumentException("Không tìm thấy công việc tương ứng");


            var generatedCode = GenerateUniqueCode();
            exam.Code = generatedCode;

            var newExam = new SpecializedExam
            {
                ExamName = exam.ExamName,
                Description = exam.Description,
                StartDate = exam.StartDate,
                EndDate = exam.EndDate,
                CreatedAt = DateTime.Now,
                Status = exam.Status,
                Code = generatedCode,
                JobId = exam.JobId,
            };

            _context.SpecializedExams.Add(newExam);
            await _context.SaveChangesAsync();
        }


        public async Task<SpecializedExam> UpdateSpecializedExam(SpecializedExamDTO exam)
        {
            var originalExam = await _context.SpecializedExams.FindAsync(exam.ExamId);

            if (originalExam == null)
            {
                throw new ArgumentException("Không tìm thấy exam");
            }

            if (exam.ExamName != null)
            {
                originalExam.ExamName = exam.ExamName;
            }

            if (exam.Description != null)
            {
                originalExam.Description = exam.Description;
            }

            if (exam.StartDate != null)
            {
                originalExam.StartDate = exam.StartDate;
            }

            if (exam.EndDate != null)
            {
                originalExam.EndDate = exam.EndDate;
            }

            if (exam.CreatedAt != null)
            {
                originalExam.CreatedAt = exam.CreatedAt;
            }

            if (exam.CreatedBy != null)
            {
                originalExam.CreatedBy = exam.CreatedBy;
            }

            if (exam.Status != null)
            {
                originalExam.Status = exam.Status;
            }

            if (exam.Code != null)
            {
                originalExam.Code = exam.Code;
            }
            if (exam.JobId != null)
            {
                originalExam.JobId = exam.JobId;
            }
            if (exam.JobId != null)
            {
                originalExam.JobId = exam.JobId;
            }
            if (exam.ExpertEmail != null)
            {
                originalExam.ExpertEmail = exam.ExpertEmail;
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return the updated entity
            return originalExam;
        }


        public async Task<bool> DeleteSpecializedExam(int examId)
        {

            var specializedExam = await _context.SpecializedExams.FindAsync(examId);

            if (specializedExam == null)
            {
                return false;
            }

            _context.SpecializedExams.Remove(specializedExam);
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateUniqueCode()
        {
            int maxAttempts = 10; // Set a maximum number of attempts.
            int attemptCount = 0;
            string generatedCode;

            do
            {
                // Generate a random code.
                generatedCode = TokenHelper.GenerateRandomToken(8);
                attemptCount++;

                // Check if the generated code is unique.
                var isCodeUnique = !_context.SpecializedExams.Any(e => e.Code == generatedCode);

                if (isCodeUnique)
                {
                    return generatedCode;
                }
            } while (attemptCount < maxAttempts);

            // Handle the case where a unique code couldn't be generated.
            throw new Exception("Failed to generate a unique code after multiple attempts.");
        }
    }
}
