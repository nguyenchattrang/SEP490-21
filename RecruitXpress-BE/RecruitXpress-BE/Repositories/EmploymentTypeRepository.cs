using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class EmploymentTypeRepository : IEmploymentTypeRepository
{
    private readonly RecruitXpressContext _context;

    public EmploymentTypeRepository(RecruitXpressContext context)
    {
        _context = context;
    }

    public async Task<EmploymentTypeResponse> GetEmploymentTypes(EmploymentTypeRequest request)
    {
        try
        {
            var query = _context.EmploymentTypes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                query = query.Where(e => e.EmploymentTypeName != null && e.EmploymentTypeName.Contains(request.Name));
            }

            if (request.SortBy == "name")
            {
                query = request.OrderByAscending
                    ? query.OrderBy(e => e.EmploymentTypeName)
                    : query.OrderByDescending(e => e.EmploymentTypeName);
            }

            var total = await query.CountAsync();

            if (string.IsNullOrWhiteSpace(request.SearchAll))
            {
                query = query.Skip((request.Page - 1) * request.Size).Take(request.Size);
            }

            return new EmploymentTypeResponse()
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

    public async Task<EmploymentType> GetEmploymentType(int id)
    {
        try
        {
            var employmentType = await _context.EmploymentTypes.FirstOrDefaultAsync(i => i.EmploymentTypeId ==  id);
            if (employmentType == null)
            {
                throw new Exception("Không tìm thấy thông tin loại công việc");
            }

            return employmentType;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<EmploymentType> AddEmploymentType(EmploymentType employmentType)
    {
        try
        {
            _context.Entry(employmentType).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return employmentType;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<EmploymentType> UpdateEmploymentType(int id, EmploymentType employmentType)
    {
        try
        {
            employmentType.EmploymentTypeId = id;
            if (!_context.EmploymentTypes.Any(i => i.EmploymentTypeId == id))
            {
                throw new Exception("Không tìm thấy thông tin loại công việc");
            }
            _context.Entry(employmentType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return employmentType;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteEmploymentType(int id)
    {
        try
        {
            var employmentType = _context.EmploymentTypes.FirstOrDefault(i => i.EmploymentTypeId == id);
            if (employmentType == null)
            {
                throw new Exception("Không tìm thấy thông tin loại công việc");
            }
            _context.Entry(employmentType).State = EntityState.Deleted;
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