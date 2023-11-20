using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pindi.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DegreePrograms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DegreePrograms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lecturers",
                columns: table => new
                {
                    StaffNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lecturers", x => x.StaffNumber);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    RegistrationNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProgramId = table.Column<int>(type: "int", nullable: false),
                    YearId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.RegistrationNumber);
                    table.ForeignKey(
                        name: "FK_Students_AcademicYears_YearId",
                        column: x => x.YearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_DegreePrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "DegreePrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EnrollmentKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LecturerStaffNumber = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Courses_lecturers_LecturerStaffNumber",
                        column: x => x.LecturerStaffNumber,
                        principalTable: "lecturers",
                        principalColumn: "StaffNumber");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role = table.Column<bool>(type: "bit", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    studentRegistrationNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    lecturerStaffNumber = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Students_studentRegistrationNumber",
                        column: x => x.studentRegistrationNumber,
                        principalTable: "Students",
                        principalColumn: "RegistrationNumber");
                    table.ForeignKey(
                        name: "FK_Users_lecturers_lecturerStaffNumber",
                        column: x => x.lecturerStaffNumber,
                        principalTable: "lecturers",
                        principalColumn: "StaffNumber");
                });

            migrationBuilder.CreateTable(
                name: "AcademicYearCourse",
                columns: table => new
                {
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    CoursesCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYearCourse", x => new { x.AcademicYearId, x.CoursesCode });
                    table.ForeignKey(
                        name: "FK_AcademicYearCourse_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcademicYearCourse_Courses_CoursesCode",
                        column: x => x.CoursesCode,
                        principalTable: "Courses",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseDegreeProgram",
                columns: table => new
                {
                    CoursesCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DegreeProgramId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseDegreeProgram", x => new { x.CoursesCode, x.DegreeProgramId });
                    table.ForeignKey(
                        name: "FK_CourseDegreeProgram_Courses_CoursesCode",
                        column: x => x.CoursesCode,
                        principalTable: "Courses",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseDegreeProgram_DegreePrograms_DegreeProgramId",
                        column: x => x.DegreeProgramId,
                        principalTable: "DegreePrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseStudent",
                columns: table => new
                {
                    EnrolledCoursesCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentsRegistrationNumber = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudent", x => new { x.EnrolledCoursesCode, x.StudentsRegistrationNumber });
                    table.ForeignKey(
                        name: "FK_CourseStudent_Courses_EnrolledCoursesCode",
                        column: x => x.EnrolledCoursesCode,
                        principalTable: "Courses",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseStudent_Students_StudentsRegistrationNumber",
                        column: x => x.StudentsRegistrationNumber,
                        principalTable: "Students",
                        principalColumn: "RegistrationNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicYearCourse_CoursesCode",
                table: "AcademicYearCourse",
                column: "CoursesCode");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicYears_Year",
                table: "AcademicYears",
                column: "Year",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseDegreeProgram_DegreeProgramId",
                table: "CourseDegreeProgram",
                column: "DegreeProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_EnrollmentKey",
                table: "Courses",
                column: "EnrollmentKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_LecturerStaffNumber",
                table: "Courses",
                column: "LecturerStaffNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Name",
                table: "Courses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudent_StudentsRegistrationNumber",
                table: "CourseStudent",
                column: "StudentsRegistrationNumber");

            migrationBuilder.CreateIndex(
                name: "IX_DegreePrograms_Name",
                table: "DegreePrograms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lecturers_Name",
                table: "lecturers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_Name",
                table: "Students",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_ProgramId",
                table: "Students",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_YearId",
                table: "Students",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_lecturerStaffNumber",
                table: "Users",
                column: "lecturerStaffNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Users_studentRegistrationNumber",
                table: "Users",
                column: "studentRegistrationNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicYearCourse");

            migrationBuilder.DropTable(
                name: "CourseDegreeProgram");

            migrationBuilder.DropTable(
                name: "CourseStudent");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "lecturers");

            migrationBuilder.DropTable(
                name: "AcademicYears");

            migrationBuilder.DropTable(
                name: "DegreePrograms");
        }
    }
}
