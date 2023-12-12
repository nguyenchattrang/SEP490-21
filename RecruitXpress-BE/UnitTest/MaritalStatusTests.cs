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
        private Mock<RecruitXpressContext> _mockContext;
        private IMapper _mapper;
        private MaritalStatusController _controller;

        [TestInitialize]
        public void Setup()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var options = new DbContextOptionsBuilder<RecruitXpressContext>()
           .UseSqlServer(config.GetConnectionString("RecruitXpress"))
           .Options;

            // Create a mock context instead of using the actual context
            _mockContext = new Mock<RecruitXpressContext>(options);

            // Configure AutoMapper with your mapping profile
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            // Create an instance of the mapper
            _mapper = mappingConfig.CreateMapper();

            // Create an instance of your controller with the mock context and mapper
            _controller = new MaritalStatusController(_mockContext.Object, _mapper);

        }

        [TestMethod]
        public async Task GetMaritalStatus_ReturnsBadRequest_WhenAccountIdIsNull()
        {
            // Arrange
            var accountId = 0; // Change this value based on your test scenario

            // Act
            var result = await _controller.GetMaritalStatus(accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetMaritalStatus_ReturnsBadRequest_WhenProfileNotFound()
        {
            // Arrange
            var accountId = 99; // Change this value based on your test scenario
           
            // Act
            var result = await _controller.GetMaritalStatus(accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetMaritalStatus_ReturnsNotFound_WhenMaritalStatusNotFound()
        {
            // Arrange
            var accountId = 13; // Change this value based on your test scenario
            // Act
            var result = await _controller.GetMaritalStatus(accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

            var notFoundResult = result as BadRequestObjectResult;
            Assert.AreEqual("Không có dữ liệu", notFoundResult.Value);

        }

        [TestMethod]
        public async Task AddMaritalStatus_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var accountId = 13; // Change this value based on your test scenario
            var maritalStatus = new MaritalStatus { Description =  "zai"};
            // Act
            var result = await _controller.AddMaritalStatus(maritalStatus, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        // Add similar test methods for other actions (AddMaritalStatus, UpdateMaritalStatus, DeleteMaritalStatus) based on your specific requirements.
    }
}