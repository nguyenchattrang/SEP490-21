using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using RecruitXpress_BE.Contracts;
using RecruitXpress_BE.Controllers;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Profile = RecruitXpress_BE.Models.Profile;

namespace UnitTest
{
    [TestClass]
    public class AccountControllerTests : ConfigUnitTest
    {
        private AccountController _controller;
        private IAccountRepository _accountRepository;


        [TestInitialize]
        public void Setup()
        {
            _controller = new AccountController(_accountRepository, new RecruitXpressContext(DbContextOptions));
        }

        [TestMethod]
        public async Task GetAccount_ReturnsNotFound_WhenAccountNotFound()
        {
            // Arrange
            var accountId = 99;
            // Act
            var result = await _controller.GetAccount(accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public async Task GetAccount_ReturnsOk_WhenAccountFound()
        {
            // Arrange
            var accountId = 1;
            // Act
            var result = await _controller.GetAccount(accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task AddAccountByAdmin_ReturnsOk_WhenSuccessful()
        {
            // Arrange

            var Account = new Account
            {
                AccountId = 1,
                Account1 = "sampleUsername",
                Password = "samplePassword",
                RoleId = 1,
                Token = "sampleToken",
                CreatedAt = DateTime.Now,
                Status = 1,
                FullName = "John Doe",
                Dob = new DateTime(1990, 1, 1),
                Gender = "Male",

            };
            // Act
            var result = await _controller.AddAccount(Account);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task AddAccountByAdmin_ReturnsBadRequest_WhenRoleIDInvalid()
        {
            // Arrange

            var Account = new Account
            {
                AccountId = 1,
                Account1 = "sampleUsername",
                Password = "samplePassword",
                RoleId = 99,
                Token = "sampleToken",
                CreatedAt = DateTime.Now,
                Status = 1,
                FullName = "John Doe",
                Dob = new DateTime(1990, 1, 1),
                Gender = "Male",

            };
            // Act
            var result = await _controller.AddAccount(Account);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task AddAccountByAdmin_ReturnsBadRequest_WhenAccountInforInvalid()
        {
            // Arrange

            var Account = new Account
            {
                AccountId = 1,
                Account1 = null,
                Password = "samplePassword",
                RoleId = 99,
                Token = "sampleToken",
                CreatedAt = DateTime.Now,
                Status = 1,
                FullName = null,
                Dob = new DateTime(1990, 1, 1),
                Gender = "Male",

            };
            // Act
            var result = await _controller.AddAccount(Account);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task UpdateAccountByAdmin_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var accountId = 12;
            var Account = new Account { FullName = "Dat" };
            // Act
            var result = await _controller.UpdateAccount(accountId, Account);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task UpdateAccountByAdmin_ReturnsBadRequest_WhenInvalidInfor()
        {
            // Arrange
            var accountId = 12;
            var Account = new Account { FullName = null };
            // Act
            var result = await _controller.UpdateAccount(accountId, Account);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task UpdateAccountByAdmin_ReturnsBadRequest_WhenNotfoundAccount()
        {
            // Arrange
            var accountId = 0;
            var Account = new Account { FullName = "Dat" };
            // Act
            var result = await _controller.UpdateAccount(accountId, Account);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task DeleteAccountByAdmin_ReturnsBadRequest_WhenFail()
        {
            // Arrange
            var accountId = 0;

            // Act
            var result = _controller.DeleteAccount(accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task DeleteAccount_ShouldReturnOk()
        {
            // Arrange
            var Account = 1;
            // Act
            var result = _controller.DeleteAccount(Account);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task GetAccount_WithValidId_ReturnsOkResultWithAccount()
        {
            // Arrange
            var accountId = 1;
            var mockRepository = new Mock<IAccountRepository>();
            var account = new Account { AccountId = accountId };
            mockRepository.Setup(repo => repo.GetAccount(accountId)).ReturnsAsync(account);
            var controller = new AccountController(mockRepository.Object, null);

            // Act
            var result = await controller.GetAccount(accountId);

            // Assert
           
            Assert.AreEqual(result, typeof(OkObjectResult));
        }
    }

        // Add similar tests for other actions (AddAccount, UpdateAccount, DeleteAccount, getMyCV)
    }


    // Add similar test methods for other actions (AddAccount, UpdateAccount, DeleteAccount) based on your specific requirements.
