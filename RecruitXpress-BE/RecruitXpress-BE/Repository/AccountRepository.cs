using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepository;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly RecruitXpressContext _context = new();
    public async Task<List<Account>> GetListAccount()
    {
        var result = await _context.Accounts.ToListAsync();
        return(result);
    }
    
    public async Task<Account?> GetAccount(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    //public async Task<Account> AddAccount(Account account)
    //{
    //    try
    //    {
    //        _context.Entry(account).State = EntityState.Added;
    //        await _context.SaveChangesAsync();
    //        return account;
    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine(e);
    //        throw;
    //    }
    //}

   // public async Task<Account> UpdateAccount(int id, Account account)
    //{
    //    _context.Entry(account).State = EntityState.Modified;
    //    try
    //    {
    //      var editAccount = await _context.Accounts.SingleOrDefaultAsync(x => x.AccountId == account.AccountId );
    //        if(editAccount != null)
    //        {
    //            var result = account;
    //            _context.Update(result);
    //            await _context.SaveChangesAsync();
    //            return result;
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        throw;
    //    }
    //}

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