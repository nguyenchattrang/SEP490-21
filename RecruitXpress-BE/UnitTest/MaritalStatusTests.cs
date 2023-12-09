using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using RecruitXpress_BE.Contracts;
using RecruitXpress_BE.Controllers;
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
    public class MaritalStatusControllerTests
    {
        private IMapper _mapper;
        private MaritalStatusController _controller;


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
            _controller = new MaritalStatusController(dbContext, _mapper);

        }

        [TestMethod]
        public async Task GetMaritalStatus_ReturnsNotFound_WhenMaritalStatusNotFound()
        {
            // Arrange
            
            // Act
            var result = await _controller.GetMaritalStatus();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var notFoundResult = result as BadRequestObjectResult;
            Assert.AreEqual("Không có dữ liệu", notFoundResult.Value);

        }

        [TestMethod]
        public async Task AddMaritalStatus_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var accountId = 22; // Change this value based on your test scenario
            var maritalStatus = 1;
            // Act
            var result = await _controller.AddMaritalStatus(maritalStatus, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task AddMaritalStatusByAdmin_ReturnsOk_WhenSuccessful()
        {
            // Arrange
           // Change this value based on your test scenario
            var maritalStatus = new MaritalStatus{ Description = "Doc than" };
            // Act
            var result = await _controller.CreateNewMaritalStauts(maritalStatus);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task AddMaritalStatusByAdmin_ReturnsBadRequest_WhenFail()
        {
            // Arrange
            // Change this value based on your test scenario
            var maritalStatus = new MaritalStatus { Description= null };
            // Act
            var result = await _controller.CreateNewMaritalStauts(maritalStatus);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task AddMaritalStatus_ReturnsBadRequest_WhenMaritalStatusIsNotFound()
        {
            // Arrange
            var accountId = 22; // Change this value based on your test scenario
            var maritalStatus = 99;
            // Act
            var result = await _controller.AddMaritalStatus(maritalStatus, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task AddMaritalStatus_ReturnsBadRequest_WhenAccountIsNotFound()
        {
            // Arrange
            var accountId = 99; // Change this value based on your test scenario
            var maritalStatus = 1;
            // Act
            var result = await _controller.AddMaritalStatus(maritalStatus, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task DeleteMaritalStatus_ShouldReturnNotFound()
        {
            // Arrange
            
            var maritalStatus = 99;
            // Act
            var result = await _controller.DeleteMaritalStatus(maritalStatus);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public async Task DeleteMaritalStatus_ShouldReturnOk()
        {
            // Arrange
            var maritalStatus = 1;
            // Act
            var result = await _controller.DeleteMaritalStatus(maritalStatus);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
       
        [TestMethod]
        public async Task UpdateMaritalStatus_ShouldReturnOk()
        {
            // Arrange
            
            var maritalStatus = new MaritalStatus { StatusId =1, Description = "ly hon"};
            // Act
            var result = await _controller.UpdateMaritalStatus(maritalStatus);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task UpdateMaritalStatus_ShouldReturnBadRequest()
        {
            // Arrange
            var maritalStatus = new MaritalStatus { StatusId = 99, Description = "ly hon" };
            // Act
            var result = await _controller.UpdateMaritalStatus(maritalStatus);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
       
        // Add similar test methods for other actions (AddMaritalStatus, UpdateMaritalStatus, DeleteMaritalStatus) based on your specific requirements.
    }
}