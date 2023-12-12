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
    public class ConfigUnitTest
    {

        protected IMapper Mapper;
        protected DbContextOptions<RecruitXpressContext> DbContextOptions;

        [TestInitialize]
        public void BaseSetup()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            DbContextOptions = new DbContextOptionsBuilder<RecruitXpressContext>()
                .UseSqlServer(config.GetConnectionString("RecruitXpress"))
                .Options;

            var dbContext = new RecruitXpressContext(DbContextOptions);

            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            Mapper = mappingConfig.CreateMapper();
        }

    }
      
}