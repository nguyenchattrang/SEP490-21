using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IJobPostingRepository
{
    Task<JobPostingResponse> GetListJobPostings(string? searchString, string? orderBy, bool? isSortAscending, int? accountId, int? page, int? size);
    Task<JobPostingPrepareSearch> GetJobPostingPrepareSearch();
    Task<List<District>> GetDistrictsByCityId(int cityId);
    Task<JobPostingResponse> GetListJobPostingAdvancedSearch(JobPostingSearchDTO jobPostingSearchDto, int? accountId);
    Task<JobPostingDTO?> GetJobPosting(int id, int? accountId);
    Task<JobPosting> AddJobPosting(JobPosting jobPosting);
    Task<JobPosting> UpdateJobPostings(int id, JobPosting jobPosting);
    Task<bool> DeleteJobPosting(int jobId);
}