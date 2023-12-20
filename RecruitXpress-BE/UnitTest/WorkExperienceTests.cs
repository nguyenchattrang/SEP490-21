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
    public class WorkExperienceControllerTests
    {
        private IMapper _mapper;
        private WorkExperienceController _controller;


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
            _controller = new WorkExperienceController(dbContext, _mapper);

        }

        // Add more test cases as needed to cover different scenarios

        [TestMethod]
        public async Task GetWorkExperience_InvalidAccountId_ReturnsNotFound()
        {
            // Arrange
            int invalidAccountId = -1; // Provide an invalid account ID for testing

            // Act
            var result = await _controller.GetWorkExperience(invalidAccountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            // You can add more assertions based on the expected behavior for invalid input
        }

        [TestMethod]
        public async Task AddWorkExperience_NullData_ReturnsBadRequest()
        {
            // Arrange
            List<WorkExperience> nullData = null;
            int accountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.AddWorkExperience(nullData, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            // You can add more assertions based on the expected behavior for null input
        }

        [TestMethod]
        public async Task UpdateWorkExperience_NullData_ReturnsBadRequest()
        {
            // Arrange
            List<WorkExperience> nullData = null;

            // Act
            var result = await _controller.UpdateWorkExperience(nullData);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            // You can add more assertions based on the expected behavior for null input
        }

        [TestMethod]
        public async Task DeleteWorkExperience_InvalidId_ReturnsNotFound()
        {
            // Arrange
            int invalidId = -1; // Provide an invalid WorkExperienceId for testing

            // Act
            var result = await _controller.DeleteWorkExperience(invalidId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            // You can add more assertions based on the expected behavior for invalid input
        }
        // Add more test cases as needed to cover different scenarios

        [TestMethod]
        public async Task GetWorkExperience_ValidAccountId_ReturnsOkWithWorkExperiences()
        {
            // Arrange
            int validAccountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.GetWorkExperience(validAccountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.IsNotNull(okResult.Value);
            // Add more assertions to validate the content of the result based on your implementation
        }

        [TestMethod]
        public async Task AddWorkExperience_ValidData_ReturnsOk()
        {
            // Arrange
            List<WorkExperience> validData = new List<WorkExperience>
    {
        // Provide valid WorkExperience objects for testing
        new WorkExperience { /* Set properties */ },
        new WorkExperience { /* Set properties */ }
    };
            int accountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.AddWorkExperience(validData, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // Add more assertions based on the expected behavior for valid input
        }

        [TestMethod]
        public async Task UpdateWorkExperience_ValidData_ReturnsOk()
        {
            // Arrange
            List<WorkExperience> validData = new List<WorkExperience>
    {
        // Provide valid WorkExperience objects for testing
        new WorkExperience { /* Set properties */ },
        new WorkExperience { /* Set properties */ }
    };

            // Act
            var result = await _controller.UpdateWorkExperience(validData);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // Add more assertions based on the expected behavior for valid input
        }

        [TestMethod]
        public async Task DeleteWorkExperience_ValidId_ReturnsOk()
        {
            // Arrange
            int validId = 1; // Provide a valid WorkExperienceId for testing

            // Act
            var result = await _controller.DeleteWorkExperience(validId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // Add more assertions based on the expected behavior for valid input
        }


        // Add similar test methods for other actions (AddWorkExperience, UpdateWorkExperience, DeleteWorkExperience) based on your specific requirements.
    }
}