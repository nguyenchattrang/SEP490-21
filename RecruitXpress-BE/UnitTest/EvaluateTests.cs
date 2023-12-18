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
    public class EvaluateControllerTests
    {
        private IMapper _mapper;
        private EvaluateController _controller;


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
            _controller = new EvaluateController(dbContext, _mapper);

        }
        // ... (previous code)

        // ... (previous code)

        [TestMethod]
        public async Task AddEvaluate_ValidData_ReturnsOk()
        {
            // Arrange
            var validEvaluateDTO = new EvaluateDTO
            {
                JobApplicationId = 1, // Provide a valid JobApplicationId
                CalendarId = 1,       // Provide a valid CalendarId
                ProfileId = 1,        // Provide a valid ProfileId
                Comments = "Good candidate",
                Strengths = "Technical skills",
                Weaknesses = "Limited experience in project management",
                Score = 8.5,          // Provide a valid Score
                CreatedAt = DateTime.Now,
                Status = 1            // Provide a valid Status
            };

            // Act
            var result = await _controller.AddEvaluate(validEvaluateDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // Add more assertions as needed based on the expected behavior
        }

        [TestMethod]
        public async Task UpdateEvaluate_ValidData_ReturnsOk()
        {
            // Arrange
            var existingEvaluateId = 1; // Provide a valid existing EvaluateId
            var validEvaluateDTO = new EvaluateDTO
            {
                EvaluateId = existingEvaluateId,
                JobApplicationId = 2, // Provide a valid JobApplicationId
                CalendarId = 2,       // Provide a valid CalendarId
                ProfileId = 2,        // Provide a valid ProfileId
                Comments = "Updated comments",
                Strengths = "Updated technical skills",
                Weaknesses = "Updated weaknesses",
                Score = 9.0,          // Provide a valid Score
                CreatedAt = DateTime.Now.AddDays(-1), // Update CreatedAt if needed
                Status = 2            // Provide a valid Status
            };

            // Act
            var result = await _controller.UpdateEvaluate(validEvaluateDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // Add more assertions as needed based on the expected behavior
        }
        // ... (previous code)

        [TestMethod]
        public async Task ListEvaluate_WithValidFilter_ReturnsOkWithResults()
        {
            // Arrange
            var validFilter = new GetListEvaluateRequest
            {
                JobApplicationId = 1,   // Provide a valid JobApplicationId for filtering
                CalendarId = 1,         // Provide a valid CalendarId for filtering
                ProfileId = 1,          // Provide a valid ProfileId for filtering
                Comments = "Good",      // Provide a valid Comments for filtering
                Strengths = "Technical",// Provide a valid Strengths for filtering
                Weaknesses = "Limited", // Provide a valid Weaknesses for filtering
                Score = 8.0,            // Provide a valid Score for filtering
                CreatedAt = DateTime.Now.AddDays(-7), // Provide a valid CreatedAt for filtering
                Status = 1              // Provide a valid Status for filtering
            };

            // Act
            var result = await _controller.ListEvaluate(validFilter);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // Add more assertions as needed based on the expected behavior
        }

        [TestMethod]
        public async Task GetDetailEvaluate_WithValidId_ReturnsOkWithResult()
        {
            // Arrange
            var validEvaluateId = 1; // Provide a valid EvaluateId

            // Act
            var result = await _controller.GetDetailEvaluate(validEvaluateId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // Add more assertions as needed based on the expected behavior
        }

        [TestMethod]
        public async Task GetAllEvaluateOfAccount_WithValidAccountId_ReturnsOkWithResults()
        {
            // Arrange
            var validAccountId = 1; // Provide a valid AccountId

            // Act
            var result = await _controller.GetAllEvaluateOfAccount(validAccountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // Add more assertions as needed based on the expected behavior
        }

        // ... (previous code)

        [TestMethod]
        public async Task AddEvaluate_WithValidData_ReturnsOk()
        {
            // Arrange
            var validEvaluateDTO = new EvaluateDTO
            {
                JobApplicationId = 1,   // Provide a valid JobApplicationId
                CalendarId = 1,         // Provide a valid CalendarId
                Comments = "Good",      // Provide valid Comments
                Strengths = "Technical",// Provide valid Strengths
                Weaknesses = "Limited", // Provide valid Weaknesses
                Score = 8.0             // Provide a valid Score
                                        // Add other required properties based on your model
            };

            // Act
            var result = await _controller.AddEvaluate(validEvaluateDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // Add more assertions as needed based on the expected behavior
        }

        [TestMethod]
        public async Task UpdateEvaluate_WithValidData_ReturnsOk()
        {
            // Arrange
            var validEvaluateDTO = new EvaluateDTO
            {
                EvaluateId = 1,          // Provide a valid EvaluateId
                JobApplicationId = 1,    // Provide a valid JobApplicationId
                CalendarId = 1,          // Provide a valid CalendarId
                Comments = "Updated",    // Provide valid Comments
                Strengths = "Improved",  // Provide valid Strengths
                Weaknesses = "Overcome", // Provide valid Weaknesses
                Score = 9.0              // Provide a valid Score
                                         // Add other required properties based on your model
            };

            // Act
            var result = await _controller.UpdateEvaluate(validEvaluateDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // Add more assertions as needed based on the expected behavior
        }


        // Add similar test methods for other actions (AddEvaluate, UpdateEvaluate, DeleteEvaluate) based on your specific requirements.
    }
}