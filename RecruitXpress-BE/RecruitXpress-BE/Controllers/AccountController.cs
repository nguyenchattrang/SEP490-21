using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/Account/")]
    [ApiController]

    public class AccountController : ControllerBase
    {
        public IAccountRepository _accountRepository;
        public AccountController( IAccountRepository repository)
        {
            
            _accountRepository = repository;
        }
        //GET: api/AccountManagement
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetListAccount()
        {
             var result =  await _accountRepository.GetListAccount();
           return Ok(result);
           
        }

        //GET: api/AccountManagement/{id}
        [HttpGet("id")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _accountRepository.GetAccount(id);
            if (account == null)
            {
                return NotFound("Không kết quả");
            }

            return account;
        }

        //POST: api/AccountManagement
        [HttpPost]
        public async Task<ActionResult<Account>> AddAccount(Account account)
        {
            try
            {
                var result = await _accountRepository.AddAccount(account);
                return CreatedAtAction(nameof(AddAccount), result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        //PUT: api/AccountManagement/{id}
        [HttpPut("id")]
        public async Task<ActionResult<Account>> UpdateAccount(int id, Account account)
        {
            //if (id != account.StatusId)
            //{
            //    return BadRequest();
            //}
            try
            {
                var result = await _accountRepository.UpdateAccount(id, account);
                return CreatedAtAction(nameof(UpdateAccount), result);
            }
            catch (Exception e)
            {

                return NotFound("Không kết quả");
            }
        }

        //DELETE: api/AccountManagement
        [HttpDelete("id")]
        public IActionResult DeleteAccount(int id)
        {
            try
            {
                _accountRepository.DeleteAccount(id);
                return Ok();
            }
            catch (Exception e)
            {
                return NotFound("Không kết quả");
            }
        }
    }
}
