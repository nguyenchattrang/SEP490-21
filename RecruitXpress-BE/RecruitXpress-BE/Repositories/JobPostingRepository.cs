using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class JobPostingRepository : IJobPostingRepository
{
    private readonly RecruitXpressContext _context;

    public JobPostingRepository(RecruitXpressContext context)
    {
        _context = context;
    }

    public async Task<JobPostingResponse> GetListJobPostings(string? searchString, string? sortBy,
        bool? isSortAscending, int? accountId, int? page, int? size)
    {
        var query = GetAdvancedSearchJobPostingQuery(
            new JobPostingSearchDTO()
            {
                SearchString = searchString,
                SortBy = sortBy,
                IsSortAscending = isSortAscending == true
            },
            accountId
        );
        var totalCount = await query.CountAsync();
        if (page != null && size != null)
        {
            query = query.Skip(((int)page - 1) * (int)size).Take((int)size);
        }
        var jobPostingDTO =  await query
            .Select(jobPosting => new JobPostingDTO()
            {
                JobId = jobPosting.JobId,
                Title = jobPosting.Title,
                Description = jobPosting.Description,
                Company = jobPosting.Company,
                Location = jobPosting.LocationNavigation.DistrictName + " - " + jobPosting.LocationNavigation.City.CityName,
                EmploymentType = jobPosting.EmploymentTypeNavigation.EmploymentTypeName,
                Industry = jobPosting.IndustryNavigation.IndustryName,
                LocationId = jobPosting.Location,
                EmploymentTypeId = jobPosting.EmploymentType,
                IndustryId = jobPosting.Industry,
                Requirements = jobPosting.Requirements,
                MinSalary = jobPosting.MinSalary,
                MaxSalary = jobPosting.MaxSalary,
                ApplicationDeadline = jobPosting.ApplicationDeadline,
                DatePosted = jobPosting.DatePosted,
                ContactPerson = jobPosting.ContactPerson,
                ApplicationInstructions = jobPosting.ApplicationInstructions,
                Status = jobPosting.Status,
                IsPreferred = jobPosting.WishLists.Any(w => w.AccountId == accountId),
            })
            .ToListAsync();
        return new JobPostingResponse()
        {
            Items = jobPostingDTO,
            TotalCount = totalCount
        };
    }

    public async Task<JobPostingPrepareSearch> GetJobPostingPrepareSearch()
    {
        return new JobPostingPrepareSearch()
        {
            Locations = await _context.Districts.Include(d => d.City).ToListAsync(),
            EmploymentTypes = await _context.EmploymentTypes.ToListAsync(),
            Industries = await _context.Industries.ToListAsync()
        };
    }

    public async Task<JobPostingResponse> GetListJobPostingAdvancedSearch(JobPostingSearchDTO jobPostingSearchDto, int? accountId)
    {
        try
        {
            var query = GetAdvancedSearchJobPostingQuery(jobPostingSearchDto, accountId);
            var totalCount = await query.CountAsync();
            if (jobPostingSearchDto is { Page: not null, Size: not null })
            {
                query = query.Skip(((int)jobPostingSearchDto.Page - 1) * (int)jobPostingSearchDto.Size).Take((int)jobPostingSearchDto.Size);
            }
            var jobPostingDTO =  await query
                .Select(jobPosting => new JobPostingDTO()
                {
                    JobId = jobPosting.JobId,
                    Title = jobPosting.Title,
                    Description = jobPosting.Description,
                    Company = jobPosting.Company,
                    Location = jobPosting.LocationNavigation.DistrictName + " - " + jobPosting.LocationNavigation.City.CityName,
                    EmploymentType = jobPosting.EmploymentTypeNavigation.EmploymentTypeName,
                    Industry = jobPosting.IndustryNavigation.IndustryName,
                    LocationId = jobPosting.Location,
                    EmploymentTypeId = jobPosting.EmploymentType,
                    IndustryId = jobPosting.Industry,
                    Requirements = jobPosting.Requirements,
                    MinSalary = jobPosting.MinSalary,
                    MaxSalary = jobPosting.MaxSalary,
                    ApplicationDeadline = jobPosting.ApplicationDeadline,
                    DatePosted = jobPosting.DatePosted,
                    ContactPerson = jobPosting.ContactPerson,
                    ApplicationInstructions = jobPosting.ApplicationInstructions,
                    Status = jobPosting.Status,
                    IsPreferred = jobPosting.WishLists.Any(w => w.AccountId == accountId),
                }).ToListAsync();
            return new JobPostingResponse()
            {
                Items = jobPostingDTO,
                TotalCount = totalCount
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<JobPostingDTO?> GetJobPosting(int id, int? accountId)
    {
        var jobPosting = await GetAdvancedSearchJobPostingQuery(
            new JobPostingSearchDTO() { JobId = id },
            accountId
        ).SingleOrDefaultAsync();
        if (jobPosting == null) return null;
        return  new JobPostingDTO()
        {
            JobId = jobPosting.JobId,
            Title = jobPosting.Title,
            Description = jobPosting.Description,
            Company = jobPosting.Company,
            Location = jobPosting.LocationNavigation.DistrictName + " - " + jobPosting.LocationNavigation.City.CityName,
            EmploymentType = jobPosting.EmploymentTypeNavigation.EmploymentTypeName,
            Industry = jobPosting.IndustryNavigation.IndustryName,
            LocationId = jobPosting.Location,
            EmploymentTypeId = jobPosting.EmploymentType,
            IndustryId = jobPosting.Industry,
            Requirements = jobPosting.Requirements,
            MinSalary = jobPosting.MinSalary,
            MaxSalary = jobPosting.MaxSalary,
            ApplicationDeadline = jobPosting.ApplicationDeadline,
            DatePosted = jobPosting.DatePosted,
            ContactPerson = jobPosting.ContactPerson,
            ApplicationInstructions = jobPosting.ApplicationInstructions,
            Status = jobPosting.Status,
            IsPreferred = jobPosting.WishLists.Any(w => w.AccountId == accountId),
        };
    }

    public async Task<JobPosting> AddJobPosting(JobPosting jobPosting)
    {
        try
        {
            if (jobPosting.ApplicationDeadline < DateTime.Now)
            {
                throw new Exception("Application deadline must be greater or equal now!");
            }
            jobPosting.DatePosted = DateTime.Now;
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
        if (jobPosting.ApplicationDeadline < DateTime.Now)
        {
            throw new Exception("Application deadline must be greater or equal now!");
        }
        jobPosting.DatePosted = DateTime.Now;
        jobPosting.JobId = id;
        _context.Entry(jobPosting).State = EntityState.Modified;
        try
        {
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

    private IQueryable<JobPosting> GetAdvancedSearchJobPostingQuery(JobPostingSearchDTO searchDto, int? accountId)
    {
        var query = _context.JobPostings
            .Include(j => j.EmploymentTypeNavigation)
            .Include(j => j.IndustryNavigation)
            .Include(j => j.LocationNavigation)
            .ThenInclude(d => d.City)
            .AsQueryable();

        if (accountId != null)
        {
            query = query.Include(j => j.WishLists);
        }
        
        if (searchDto.JobId != null)
        {
            query = query.Where(j =>
                j.JobId == searchDto.JobId);
        }
        
        if (!string.IsNullOrEmpty(searchDto.SearchString))
        {
            query = query.Where(j =>
                j.Title.Contains(searchDto.SearchString) || j.Description.Contains(searchDto.SearchString));
        }

        if (searchDto.LocationId != null)
        {
            query = query.Where(j => j.Location == searchDto.LocationId);
        }

        if (searchDto.EmploymentTypeId != null)
        {
            query = query.Where(j => j.EmploymentType == searchDto.EmploymentTypeId);
        }

        if (searchDto.IndustryId != null)
        {
            query = query.Where(j => j.Industry == searchDto.IndustryId);
        }

        if (searchDto is { MinSalary: not null, MaxSalary: not null })
        {
            query = query.Where(j => j.MinSalary >= searchDto.MinSalary && j.MaxSalary <= searchDto.MaxSalary);
        }

        if (searchDto.ApplicationDeadline.HasValue)
        {
            query = query.Where(j => j.ApplicationDeadline <= searchDto.ApplicationDeadline);
        }
        
        if (searchDto.status.HasValue)
        {
            query = query.Where(j => j.Status == searchDto.status);
        }

        if (!string.IsNullOrEmpty(searchDto.SortBy))
        {
            query = searchDto.SortBy switch
            {
                "Location" => searchDto.IsSortAscending
                    ? query.OrderBy(j => j.LocationNavigation.DistrictName)
                    : query.OrderByDescending(j => j.LocationNavigation.DistrictName),
                "EmploymentType" => searchDto.IsSortAscending
                    ? query.OrderBy(j => j.EmploymentTypeNavigation.EmploymentTypeName)
                    : query.OrderByDescending(j => j.EmploymentTypeNavigation.EmploymentTypeName),
                "Industry" => searchDto.IsSortAscending
                    ? query.OrderBy(j => j.IndustryNavigation.IndustryName)
                    : query.OrderByDescending(j => j.IndustryNavigation.IndustryName),
                "ApplicationDeadline" => searchDto.IsSortAscending
                    ? query.OrderBy(j => j.ApplicationDeadline)
                    : query.OrderByDescending(j => j.ApplicationDeadline),
                _ => searchDto.IsSortAscending ? query.OrderBy(j => j.JobId) : query.OrderByDescending(j => j.JobId)
            };
        }
        else
        {
            query = searchDto.IsSortAscending ? query.OrderBy(j => j.JobId) : query.OrderByDescending(j => j.JobId);
        }
        
        return query;
    }
    
}