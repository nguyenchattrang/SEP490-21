using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RecruitXpress_BE.Models
{
    public partial class RecruitXpressContext : DbContext
    {
        public RecruitXpressContext()
        {
        }

        public RecruitXpressContext(DbContextOptions<RecruitXpressContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<ComputerProficiency> ComputerProficiencies { get; set; } = null!;
        public virtual DbSet<Cvtemplate> Cvtemplates { get; set; } = null!;
        public virtual DbSet<EducationalBackground> EducationalBackgrounds { get; set; } = null!;
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; } = null!;
        public virtual DbSet<EmailToken> EmailTokens { get; set; } = null!;
        public virtual DbSet<Evaluate> Evaluates { get; set; } = null!;
        public virtual DbSet<Exam> Exams { get; set; } = null!;
        public virtual DbSet<FamilyInformation> FamilyInformations { get; set; } = null!;
        public virtual DbSet<GeneralTest> GeneralTests { get; set; } = null!;
        public virtual DbSet<GeneralTestDetail> GeneralTestDetails { get; set; } = null!;
        public virtual DbSet<Interviewer> Interviewers { get; set; } = null!;
        public virtual DbSet<JobApplication> JobApplications { get; set; } = null!;
        public virtual DbSet<JobPosting> JobPostings { get; set; } = null!;
        public virtual DbSet<LanguageProficiency> LanguageProficiencies { get; set; } = null!;
        public virtual DbSet<MaritalStatus> MaritalStatuses { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Option> Options { get; set; } = null!;
        public virtual DbSet<Profile> Profiles { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Schedule> Schedules { get; set; } = null!;
        public virtual DbSet<ScheduleDetail> ScheduleDetails { get; set; } = null!;
        public virtual DbSet<SpecializedExam> SpecializedExams { get; set; } = null!;
        public virtual DbSet<UserAnalytic> UserAnalytics { get; set; } = null!;
        public virtual DbSet<WishList> WishLists { get; set; } = null!;
        public virtual DbSet<WorkExperience> WorkExperiences { get; set; } = null!;
        public virtual DbSet<training> training { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =DATVT\\DATVT25; database = RecruitXpress;uid=sa;pwd=123456; TrustServerCertificate=True ");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Account1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Account");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Token)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Account__RoleID__267ABA7A");
            });

            modelBuilder.Entity<ComputerProficiency>(entity =>
            {
                entity.ToTable("ComputerProficiency");

                entity.Property(e => e.ComputerProficiencyId).HasColumnName("ComputerProficiencyID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.ProfileId).HasColumnName("ProfileID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ComputerProficiencies)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("FK__ComputerP__Profi__46E78A0C");
            });

            modelBuilder.Entity<Cvtemplate>(entity =>
            {
                entity.HasKey(e => e.TemplateId)
                    .HasName("PK__CVTempla__F87ADD0766EBA548");

                entity.ToTable("CVTemplate");

                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("URL");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Cvtemplates)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__CVTemplat__Accou__3A81B327");
            });

            modelBuilder.Entity<EducationalBackground>(entity =>
            {
                entity.ToTable("EducationalBackground");

                entity.Property(e => e.EducationalBackgroundId).HasColumnName("EducationalBackgroundID");

                entity.Property(e => e.Awards).HasColumnType("text");

                entity.Property(e => e.Certifications).HasColumnType("text");

                entity.Property(e => e.Gpa).HasColumnName("GPA");

                entity.Property(e => e.ProfileId).HasColumnName("ProfileID");

                entity.Property(e => e.ResearchProjects).HasColumnType("text");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.EducationalBackgrounds)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("FK__Education__Profi__2F10007B");
            });

            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.HasKey(e => e.TemplateId)
                    .HasName("PK__EmailTem__F87ADD07A062333C");

                entity.ToTable("EmailTemplate");

                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

                entity.Property(e => e.SendTo).IsUnicode(false);
            });

            modelBuilder.Entity<EmailToken>(entity =>
            {
                entity.HasKey(e => e.TokenId)
                    .HasName("PK__EmailTok__658FEEEA79A81FCB");

                entity.ToTable("EmailToken");

                entity.Property(e => e.ExpiredAt).HasColumnType("datetime");

                entity.Property(e => e.IssuedAt).HasColumnType("datetime");

                entity.Property(e => e.Token).HasMaxLength(255);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.EmailTokens)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__EmailToke__Accou__6FE99F9F");
            });

            modelBuilder.Entity<Evaluate>(entity =>
            {
                entity.ToTable("Evaluate");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.EvaluaterEmailContact).HasMaxLength(255);

                entity.Property(e => e.EvaluaterPhoneContact).HasMaxLength(50);

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.Evaluates)
                    .HasForeignKey(d => d.ProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Evaluate__Profil__02084FDA");
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.ToTable("Exam");

                entity.Property(e => e.ExamId).HasColumnName("ExamID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Comment).HasColumnType("ntext");

                entity.Property(e => e.FileUrl)
                    .HasColumnType("ntext")
                    .HasColumnName("FileURL");

                entity.Property(e => e.MarkedBy).HasColumnType("ntext");

                entity.Property(e => e.MarkedDate).HasColumnType("date");

                entity.Property(e => e.Point)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TestDate).HasColumnType("date");

                entity.Property(e => e.TestTime).HasColumnType("date");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Exam__AccountID__6754599E");
            });

            modelBuilder.Entity<FamilyInformation>(entity =>
            {
                entity.HasKey(e => e.FamilyId)
                    .HasName("PK__FamilyIn__41D82F4B3F932CC3");

                entity.ToTable("FamilyInformation");

                entity.Property(e => e.FamilyId).HasColumnName("FamilyID");

                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.Birthdays).HasColumnType("date");

                entity.Property(e => e.ContactNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EducationLevel).IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FamilyName).IsUnicode(false);

                entity.Property(e => e.ProfileId).HasColumnName("ProfileID");

                entity.Property(e => e.RelationshipStatus).IsUnicode(false);

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.FamilyInformations)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("FK__FamilyInf__Profi__5FB337D6");
            });

            modelBuilder.Entity<GeneralTest>(entity =>
            {
                entity.ToTable("GeneralTest");

                entity.Property(e => e.GeneralTestId).HasColumnName("GeneralTestID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.ProfileId).HasColumnName("ProfileID");
            });

            modelBuilder.Entity<GeneralTestDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId)
                    .HasName("PK__GeneralT__135C314D16244C47");

                entity.ToTable("GeneralTestDetail");

                entity.Property(e => e.DetailId).HasColumnName("DetailID");

                entity.Property(e => e.GeneralTestId).HasColumnName("GeneralTestID");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.HasOne(d => d.GeneralTest)
                    .WithMany(p => p.GeneralTestDetails)
                    .HasForeignKey(d => d.GeneralTestId)
                    .HasConstraintName("FK__GeneralTe__Gener__4BAC3F29");
            });

            modelBuilder.Entity<Interviewer>(entity =>
            {
                entity.Property(e => e.InterviewerId).HasColumnName("InterviewerID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Interviewers)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Interview__Accou__59FA5E80");

                entity.HasOne(d => d.Schedule)
                    .WithMany(p => p.Interviewers)
                    .HasForeignKey(d => d.ScheduleId)
                    .HasConstraintName("FK__Interview__Sched__5AEE82B9");
            });

            modelBuilder.Entity<JobApplication>(entity =>
            {
                entity.HasKey(e => e.ApplicationId)
                    .HasName("PK__JobAppli__C93A4F7978364438");

                entity.ToTable("JobApplication");

                entity.Property(e => e.ApplicationId).HasColumnName("ApplicationID");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.ProfileId).HasColumnName("ProfileID");

                entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobApplications)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK__JobApplic__JobID__3F466844");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.JobApplications)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("FK__JobApplic__Profi__403A8C7D");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.JobApplications)
                    .HasForeignKey(d => d.TemplateId)
                    .HasConstraintName("FK__JobApplic__Templ__412EB0B6");
            });

            modelBuilder.Entity<JobPosting>(entity =>
            {
                entity.HasKey(e => e.JobId)
                    .HasName("PK__JobPosti__056690E2634FA7CD");

                entity.ToTable("JobPosting");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.Property(e => e.ApplicationDeadline).HasColumnType("date");

                entity.Property(e => e.Company).HasMaxLength(255);

                entity.Property(e => e.ContactPerson).HasMaxLength(255);

                entity.Property(e => e.DatePosted).HasColumnType("date");
            });

            modelBuilder.Entity<LanguageProficiency>(entity =>
            {
                entity.ToTable("LanguageProficiency");

                entity.Property(e => e.LanguageProficiencyId).HasColumnName("LanguageProficiencyID");

                entity.Property(e => e.ProfileId).HasColumnName("ProfileID");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.LanguageProficiencies)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("FK__LanguageP__Profi__440B1D61");
            });

            modelBuilder.Entity<MaritalStatus>(entity =>
            {
                entity.HasKey(e => e.StatusId)
                    .HasName("PK__MaritalS__C8EE204339E40737");

                entity.ToTable("MaritalStatus");

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.NotificationId).HasColumnName("NotificationID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ReceiverId).HasColumnName("ReceiverID");

                entity.Property(e => e.SenderId).HasColumnName("SenderID");

                entity.Property(e => e.TargetUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("TargetURL");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.NotificationReceivers)
                    .HasForeignKey(d => d.ReceiverId)
                    .HasConstraintName("FK__Notificat__Recei__534D60F1");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.NotificationSenders)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK__Notificat__Sende__52593CB8");
            });

            modelBuilder.Entity<Option>(entity =>
            {
                entity.ToTable("Option");

                entity.Property(e => e.OptionId).HasColumnName("OptionID");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.ToTable("Profile");

                entity.Property(e => e.ProfileId).HasColumnName("ProfileID");

                entity.Property(e => e.Accomplishment).HasColumnType("text");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Address).HasMaxLength(100);

                entity.Property(e => e.Article).HasColumnType("text");

                entity.Property(e => e.Avatar).IsUnicode(false);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Gender).HasMaxLength(100);

                entity.Property(e => e.Imperfection).HasColumnType("text");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ResearchWork).HasColumnType("text");

                entity.Property(e => e.Skills).HasMaxLength(100);

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.Property(e => e.Strength).HasColumnType("text");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__Profile__Account__2B3F6F97");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__Profile__StatusI__2C3393D0");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("Question");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.Question1).HasColumnName("Question");

                entity.Property(e => e.Type)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("RoleID");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.RoleName).HasMaxLength(100);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");

                entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Hr)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("HR");
            });

            modelBuilder.Entity<ScheduleDetail>(entity =>
            {
                entity.Property(e => e.ScheduleDetailId).HasColumnName("ScheduleDetailID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ProfileId).HasColumnName("ProfileID");

                entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");

                entity.Property(e => e.Strengths).HasColumnType("text");

                entity.Property(e => e.Weaknesses).HasColumnType("text");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.ScheduleDetails)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("FK__ScheduleD__Profi__37A5467C");

                entity.HasOne(d => d.Schedule)
                    .WithMany(p => p.ScheduleDetails)
                    .HasForeignKey(d => d.ScheduleId)
                    .HasConstraintName("FK__ScheduleD__Sched__36B12243");
            });

            modelBuilder.Entity<SpecializedExam>(entity =>
            {
                entity.HasKey(e => e.ExamId)
                    .HasName("PK__Speciali__297521C788E8B7E3");

                entity.ToTable("SpecializedExam");

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.ExamName).HasMaxLength(255);

                entity.Property(e => e.StartDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserAnalytic>(entity =>
            {
                entity.HasKey(e => e.AnalyticId)
                    .HasName("PK__UserAnal__4C99FBC30F0AB1D3");

                entity.Property(e => e.AnalyticId).HasColumnName("AnalyticID");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Data).HasColumnType("ntext");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WishList>(entity =>
            {
                entity.ToTable("WishList");

                entity.Property(e => e.WishlistId).HasColumnName("WishlistID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.JobId).HasColumnName("JobID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.WishLists)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK__WishList__Accoun__5629CD9C");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.WishLists)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK__WishList__JobID__571DF1D5");
            });

            modelBuilder.Entity<WorkExperience>(entity =>
            {
                entity.ToTable("WorkExperience");

                entity.Property(e => e.WorkExperienceId).HasColumnName("WorkExperienceID");

                entity.Property(e => e.Achievements).HasColumnType("text");

                entity.Property(e => e.Company).HasMaxLength(256);

                entity.Property(e => e.EmploymentType).HasMaxLength(256);

                entity.Property(e => e.JobTitle).HasMaxLength(256);

                entity.Property(e => e.Location).HasMaxLength(256);

                entity.Property(e => e.ProfileId).HasColumnName("ProfileID");

                entity.Property(e => e.Responsibilities).HasColumnType("text");

                entity.Property(e => e.SkillsUsed).HasMaxLength(256);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.WorkExperiences)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("FK__WorkExper__Profi__31EC6D26");
            });

            modelBuilder.Entity<training>(entity =>
            {
                entity.ToTable("Training");

                entity.Property(e => e.TrainingId).HasColumnName("TrainingID");

                entity.Property(e => e.CertificationOffered).IsUnicode(false);

                entity.Property(e => e.Duration).IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.FormatName).IsUnicode(false);

                entity.Property(e => e.Language).IsUnicode(false);

                entity.Property(e => e.Location).IsUnicode(false);

                entity.Property(e => e.ProfileId).HasColumnName("ProfileID");

                entity.Property(e => e.SkillsCovered).IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Profile)
                    .WithMany(p => p.training)
                    .HasForeignKey(d => d.ProfileId)
                    .HasConstraintName("FK__Training__Profil__628FA481");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
