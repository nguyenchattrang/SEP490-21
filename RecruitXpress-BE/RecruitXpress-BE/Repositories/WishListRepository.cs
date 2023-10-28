using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class WishListRepository : IWishListRepository
{
    private readonly RecruitXpressContext _context = new();

    public async Task<List<WishList>> GetListWishLists()
        => await _context.WishLists.ToListAsync();

    public async Task<List<WishList>> GetListWishLists(string? searchString, string? sortBy,
        bool? isSortAscending, int? page, int? size)
        => await GetAdvancedSearchWishListQuery(
            new JobPostingSearchDTO()
            {
                SearchString = searchString,
                SortBy = sortBy,
                IsSortAscending = isSortAscending == true
            },
            page,
            size
        ).ToListAsync();
    
    public async Task<WishList?> GetWishList(int id)
        => await _context.WishLists.FindAsync(id);

    public async Task<WishList> AddWishList(WishList wishList)
    {
        try
        {
            _context.Entry(wishList).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return wishList;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteWishList(int jobId)
    {
        var wishList = await _context.WishLists.FindAsync(jobId);
        if (wishList == null)
        {
            return false;
        }

        _context.Entry(wishList).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
        return true;
    }

    private IQueryable<WishList> GetAdvancedSearchWishListQuery(JobPostingSearchDTO searchDto, int? page, int? size)
    {
        var query = _context.WishLists
            .Where(wishList => wishList.JobId == wishList.Job.JobId).AsQueryable();

        if (!string.IsNullOrEmpty(searchDto.SearchString))
        {
            query = query.Where(w => 
                w.Job.Title.Contains(searchDto.SearchString) || w.Job.Description.Contains(searchDto.SearchString));
        }

        if (!string.IsNullOrEmpty(searchDto.Location))
        {
            query = query.Where(w => w.Job.Location.Contains(searchDto.Location));
        }

        if (!string.IsNullOrEmpty(searchDto.EmploymentType))
        {
            query = query.Where(w => w.Job.EmploymentType == searchDto.EmploymentType);
        }

        if (!string.IsNullOrEmpty(searchDto.Industry))
        {
            query = query.Where(w => w.Job.Industry == searchDto.Industry);
        }

        // if (!string.IsNullOrEmpty(searchDto.SalaryRange))
        // {
        //     var salaryRange = searchDto.SalaryRange.Split("-");
        //     query = query.Where(w => w.Job.MinSalary >= double.Parse(salaryRange[0]) && j.MaxSalary <= double.Parse(salaryRange[1]));
        // }

        if (searchDto.ApplicationDeadline.HasValue)
        {
            query = query.Where(w => w.Job.ApplicationDeadline <= searchDto.ApplicationDeadline);
        }

        if (!string.IsNullOrEmpty(searchDto.SortBy))
        {
            query = searchDto.SortBy switch
            {
                "Location" => searchDto.IsSortAscending
                    ? query.OrderBy(w => w.Job.Location)
                    : query.OrderByDescending(w => w.Job.Location),
                "EmploymentType" => searchDto.IsSortAscending
                    ? query.OrderBy(w => w.Job.EmploymentType)
                    : query.OrderByDescending(w => w.Job.EmploymentType),
                "Industry" => searchDto.IsSortAscending
                    ? query.OrderBy(w => w.Job.Industry)
                    : query.OrderByDescending(w => w.Job.Industry),
                "ApplicationDeadline" => searchDto.IsSortAscending
                    ? query.OrderBy(w => w.Job.ApplicationDeadline)
                    : query.OrderByDescending(w => w.Job.ApplicationDeadline),
                _ => searchDto.IsSortAscending ? query.OrderBy(w => w.Job.JobId) : query.OrderByDescending(w => w.Job.JobId)
            };
        }
        
        return query.Skip(((page ?? 1) - 1) * (size ?? 20)).Take(size ?? 20);
    }
    
}