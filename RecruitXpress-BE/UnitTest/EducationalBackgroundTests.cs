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
    public class EducationalBackgroundControllerTests
    {
        private IMapper _mapper;
        private EducationalBackgroundController _controller;


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
            _controller = new EducationalBackgroundController(dbContext, _mapper);

        }

        // Add more test cases as needed to cover different scenarios

        [TestMethod]
        public async Task GetEducationBackground_InvalidAccountId_ReturnsNotFound()
        {
            // Arrange
            int invalidAccountId = -1; // Provide an invalid account ID for testing

            // Act
            var result = await _controller.GetEducationBackground(invalidAccountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            // You can add more assertions based on the expected behavior for invalid input
        }

        [TestMethod]
        public async Task AddEducationBackground_NullData_ReturnsBadRequest()
        {
            // Arrange
            List<EducationalBackground> nullData = null;
            int accountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.AddEducationBackground(nullData, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            // You can add more assertions based on the expected behavior for null input
        }

        [TestMethod]
        public async Task UpdateEducationBackground_NullData_ReturnsBadRequest()
        {
            // Arrange
            int accountId = 1; // Provide a valid account ID for testing
            EducationalBackground nullData = null;

            // Act
            var result = await _controller.UpdateEducationBackground(accountId, nullData);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            // You can add more assertions based on the expected behavior for null input
        }

        [TestMethod]
        public async Task DeleteEducationBackground_InvalidId_ReturnsNotFound()
        {
            // Arrange
            int invalidId = -1; // Provide an invalid EducationalBackgroundId for testing

            // Act
            var result = await _controller.DeleteEducationBackground(invalidId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            // You can add more assertions based on the expected behavior for invalid input
        }
        // Add more test cases as needed to cover different scenarios

        [TestMethod]
        public async Task GetEducationBackground_ValidAccountId_ReturnsOkWithResults()
        {
            // Arrange
            int validAccountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.GetEducationBackground(validAccountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior for valid input
        }

        [TestMethod]
        public async Task AddEducationBackground_ValidData_ReturnsOk()
        {
            // Arrange
            List<EducationalBackground> validData = new List<EducationalBackground>
    {
        new EducationalBackground { /* Provide valid properties for testing */ }
    };
            int accountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.AddEducationBackground(validData, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior for valid input
        }

        [TestMethod]
        public async Task UpdateEducationBackground_ValidData_ReturnsOk()
        {
            // Arrange
            int accountId = 1; // Provide a valid account ID for testing
            EducationalBackground validData = new EducationalBackground { /* Provide valid properties for testing */ };

            // Act
            var result = await _controller.UpdateEducationBackground(accountId, validData);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior for valid input
        }

        [TestMethod]
        public async Task DeleteEducationBackground_ValidId_ReturnsOk()
        {
            // Arrange
            int validId = 1; // Provide a valid EducationalBackgroundId for testing

            // Act
            var result = await _controller.DeleteEducationBackground(validId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior for valid input
        }


        // Add similar test methods for other actions (AddEducationalBackground, UpdateEducationalBackground, DeleteEducationalBackground) based on your specific requirements.
    }
}