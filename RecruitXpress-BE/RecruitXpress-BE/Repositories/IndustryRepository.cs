using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class IndustryRepository : IIndustryRepository
{
    private readonly RecruitXpressContext _context;

    public IndustryRepository(RecruitXpressContext context)
    {
        _context = context;
    }

    public async Task<IndustryResponse> GetIndustries(IndustryRequest request)
    {
        try
        {
            var query = _context.Industries.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                query = query.Where(e => e.IndustryName != null && e.IndustryName.Contains(request.Name));
            }

            if (request.SortBy == "name")
            {
                query = request.OrderByAscending
                    ? query.OrderBy(e => e.IndustryName)
                    : query.OrderByDescending(e => e.IndustryName);
            }

            var total = await query.CountAsync();

            if (string.IsNullOrWhiteSpace(request.SearchAll))
            {
                query = query.Skip((request.Page - 1) * request.Size).Take(request.Size);
            }

            return new IndustryResponse()
            {
                TotalCount = total,
                Items = await query.ToListAsync()
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Industry> GetIndustry(int id)
    {
        try
        {
            var industry = await _context.Industries.FirstOrDefaultAsync(i => i.IndustryId ==  id);
            if (industry == null)
            {
                throw new Exception("Không tìm thấy thông tin vị trí công việc");
            }

            return industry;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Industry> AddIndustry(Industry industry)
    {
        try
        {
            _context.Entry(industry).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return industry;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Industry> UpdateIndustry(int id, Industry industry)
    {
        try
        {
            industry.IndustryId = id;
            if (!_context.Industries.Any(i => i.IndustryId == id))
            {
                throw new Exception("Không tìm thấy thông tin vị trí công việc");
            }
            _context.Entry(industry).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return industry;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteIndustry(int id)
    {
        try
        {
            var industry = _context.Industries.FirstOrDefault(i => i.IndustryId == id);
            if (industry == null)
            {
                throw new Exception("Không tìm thấy thông tin vị trí công việc");
            }
            _context.Entry(industry).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}