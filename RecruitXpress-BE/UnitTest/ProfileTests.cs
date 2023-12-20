using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using RecruitXpress_BE.Contracts;
using RecruitXpress_BE.Controllers;
using RecruitXpress_BE.DTO;
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
    public class ProfileControllerTests
    {
        private IMapper _mapper;
        private ProfileController _controller;


        [TestInitialize]
        public void Setup()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var options = new DbContextOptionsBuilder<RecruitXpressContext>()
     .UseSqlServer(config.GetConnectionString("RecruitXpress")) // Use the appropriate provider (e.g., UseSqlServer for SQL Server)
     .Options;

            // Create an instance of the context using the actual database
            var dbContext = new RecruitXpressContext(options);

            // Configure AutoMapper with your mapping profile
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            // Create an instance of the mapper
            _mapper = mappingConfig.CreateMapper();

            // Create an instance of your controller with the mock context and mapper
            _controller = new ProfileController(dbContext, _mapper);

        }

        [TestMethod]
        public async Task GetProfile_ReturnsNotFound_WhenProfileNotFound()
        {
            // Arrange
            int accountId = 100;
            // Act
            var result = await _controller.GetProfile(accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));


        }

        [TestMethod]
        public async Task AddProfile_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var accountId = 1; // Change this value based on your test scenario
            var ProfileDto = new ProfileDTO { AccountId = 1, Address = "HN", Gender = "1", PhoneNumber = "1123" };
            // Act
            var result = await _controller.AddProfile(ProfileDto, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
      
        [TestMethod]
        public async Task AddProfile_ReturnsBadRequest_WhenFail()
        {
            // Arrange
            var accountId = 99; // Change this value based on your test scenario
            var ProfileDto = new ProfileDTO { ProfileId = 1, AccountId = 1, Address = "HN", Gender = "1", PhoneNumber = "1123" };
            // Act
            var result = await _controller.AddProfile(ProfileDto, accountId);


            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task AddProfile_ReturnsBadRequest_WhenAccountIsNotFound()
        {
            // Arrange
            var accountId = 99; // Change this value based on your test scenario
            var ProfileDto = new ProfileDTO { AccountId = 1, Address = "HN", Gender = "1", PhoneNumber = "1123" };
            // Act
            var result = await _controller.AddProfile(ProfileDto, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
       
        [TestMethod]
        public async Task UpdateProfile_ShouldReturnOk()
        {
            // Arrange

            var accountId = 22; // Change this value based on your test scenario
            var ProfileDto = new ProfileDTO { ProfileId = 1, AccountId = 1, Address = "HN", Gender = "1", PhoneNumber = "1123" };

            // Act
            var result = await _controller.UpdateProfile(accountId, ProfileDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task UpdateProfile_ShouldReturnBadRequest()
        {
            // Arrange
            var accountId = 99; // Change this value based on your test scenario
            var ProfileDto = new ProfileDTO { ProfileId = 1, AccountId = 1, Address = "HN", Gender = "1", PhoneNumber = "1123" };

            // Act
            var result = await _controller.UpdateProfile(accountId, ProfileDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
       
        // Add similar test methods for other actions (AddProfile, UpdateProfile, DeleteProfile) based on your specific requirements.
    }
}