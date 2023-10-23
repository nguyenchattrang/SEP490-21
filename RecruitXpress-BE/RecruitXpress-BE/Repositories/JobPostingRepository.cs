using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class JobPostingRepository : IJobPostingRepository
{
    private readonly RecruitXpressContext _context = new();
    public async Task<List<JobPosting>> GetListJobPostings() 
        => await _context.JobPostings.ToListAsync();

    public async Task<List<JobPosting>> GetListJobPostings(int page, int size) 
        => await _context.JobPostings.Skip((page - 1) * size).Take(size).ToListAsync();

    public async Task<List<JobPosting>> GetListJobPostingAdvancedSearch(JobPostingSearchDTO jobPostingSearchDto, int page, int size)
        => await GetAdvancedSearchJobPostingQuery(jobPostingSearchDto).Skip((page - 1) * size).Take(size).ToListAsync();

    public async Task<JobPosting?> GetJobPosting(int id) 
        => await _context.JobPostings.FindAsync(id);

    public async Task<JobPosting> AddJobPosting(JobPosting jobPosting)
    {
        try
        {
            _context.Entry(jobPosting).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return jobPosting;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<JobPosting> UpdateJobPostings(int id, JobPosting jobPosting)
    {
        _context.Entry(jobPosting).State = EntityState.Modified;
        try
        {
            jobPosting.JobId = id;
            await _context.SaveChangesAsync();
            return jobPosting;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteJobPosting(int jobId)
    {
        var jobPosting = await _context.JobPostings.FindAsync(jobId);
        if (jobPosting == null)
        {
            return false;
        }

        _context.Entry(jobPosting).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
        return true;
    }
    
    public IQueryable<JobPosting> GetAdvancedSearchJobPostingQuery(JobPostingSearchDTO searchDto)
        {
            var query = _context.JobPostings.AsQueryable();
    
            if (!string.IsNullOrEmpty(searchDto.SearchString))
            {
                query = query.Where(j => j.Title.Contains(searchDto.SearchString) || j.Description.Contains(searchDto.SearchString));
            }
    
            if (!string.IsNullOrEmpty(searchDto.Location))
            {
                query = query.Where(j => j.Location.Contains(searchDto.Location));
            }
    
            if (!string.IsNullOrEmpty(searchDto.EmploymentType))
            {
                query = query.Where(j => j.EmploymentType == searchDto.EmploymentType);
            }
    
            if (!string.IsNullOrEmpty(searchDto.Industry))
            {
                query = query.Where(j => j.Industry == searchDto.Industry);
            }
    
            if (!string.IsNullOrEmpty(searchDto.SalaryRange))
            {
                var salaryRange = searchDto.SalaryRange.Split("-");
                query = query.Where(j => j.MinSalary >= double.Parse(salaryRange[0]) && j.MaxSalary <= double.Parse(salaryRange[1]));
            }
    
            if (searchDto.ApplicationDeadline.HasValue)
            {
                query = query.Where(j => j.ApplicationDeadline <= searchDto.ApplicationDeadline);
            }
    
            return query;
        }
}