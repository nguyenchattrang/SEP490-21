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
    public class LanguageProficiencyControllerTests
    {
        private IMapper _mapper;
        private LanguageProficiencyController _controller;


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
            _controller = new LanguageProficiencyController(dbContext, _mapper);

        }

        [TestMethod]
        public async Task GetLanguageProficiency_ValidAccountId_ReturnsOkResult()
        {
            // Arrange
            int accountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.GetLanguageProficiency(accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior
        }

        [TestMethod]
        public async Task AddLanguageProficiency_ValidData_ReturnsOkResult()
        {
            // Arrange
            List<LanguageProficiency> data = new List<LanguageProficiency>();
            int accountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.AddLanguageProficiency(data, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior
        }

        [TestMethod]
        public async Task UpdateLanguageProficiency_ValidData_ReturnsOkResult()
        {
            // Arrange
            List<LanguageProficiency> data = new List<LanguageProficiency>();

            // Act
            var result = await _controller.UpdateLanguageProficiency(data);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior
        }

        [TestMethod]
        public async Task DeleteLanguageProficiency_ValidId_ReturnsOkResult()
        {
            // Arrange
            int Id = 1; // Provide a valid LanguageProficiencyId for testing

            // Act
            var result = await _controller.DeleteLanguageProficiency(Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior
        }
        [TestMethod]
        public async Task GetLanguageProficiency_InvalidAccountId_ReturnsNotFound()
        {
            // Arrange
            int invalidAccountId = -1; // Provide an invalid account ID for testing

            // Act
            var result = await _controller.GetLanguageProficiency(invalidAccountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            // You can add more assertions based on the expected behavior
        }

        [TestMethod]
        public async Task AddLanguageProficiency_NullData_ReturnsBadRequest()
        {
            // Arrange
            List<LanguageProficiency> nullData = null;
            int accountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.AddLanguageProficiency(nullData, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            // You can add more assertions based on the expected behavior
        }

        [TestMethod]
        public async Task UpdateLanguageProficiency_NullData_ReturnsBadRequest()
        {
            // Arrange
            List<LanguageProficiency> nullData = null;

            // Act
            var result = await _controller.UpdateLanguageProficiency(nullData);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            // You can add more assertions based on the expected behavior
        }

        [TestMethod]
        public async Task DeleteLanguageProficiency_InvalidId_ReturnsNotFound()
        {
            // Arrange
            int invalidId = -1; // Provide an invalid LanguageProficiencyId for testing

            // Act
            var result = await _controller.DeleteLanguageProficiency(invalidId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            // You can add more assertions based on the expected behavior
        }
        [TestMethod]
        public async Task GetLanguageProficiency_ValidAccountId_ReturnsOkWithListOfLanguageProficiencies()
        {
            // Arrange
            int validAccountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.GetLanguageProficiency(validAccountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior, such as checking the returned data
        }

        [TestMethod]
        public async Task AddLanguageProficiency_ValidData_ReturnsOk()
        {
            // Arrange
            List<LanguageProficiency> validData = new List<LanguageProficiency>
            {
                // Provide valid LanguageProficiency data for testing
            };
            int accountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.AddLanguageProficiency(validData, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior
        }

        [TestMethod]
        public async Task UpdateLanguageProficiency_ValidData_ReturnsOk()
        {
            // Arrange
            List<LanguageProficiency> validData = new List<LanguageProficiency>
            {
                // Provide valid LanguageProficiency data for testing
            };

            // Act
            var result = await _controller.UpdateLanguageProficiency(validData);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior
        }

        [TestMethod]
        public async Task DeleteLanguageProficiency_ValidId_ReturnsOk()
        {
            // Arrange
            int validId = 1; // Provide a valid LanguageProficiencyId for testing

            // Act
            var result = await _controller.DeleteLanguageProficiency(validId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior
        }

        // Add similar test methods for other actions (AddLanguageProficiency, UpdateLanguageProficiency, DeleteLanguageProficiency) based on your specific requirements.
    }
}