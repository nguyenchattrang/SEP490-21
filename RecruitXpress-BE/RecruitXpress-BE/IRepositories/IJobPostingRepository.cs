using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IJobPostingRepository
{
    Task<List<JobPosting>> GetListJobPostings();
    Task<List<JobPostingDTO>> GetListJobPostings(string? searchString, string? orderBy, bool? isSortAscending, int? accountId, int? page, int? size);
    Task<List<JobPosting>> GetListJobPostingAdvancedSearch(JobPostingSearchDTO jobPostingSearchDto, int? accountId, int page, int size);
    Task<JobPosting?> GetJobPosting(int id);
    Task<JobPosting> AddJobPosting(JobPosting jobPosting);
    Task<JobPosting> UpdateJobPostings(int id, JobPosting jobPosting);
    Task<bool> DeleteJobPosting(int jobId);
}