using AutoMapper;
using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RecruitXpress_BE.Repositories;

public class AccountRepository : IAccountRepository
{
  
    private readonly RecruitXpressContext _context;
    private readonly IMapper _mapper;
    public IConfiguration _configuration;
    public AccountRepository(RecruitXpressContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
    }
    public async Task<ApiResponse<AccountInfoDTO>> GetListAccount([FromQuery] AccountRequest request)
    {
        var query = _context.Accounts.Include(x => x.Profiles).Include(x => x.Role).AsQueryable();

        if (request.Username != null)
        {
            query = query.Where(x => x.Account1 == request.Username);
        }
        if (request.FullName != null)
        {
            query = query.Where(x => x.FullName == request.FullName);
        }
        if (request.Gender != null)
        {
            query = query.Where(x => x.Gender == request.Gender);
        }
        if (request.RoleId != null)
        {
            query = query.Where(x => x.RoleId == request.RoleId);
        }
        if (request.Dob != null)
        {
            query = query.Where(x => x.Dob == request.Dob);
        }
        if (request.Status != null)
        {
            query = query.Where(x => x.Status == request.Status);
        }
        if (request.SortBy != null)
        {
            switch (request.SortBy)
            {
                case "FullName":
                    query = request.OrderByAscending
                        ? query.OrderBy(j => j.FullName)
                        : query.OrderByDescending(j => j.FullName);
                    break;
                case "Status":
                    query = request.OrderByAscending
                        ? query.OrderBy(j => j.Status)
                        : query.OrderByDescending(j => j.Status);
                    break;
                case "Username":
                    query = request.OrderByAscending
                        ? query.OrderBy(j => j.FullName)
                        : query.OrderByDescending(j => j.FullName);
                    break;
                case "Gender":
                    query = request.OrderByAscending
                        ? query.OrderBy(j => j.Gender)
                        : query.OrderByDescending(j => j.Gender);
                    break;
                case "Dob":
                    query = request.OrderByAscending
                        ? query.OrderBy(j => j.Dob)
                        : query.OrderByDescending(j => j.Dob);
                    break;
                case "RoleId":
                    query = request.OrderByAscending
                        ? query.OrderBy(j => j.RoleId)
                        : query.OrderByDescending(j => j.RoleId);
                    break;
                default:
                    query = request.OrderByAscending
                           ? query.OrderByDescending(j => j.AccountId)
                           : query.OrderBy(j => j.AccountId);
                    break;
            }
        }
            if (!string.IsNullOrEmpty(request.SearchAll))
            {
                query = query.Where(s => s.Account1.Contains(request.SearchAll) ||
                 s.FullName.Contains(request.SearchAll) ||
                 s.RoleId == request.RoleId || 
                 s.Gender.Contains(request.SearchAll));

            }

            var totalCount = await query.CountAsync();
            var pageNumber = request.Page > 0 ? request.Page : 1;
            var pageSize = request.Size > 0 ? request.Size : 20;
            var accountList = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var accountDTOs = _mapper.Map<List<AccountInfoDTO>>(accountList);

            var response =  new ApiResponse<AccountInfoDTO>
            {
                Items = accountDTOs,
                TotalCount = totalCount
            };

            return response;
    }

    public async Task<Account?> GetAccount(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<Account> AddAccount(Account account)
    {
        try
        {
            var check = await _context.Accounts.FirstOrDefaultAsync(x => x.Account1 == account.Account1);

            if (check != null)
            {
                throw new Exception("Tài khoản đã tồn tại");
            }

            var user = new Account
            {
                Account1 = account.Account1,
                Password = account.Password != null ? HashHelper.Encrypt(account.Password, _configuration) : null,
                FullName = account.FullName,
                Gender= account.Gender,
                Dob = account.Dob,
                RoleId = account.RoleId,
                CreatedAt = DateTime.Now,
                Status = account.Status
            };

            _context.Entry(user).State = EntityState.Added;
            
            await _context.SaveChangesAsync();

            return user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Account> UpdateAccount(int id, Account account)
    {
        if(account.Password != null)
        {
            account.Password = HashHelper.Encrypt(account.Password, _configuration);
        }
        _context.Entry(account).State = EntityState.Modified;
        try
        {
            var editAccount = await _context.Accounts.SingleOrDefaultAsync(x => x.AccountId == account.AccountId);
            if (editAccount != null)
            {
                var result = account;
                _context.Update(result);
                await _context.SaveChangesAsync();
                return result;
            }
            else
            {
                return null;
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public void DeleteAccount(int AccountId)
    {
        var account = _context.Accounts.Find(AccountId);
        if (account == null)
        {
            throw new Exception();
        }

        account.Status = 2;
        _context.SaveChanges();

    }
}