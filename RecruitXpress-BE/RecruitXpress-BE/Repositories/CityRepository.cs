using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class CityRepository : ICityRepository
{
    private readonly RecruitXpressContext _context;

    public CityRepository(RecruitXpressContext context)
    {
        _context = context;
    }
    
    public async Task<City> AddCity(City city)
    {
        try
        {
            _context.Entry(city).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return city;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<City> UpdateCity(int id, City city)
    {
        try
        {
            city.CityId = id;
            if (!_context.Cities.Any(i => i.CityId == id))
            {
                throw new Exception("Không tìm thấy thông tin thành phố");
            }
            _context.Entry(city).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return city;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteCity(int id)
    {
        try
        {
            var city = _context.Cities.FirstOrDefault(i => i.CityId == id);
            if (city == null)
            {
                throw new Exception("Không tìm thấy thông tin thành phố");
            }
            _context.Entry(city).State = EntityState.Deleted;
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