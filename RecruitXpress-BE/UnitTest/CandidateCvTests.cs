using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    public class CandidateCvControllerTests
    {
        private readonly IConfiguration _configuration;
        private CVController _controller;


        [TestInitialize]
        public void Setup()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var options = new DbContextOptionsBuilder<RecruitXpressContext>()
     .UseSqlServer(config.GetConnectionString("RecruitXpress")) // Use the appropriate provider (e.g., UseSqlServer for SQL Server)
     .Options;

            // Create an instance of the context using the actual database
            var dbContext = new RecruitXpressContext(options);

            // Create an instance of your controller with the mock context and mapper
            _controller = new CVController(dbContext);

        }
        [TestMethod]
        public async Task AddCV_ValidData_ReturnsOkResult()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.pdf");
            fileMock.Setup(f => f.Length).Returns(1024); // Set an appropriate file length

            var accountId = 1;

            // Act
            var result = await _controller.addCV(fileMock.Object, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task ViewCV_ValidAccountId_ReturnsOkResult()
        {
            // Arrange
            var accountId = 1;

            // Act
            var result = await _controller.ViewCV(accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        // ... (previous code)

        [TestMethod]
        public async Task AddCV_InvalidFile_ReturnsBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            // Do not setup file properties to simulate an invalid file

            var accountId = 1;

            // Act
            var result = await _controller.addCV(fileMock.Object, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task ViewCV_InvalidAccountId_ReturnsNotFound()
        {
            // Arrange
            var invalidAccountId = -1;

            // Act
            var result = await _controller.ViewCV(invalidAccountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }


        [TestMethod]
        public async Task DeleteCV_InvalidCVId_ReturnsNotFound()
        {
            // Arrange
            var invalidCVId = -1;

            // Act
            var result = await _controller.DeleteCV(invalidCVId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

    

        // Add similar test methods for other actions (AddMaritalStatus, UpdateMaritalStatus, DeleteMaritalStatus) based on your specific requirements.
    }
}