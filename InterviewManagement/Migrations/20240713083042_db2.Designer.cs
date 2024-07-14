﻿// <auto-generated />
using System;
using InterviewManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InterviewManagement.Migrations
{
    [DbContext(typeof(InterviewManagementContext))]
    [Migration("20240713083042_db2")]
    partial class db2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.29")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BenefitJob", b =>
                {
                    b.Property<long>("BenefitsId")
                        .HasColumnType("bigint");

                    b.Property<int>("JobsId")
                        .HasColumnType("int");

                    b.HasKey("BenefitsId", "JobsId");

                    b.HasIndex("JobsId");

                    b.ToTable("BenefitJob");
                });

            modelBuilder.Entity("CandidateSkill", b =>
                {
                    b.Property<long>("CandidatesId")
                        .HasColumnType("bigint");

                    b.Property<int>("SkillsId")
                        .HasColumnType("int");

                    b.HasKey("CandidatesId", "SkillsId");

                    b.HasIndex("SkillsId");

                    b.ToTable("CandidateSkill");
                });

            modelBuilder.Entity("EmployeeOffer", b =>
                {
                    b.Property<long>("EmployeesId")
                        .HasColumnType("bigint");

                    b.Property<int>("OffersId")
                        .HasColumnType("int");

                    b.HasKey("EmployeesId", "OffersId");

                    b.HasIndex("OffersId");

                    b.ToTable("EmployeeOffer");
                });

            modelBuilder.Entity("EmployeeSchedule", b =>
                {
                    b.Property<long>("EmployeesId")
                        .HasColumnType("bigint");

                    b.Property<long>("SchedulesId")
                        .HasColumnType("bigint");

                    b.HasKey("EmployeesId", "SchedulesId");

                    b.HasIndex("SchedulesId");

                    b.ToTable("EmployeeSchedule");
                });

            modelBuilder.Entity("InterviewManagement.Models.Benefit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("BenefitName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Benefit");
                });

            modelBuilder.Entity("InterviewManagement.Models.Candidate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedOn")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("CvLink")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("email");

                    b.Property<long?>("EmployeeId")
                        .HasColumnType("bigint");

                    b.Property<int>("ExpYear")
                        .HasColumnType("int");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("HighestLevelId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("LevelId")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.Property<int?>("PositionId")
                        .HasColumnType("int");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("dob")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("HighestLevelId");

                    b.HasIndex("LevelId");

                    b.HasIndex("PositionId");

                    b.HasIndex("RoleId");

                    b.ToTable("Candidate", (string)null);
                });

            modelBuilder.Entity("InterviewManagement.Models.Contract", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("ContractName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Contract");
                });

            modelBuilder.Entity("InterviewManagement.Models.Department", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("DepertmentName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("InterviewManagement.Models.Employee", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("DepartmentId")
                        .HasColumnType("bigint");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("email");

                    b.Property<DateTime?>("ExpiredDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.Property<string>("ResetPasswordToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("dob")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("RoleId");

                    b.ToTable("Employee", (string)null);
                });

            modelBuilder.Entity("InterviewManagement.Models.HighestLevel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("HighestLevel");
                });

            modelBuilder.Entity("InterviewManagement.Models.Job", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndDate")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("SalaryMax")
                        .HasColumnType("float");

                    b.Property<double?>("SalaryMin")
                        .IsRequired()
                        .HasColumnType("float");

                    b.Property<DateTime?>("StartDate")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WorkingAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Job");
                });

            modelBuilder.Entity("InterviewManagement.Models.Level", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("LevelName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Level");
                });

            modelBuilder.Entity("InterviewManagement.Models.Offer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<long?>("CandidateId")
                        .IsRequired()
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("ContractFrom")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<long?>("ContractId")
                        .IsRequired()
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("ContractTo")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<long?>("DepartmentId")
                        .IsRequired()
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("DueDate")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("LevelId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PositionId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<double?>("Salary")
                        .IsRequired()
                        .HasColumnType("float");

                    b.Property<long?>("ScheduleId")
                        .IsRequired()
                        .HasColumnType("bigint");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CandidateId");

                    b.HasIndex("ContractId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("LevelId");

                    b.HasIndex("PositionId");

                    b.HasIndex("ScheduleId");

                    b.ToTable("Offer");
                });

            modelBuilder.Entity("InterviewManagement.Models.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("PositionName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Position");
                });

            modelBuilder.Entity("InterviewManagement.Models.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("InterviewManagement.Models.Schedule", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<long?>("CandidateId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("JobId")
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MeetingURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Result")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ScheduleName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("ScheduleTime")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CandidateId");

                    b.HasIndex("JobId");

                    b.ToTable("Schedule");
                });

            modelBuilder.Entity("InterviewManagement.Models.Skill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("SkillName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Skill");
                });

            modelBuilder.Entity("JobLevel", b =>
                {
                    b.Property<int>("JobsId")
                        .HasColumnType("int");

                    b.Property<int>("LevelsId")
                        .HasColumnType("int");

                    b.HasKey("JobsId", "LevelsId");

                    b.HasIndex("LevelsId");

                    b.ToTable("JobLevel");
                });

            modelBuilder.Entity("JobSkill", b =>
                {
                    b.Property<int>("JobsId")
                        .HasColumnType("int");

                    b.Property<int>("SkillsId")
                        .HasColumnType("int");

                    b.HasKey("JobsId", "SkillsId");

                    b.HasIndex("SkillsId");

                    b.ToTable("JobSkill");
                });

            modelBuilder.Entity("BenefitJob", b =>
                {
                    b.HasOne("InterviewManagement.Models.Benefit", null)
                        .WithMany()
                        .HasForeignKey("BenefitsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterviewManagement.Models.Job", null)
                        .WithMany()
                        .HasForeignKey("JobsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CandidateSkill", b =>
                {
                    b.HasOne("InterviewManagement.Models.Candidate", null)
                        .WithMany()
                        .HasForeignKey("CandidatesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterviewManagement.Models.Skill", null)
                        .WithMany()
                        .HasForeignKey("SkillsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EmployeeOffer", b =>
                {
                    b.HasOne("InterviewManagement.Models.Employee", null)
                        .WithMany()
                        .HasForeignKey("EmployeesId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("InterviewManagement.Models.Offer", null)
                        .WithMany()
                        .HasForeignKey("OffersId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("EmployeeSchedule", b =>
                {
                    b.HasOne("InterviewManagement.Models.Employee", null)
                        .WithMany()
                        .HasForeignKey("EmployeesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterviewManagement.Models.Schedule", null)
                        .WithMany()
                        .HasForeignKey("SchedulesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("InterviewManagement.Models.Candidate", b =>
                {
                    b.HasOne("InterviewManagement.Models.Employee", "Employee")
                        .WithMany("Candidates")
                        .HasForeignKey("EmployeeId");

                    b.HasOne("InterviewManagement.Models.HighestLevel", "HighestLevel")
                        .WithMany()
                        .HasForeignKey("HighestLevelId");

                    b.HasOne("InterviewManagement.Models.Level", "Level")
                        .WithMany("Candidates")
                        .HasForeignKey("LevelId");

                    b.HasOne("InterviewManagement.Models.Position", "Position")
                        .WithMany("Candidates")
                        .HasForeignKey("PositionId");

                    b.HasOne("InterviewManagement.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("HighestLevel");

                    b.Navigation("Level");

                    b.Navigation("Position");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("InterviewManagement.Models.Employee", b =>
                {
                    b.HasOne("InterviewManagement.Models.Department", "Department")
                        .WithMany("Employees")
                        .HasForeignKey("DepartmentId");

                    b.HasOne("InterviewManagement.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("InterviewManagement.Models.Offer", b =>
                {
                    b.HasOne("InterviewManagement.Models.Candidate", "Candidate")
                        .WithMany("Offers")
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterviewManagement.Models.Contract", "Contract")
                        .WithMany("Offers")
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterviewManagement.Models.Department", "Department")
                        .WithMany("Offers")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterviewManagement.Models.Level", "Level")
                        .WithMany("Offers")
                        .HasForeignKey("LevelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterviewManagement.Models.Position", "Position")
                        .WithMany("Offers")
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterviewManagement.Models.Schedule", "Schedule")
                        .WithMany()
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Candidate");

                    b.Navigation("Contract");

                    b.Navigation("Department");

                    b.Navigation("Level");

                    b.Navigation("Position");

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("InterviewManagement.Models.Schedule", b =>
                {
                    b.HasOne("InterviewManagement.Models.Candidate", "Candidate")
                        .WithMany("Schedules")
                        .HasForeignKey("CandidateId");

                    b.HasOne("InterviewManagement.Models.Job", "Job")
                        .WithMany("Schedules")
                        .HasForeignKey("JobId");

                    b.Navigation("Candidate");

                    b.Navigation("Job");
                });

            modelBuilder.Entity("JobLevel", b =>
                {
                    b.HasOne("InterviewManagement.Models.Job", null)
                        .WithMany()
                        .HasForeignKey("JobsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterviewManagement.Models.Level", null)
                        .WithMany()
                        .HasForeignKey("LevelsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("JobSkill", b =>
                {
                    b.HasOne("InterviewManagement.Models.Job", null)
                        .WithMany()
                        .HasForeignKey("JobsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InterviewManagement.Models.Skill", null)
                        .WithMany()
                        .HasForeignKey("SkillsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("InterviewManagement.Models.Candidate", b =>
                {
                    b.Navigation("Offers");

                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("InterviewManagement.Models.Contract", b =>
                {
                    b.Navigation("Offers");
                });

            modelBuilder.Entity("InterviewManagement.Models.Department", b =>
                {
                    b.Navigation("Employees");

                    b.Navigation("Offers");
                });

            modelBuilder.Entity("InterviewManagement.Models.Employee", b =>
                {
                    b.Navigation("Candidates");
                });

            modelBuilder.Entity("InterviewManagement.Models.Job", b =>
                {
                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("InterviewManagement.Models.Level", b =>
                {
                    b.Navigation("Candidates");

                    b.Navigation("Offers");
                });

            modelBuilder.Entity("InterviewManagement.Models.Position", b =>
                {
                    b.Navigation("Candidates");

                    b.Navigation("Offers");
                });
#pragma warning restore 612, 618
        }
    }
}
