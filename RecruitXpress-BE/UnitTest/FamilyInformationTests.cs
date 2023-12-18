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
    public class FamilyInformationControllerTests
    {
        private IMapper _mapper;
        private FamilyInformationController _controller;


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
            _controller = new FamilyInformationController(dbContext, _mapper);

        }

        [TestMethod]
        public async Task GetFamilyInformation_ValidAccountId_ReturnsOkWithListOfFamilyInformation()
        {
            // Arrange
            int validAccountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.GetFamilyInformation(validAccountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior, such as checking the returned data
        }

        [TestMethod]
        public async Task AddFamilyInformation_ValidData_ReturnsOk()
        {
            // Arrange
            List<FamilyInformation> validData = new List<FamilyInformation>
            {
                // Provide valid FamilyInformation data for testing
            };
            int accountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.AddFamilyInformation(validData, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior
        }

        [TestMethod]
        public async Task UpdateFamilyInformation_ValidData_ReturnsOk()
        {
            // Arrange
            List<FamilyInformation> validData = new List<FamilyInformation>
            {
                // Provide valid FamilyInformation data for testing
            };

            // Act
            var result = await _controller.UpdateFamilyInformation(validData);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior
        }

        [TestMethod]
        public async Task DeleteFamilyInformation_ValidId_ReturnsOk()
        {
            // Arrange
            int validId = 1; // Provide a valid FamilyId for testing

            // Act
            var result = await _controller.DeleteFamilyInformation(validId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior
        }
        [TestMethod]
        public async Task GetFamilyInformation_InvalidAccountId_ReturnsNotFound()
        {
            // Arrange
            int invalidAccountId = -1; // Provide an invalid account ID for testing

            // Act
            var result = await _controller.GetFamilyInformation(invalidAccountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            // You can add more assertions based on the expected behavior for invalid input
        }

        [TestMethod]
        public async Task AddFamilyInformation_NullData_ReturnsBadRequest()
        {
            // Arrange
            List<FamilyInformation> nullData = null;
            int accountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.AddFamilyInformation(nullData, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            // You can add more assertions based on the expected behavior for null input
        }

        [TestMethod]
        public async Task UpdateFamilyInformation_NullData_ReturnsBadRequest()
        {
            // Arrange
            List<FamilyInformation> nullData = null;

            // Act
            var result = await _controller.UpdateFamilyInformation(nullData);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            // You can add more assertions based on the expected behavior for null input
        }

        [TestMethod]
        public async Task DeleteFamilyInformation_InvalidId_ReturnsNotFound()
        {
            // Arrange
            int invalidId = -1; // Provide an invalid FamilyId for testing

            // Act
            var result = await _controller.DeleteFamilyInformation(invalidId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

            // Add similar test methods for other actions (AddFamilyInformation, UpdateFamilyInformation, DeleteFamilyInformation) based on your specific requirements.
        }
    }
}