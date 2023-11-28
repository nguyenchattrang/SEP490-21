using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitXpress_BE.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpirationTimestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExamCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplate",
                columns: table => new
                {
                    TemplateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Header = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendTo = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    MailType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EmailTem__F87ADD070824A102", x => x.TemplateID);
                });

            migrationBuilder.CreateTable(
                name: "JobPosting",
                columns: table => new
                {
                    JobID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmploymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinSalary = table.Column<long>(type: "bigint", nullable: true),
                    MaxSalary = table.Column<long>(type: "bigint", nullable: true),
                    ApplicationDeadline = table.Column<DateTime>(type: "date", nullable: true),
                    DatePosted = table.Column<DateTime>(type: "date", nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ApplicationInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__JobPosti__056690E27DD35E85", x => x.JobID);
                });

            migrationBuilder.CreateTable(
                name: "MaritalStatus",
                columns: table => new
                {
                    StatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MaritalS__C8EE20431005001E", x => x.StatusID);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "UserAnalytics",
                columns: table => new
                {
                    AnalyticID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Data = table.Column<string>(type: "ntext", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserAnal__4C99FBC383CF1626", x => x.AnalyticID);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    RoleID = table.Column<int>(type: "int", nullable: true),
                    Token = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountID);
                    table.ForeignKey(
                        name: "FK__Account__RoleID__38996AB5",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID");
                });

            migrationBuilder.CreateTable(
                name: "CandidateCV",
                columns: table => new
                {
                    TemplateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: true),
                    URL = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CVTempla__F87ADD070A716C45", x => x.TemplateID);
                    table.ForeignKey(
                        name: "FK__CVTemplat__Accou__4AB81AF0",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateTable(
                name: "CVTemplate",
                columns: table => new
                {
                    CVTemplateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Thumbnail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CVTemplate", x => x.CVTemplateID);
                    table.ForeignKey(
                        name: "FK_CVTemplate_Account",
                        column: x => x.CreatedBy,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateTable(
                name: "EmailToken",
                columns: table => new
                {
                    TokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    IssuedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    ExpiredAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsUsed = table.Column<bool>(type: "bit", nullable: true),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EmailTok__658FEEEA6548AD56", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK__EmailToke__Accou__681373AD",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seen = table.Column<bool>(type: "bit", nullable: true),
                    SenderID = table.Column<int>(type: "int", nullable: true),
                    ReceiverID = table.Column<int>(type: "int", nullable: true),
                    TargetURL = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK__Notificat__Recei__60A75C0F",
                        column: x => x.ReceiverID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK__Notificat__Sende__5FB337D6",
                        column: x => x.SenderID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    ProfileID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: true),
                    StatusID = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Avatar = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Skills = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Accomplishment = table.Column<string>(type: "text", nullable: true),
                    Strength = table.Column<string>(type: "text", nullable: true),
                    Imperfection = table.Column<string>(type: "text", nullable: true),
                    ResearchWork = table.Column<string>(type: "text", nullable: true),
                    Article = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.ProfileID);
                    table.ForeignKey(
                        name: "FK__Profile__Account__7B5B524B",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK__Profile__StatusI__7C4F7684",
                        column: x => x.StatusID,
                        principalTable: "MaritalStatus",
                        principalColumn: "StatusID");
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    QuestionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.QuestionID);
                    table.ForeignKey(
                        name: "FK_Question_Account",
                        column: x => x.CreatedBy,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateTable(
                name: "SpecializedExam",
                columns: table => new
                {
                    ExamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExpertEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Speciali__297521C76A3469C5", x => x.ExamId);
                    table.ForeignKey(
                        name: "FK_SpecializedExam_Account",
                        column: x => x.CreatedBy,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateTable(
                name: "WishList",
                columns: table => new
                {
                    WishlistID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: true),
                    JobID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishList", x => x.WishlistID);
                    table.ForeignKey(
                        name: "FK__WishList__Accoun__6383C8BA",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK__WishList__JobID__6477ECF3",
                        column: x => x.JobID,
                        principalTable: "JobPosting",
                        principalColumn: "JobID");
                });

            migrationBuilder.CreateTable(
                name: "ComputerProficiency",
                columns: table => new
                {
                    ComputerProficiencyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileID = table.Column<int>(type: "int", nullable: true),
                    TechnicalSkills = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SkillLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExperienceYears = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComputerProficiency", x => x.ComputerProficiencyID);
                    table.ForeignKey(
                        name: "FK__ComputerP__Profi__1332DBDC",
                        column: x => x.ProfileID,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "EducationalBackground",
                columns: table => new
                {
                    EducationalBackgroundID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileID = table.Column<int>(type: "int", nullable: true),
                    InstitutionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DegreeEarned = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GPA = table.Column<double>(type: "float", nullable: true),
                    EducationalLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Certifications = table.Column<string>(type: "text", nullable: true),
                    ResearchProjects = table.Column<string>(type: "text", nullable: true),
                    Awards = table.Column<string>(type: "text", nullable: true),
                    Time = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationalBackground", x => x.EducationalBackgroundID);
                    table.ForeignKey(
                        name: "FK__Education__Profi__7F2BE32F",
                        column: x => x.ProfileID,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "Evaluate",
                columns: table => new
                {
                    EvaluateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mark = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    EvaluatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvaluaterPhoneContact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EvaluaterEmailContact = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EvaluaterAccountId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluate", x => x.EvaluateId);
                    table.ForeignKey(
                        name: "FK__Evaluate__Profil__41B8C09B",
                        column: x => x.ProfileId,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "FamilyInformation",
                columns: table => new
                {
                    FamilyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileID = table.Column<int>(type: "int", nullable: true),
                    FamilyName = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    RelationshipStatus = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    EducationLevel = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Address = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    ContactNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Birthdays = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FamilyIn__41D82F4B191BA3E1", x => x.FamilyID);
                    table.ForeignKey(
                        name: "FK__FamilyInf__Profi__1DB06A4F",
                        column: x => x.ProfileID,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "GeneralTest",
                columns: table => new
                {
                    GeneralTestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileID = table.Column<int>(type: "int", nullable: true),
                    TestName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralTest", x => x.GeneralTestID);
                    table.ForeignKey(
                        name: "FK_GeneralTest_Account",
                        column: x => x.CreatedBy,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK_GeneralTest_Profile",
                        column: x => x.ProfileID,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "JobApplication",
                columns: table => new
                {
                    ApplicationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobID = table.Column<int>(type: "int", nullable: true),
                    ProfileID = table.Column<int>(type: "int", nullable: true),
                    TemplateID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    AssignedFor = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__JobAppli__C93A4F79B26EEE6F", x => x.ApplicationID);
                    table.ForeignKey(
                        name: "FK__JobApplic__JobID__0B91BA14",
                        column: x => x.JobID,
                        principalTable: "JobPosting",
                        principalColumn: "JobID");
                    table.ForeignKey(
                        name: "FK__JobApplic__Profi__0C85DE4D",
                        column: x => x.ProfileID,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                    table.ForeignKey(
                        name: "FK__JobApplic__Templ__0D7A0286",
                        column: x => x.TemplateID,
                        principalTable: "CandidateCV",
                        principalColumn: "TemplateID");
                });

            migrationBuilder.CreateTable(
                name: "LanguageProficiency",
                columns: table => new
                {
                    LanguageProficiencyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileID = table.Column<int>(type: "int", nullable: true),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProficiencyLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Certifications = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LanguageExperiences = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestScores = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageProficiency", x => x.LanguageProficiencyID);
                    table.ForeignKey(
                        name: "FK__LanguageP__Profi__10566F31",
                        column: x => x.ProfileID,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "ReferenceChecking",
                columns: table => new
                {
                    ReferenceCheckingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(type: "int", nullable: true),
                    NameOf = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceChecking", x => x.ReferenceCheckingId);
                    table.ForeignKey(
                        name: "FK__Reference__Profi__37FA4C37",
                        column: x => x.ProfileId,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HumanResourceId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_Schedule_Profile",
                        column: x => x.HumanResourceId,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "ShortListing",
                columns: table => new
                {
                    ListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(type: "int", nullable: true),
                    JobId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ShortLis__E3832805B6BF7C24", x => x.ListId);
                    table.ForeignKey(
                        name: "FK__ShortList__JobId__351DDF8C",
                        column: x => x.JobId,
                        principalTable: "JobPosting",
                        principalColumn: "JobID");
                    table.ForeignKey(
                        name: "FK__ShortList__Profi__3429BB53",
                        column: x => x.ProfileId,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "Training",
                columns: table => new
                {
                    TrainingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileID = table.Column<int>(type: "int", nullable: true),
                    FormatName = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Duration = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Location = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    Language = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    SkillsCovered = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CertificationOffered = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training", x => x.TrainingID);
                    table.ForeignKey(
                        name: "FK__Training__Profil__208CD6FA",
                        column: x => x.ProfileID,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "WorkExperience",
                columns: table => new
                {
                    WorkExperienceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileID = table.Column<int>(type: "int", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Company = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    Responsibilities = table.Column<string>(type: "text", nullable: true),
                    Achievements = table.Column<string>(type: "text", nullable: true),
                    SkillsUsed = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmploymentType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkExperience", x => x.WorkExperienceID);
                    table.ForeignKey(
                        name: "FK__WorkExper__Profi__02084FDA",
                        column: x => x.ProfileID,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                });

            migrationBuilder.CreateTable(
                name: "Option",
                columns: table => new
                {
                    OptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionID = table.Column<int>(type: "int", nullable: true),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Option", x => x.OptionID);
                    table.ForeignKey(
                        name: "FK_Option_Question",
                        column: x => x.QuestionID,
                        principalTable: "Question",
                        principalColumn: "QuestionID");
                });

            migrationBuilder.CreateTable(
                name: "Exam",
                columns: table => new
                {
                    ExamID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: true),
                    FileURL = table.Column<string>(type: "ntext", nullable: true),
                    TestDate = table.Column<DateTime>(type: "date", nullable: true),
                    TestTime = table.Column<DateTime>(type: "date", nullable: true),
                    Point = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Comment = table.Column<string>(type: "ntext", nullable: true),
                    MarkedBy = table.Column<string>(type: "ntext", nullable: true),
                    MarkedDate = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    SpecializedExamId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam", x => x.ExamID);
                    table.ForeignKey(
                        name: "FK__Exam__AccountID__76969D2E",
                        column: x => x.AccountID,
                        principalTable: "Account",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK_Exam_SpecializedExam",
                        column: x => x.SpecializedExamId,
                        principalTable: "SpecializedExam",
                        principalColumn: "ExamId");
                });

            migrationBuilder.CreateTable(
                name: "Interviewer",
                columns: table => new
                {
                    InterviewerId = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviewer", x => new { x.InterviewerId, x.ScheduleId });
                    table.ForeignKey(
                        name: "FK_Interviewer_Profile",
                        column: x => x.InterviewerId,
                        principalTable: "Profile",
                        principalColumn: "ProfileID");
                    table.ForeignKey(
                        name: "FK_Interviewer_Schedule",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "ScheduleId");
                });

            migrationBuilder.CreateTable(
                name: "ScheduleDetail",
                columns: table => new
                {
                    ScheduleDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleId = table.Column<int>(type: "int", nullable: true),
                    CandidateId = table.Column<int>(type: "int", nullable: true),
                    ScheduleType = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdatedTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleDetail", x => x.ScheduleDetailId);
                    table.ForeignKey(
                        name: "FK_ScheduleDetail_JobApplication",
                        column: x => x.CandidateId,
                        principalTable: "JobApplication",
                        principalColumn: "ApplicationID");
                    table.ForeignKey(
                        name: "FK_ScheduleDetail_Schedule",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "ScheduleId");
                });

            migrationBuilder.CreateTable(
                name: "GeneralTestDetail",
                columns: table => new
                {
                    DetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GeneralTestID = table.Column<int>(type: "int", nullable: true),
                    QuestionID = table.Column<int>(type: "int", nullable: true),
                    Answer = table.Column<int>(type: "int", nullable: true),
                    Point = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__GeneralT__135C314D621B7001", x => x.DetailID);
                    table.ForeignKey(
                        name: "FK__GeneralTe__Gener__5AEE82B9",
                        column: x => x.GeneralTestID,
                        principalTable: "GeneralTest",
                        principalColumn: "GeneralTestID");
                    table.ForeignKey(
                        name: "FK_GeneralTestDetail_Option",
                        column: x => x.Answer,
                        principalTable: "Option",
                        principalColumn: "OptionID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_RoleID",
                table: "Account",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateCV_AccountID",
                table: "CandidateCV",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_ComputerProficiency_ProfileID",
                table: "ComputerProficiency",
                column: "ProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_CVTemplate_CreatedBy",
                table: "CVTemplate",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EducationalBackground_ProfileID",
                table: "EducationalBackground",
                column: "ProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_EmailToken_AccountId",
                table: "EmailToken",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluate_ProfileId",
                table: "Evaluate",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_AccountID",
                table: "Exam",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_SpecializedExamId",
                table: "Exam",
                column: "SpecializedExamId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyInformation_ProfileID",
                table: "FamilyInformation",
                column: "ProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTest_CreatedBy",
                table: "GeneralTest",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTest_ProfileID",
                table: "GeneralTest",
                column: "ProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTestDetail_Answer",
                table: "GeneralTestDetail",
                column: "Answer");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralTestDetail_GeneralTestID",
                table: "GeneralTestDetail",
                column: "GeneralTestID");

            migrationBuilder.CreateIndex(
                name: "IX_Interviewer_ScheduleId",
                table: "Interviewer",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_JobID",
                table: "JobApplication",
                column: "JobID");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_ProfileID",
                table: "JobApplication",
                column: "ProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplication_TemplateID",
                table: "JobApplication",
                column: "TemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_LanguageProficiency_ProfileID",
                table: "LanguageProficiency",
                column: "ProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ReceiverID",
                table: "Notification",
                column: "ReceiverID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_SenderID",
                table: "Notification",
                column: "SenderID");

            migrationBuilder.CreateIndex(
                name: "IX_Option_QuestionID",
                table: "Option",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_AccountID",
                table: "Profile",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_StatusID",
                table: "Profile",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Question_CreatedBy",
                table: "Question",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceChecking_ProfileId",
                table: "ReferenceChecking",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_HumanResourceId",
                table: "Schedule",
                column: "HumanResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDetail_CandidateId",
                table: "ScheduleDetail",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDetail_ScheduleId",
                table: "ScheduleDetail",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShortListing_JobId",
                table: "ShortListing",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_ShortListing_ProfileId",
                table: "ShortListing",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializedExam_CreatedBy",
                table: "SpecializedExam",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ProfileID",
                table: "Training",
                column: "ProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_WishList_AccountID",
                table: "WishList",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_WishList_JobID",
                table: "WishList",
                column: "JobID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperience_ProfileID",
                table: "WorkExperience",
                column: "ProfileID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessCodes");

            migrationBuilder.DropTable(
                name: "ComputerProficiency");

            migrationBuilder.DropTable(
                name: "CVTemplate");

            migrationBuilder.DropTable(
                name: "EducationalBackground");

            migrationBuilder.DropTable(
                name: "EmailTemplate");

            migrationBuilder.DropTable(
                name: "EmailToken");

            migrationBuilder.DropTable(
                name: "Evaluate");

            migrationBuilder.DropTable(
                name: "Exam");

            migrationBuilder.DropTable(
                name: "FamilyInformation");

            migrationBuilder.DropTable(
                name: "GeneralTestDetail");

            migrationBuilder.DropTable(
                name: "Interviewer");

            migrationBuilder.DropTable(
                name: "LanguageProficiency");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "ReferenceChecking");

            migrationBuilder.DropTable(
                name: "ScheduleDetail");

            migrationBuilder.DropTable(
                name: "ShortListing");

            migrationBuilder.DropTable(
                name: "Training");

            migrationBuilder.DropTable(
                name: "UserAnalytics");

            migrationBuilder.DropTable(
                name: "WishList");

            migrationBuilder.DropTable(
                name: "WorkExperience");

            migrationBuilder.DropTable(
                name: "SpecializedExam");

            migrationBuilder.DropTable(
                name: "GeneralTest");

            migrationBuilder.DropTable(
                name: "Option");

            migrationBuilder.DropTable(
                name: "JobApplication");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "JobPosting");

            migrationBuilder.DropTable(
                name: "CandidateCV");

            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "MaritalStatus");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
