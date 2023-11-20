using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Pindi.Api.Data;
using Pindi.Api.DTOs;
using Pindi.Api.Models;
using Pindi.Api.Services;

namespace Pindi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _memoryCache;
        public StudentsController(AppDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }


        [HttpPost]
        [Route("AddStudent")]
        public async Task<ActionResult> AddStudent(CreateStudentDto request)
        {
            var degProg = _context.DegreePrograms.FirstOrDefault(i => i.Name.Equals(request.Program));

            var year = _context.AcademicYears.FirstOrDefault(i => i.Year.Equals(request.Year));

            if (degProg != null && year != null)
            {
                var student = new Student
                {
                    RegistrationNumber = request.RegNo,
                    Name = request.Name,
                    Program = degProg,
                    Year = year
                };

                _context.Students.Add(student);

                var password = PasswordService.GeneratePassword(12);

                var user = new User
                {
                    Role = false,
                    Email = request.Email,
                    PhoneNumber = request.Phone,
                    CreatedAt = DateTime.UtcNow,
                    Password = PasswordService.HashPassword(password),
                    student = student
                };

                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                var response = new
                {
                    Message = "A Student Account and Provisional Password has been created",
                    Password = password,
                };

                return Ok(response);
            }

            return NotFound();
        }


        [HttpGet("{RegNo}")]
        [ResponseCache(Duration = 60)]
        public ActionResult GetSingleStudent(String RegNo)
        {
            var student = _context.Students.Include(s => s.Program).Include(s => s.Year).FirstOrDefault(s => s.RegistrationNumber.Equals(RegNo));
            if (student is null)
            {
                return NotFound("Student does not exist");
            }

            var response = new
            {
                RegNo = student.RegistrationNumber,
                Name = student.Name,
                Program = student.Program.Name,
                AcademicYear = student.Year.Year
            };
            return Ok(response);
        }

        [HttpGet("{RegNo}/MyCourses")]
        [ResponseCache(Duration = 60)]
        public ActionResult GetMyCourses(String RegNo)
        {
            var cacheKey = $"{RegNo}-MyCourses";

            if (_memoryCache.TryGetValue(cacheKey, out List<CourseDto>? cachedCourse))
            {
                return Ok(new { TotalCourser = cachedCourse?.Count, Courses = cachedCourse});
            }

            var enrolledCourses = _context.Students
                .Where(s => s.RegistrationNumber == RegNo)
                .SelectMany(s => s.EnrolledCourses).ToList();

            if (!enrolledCourses.Any())
            {
                return NotFound("Yet to enroll in any course");
            }

            var courses = enrolledCourses.Select(c => new
            { 
                Code = c.Code,
                Name = c.Name,
                Description = c.Description,
                LecturerName = c.Lecturer.Name
            }).ToList();

            _memoryCache.Set(cacheKey, courses, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });

            return Ok(courses);
        }

        [HttpPost]
        [Route("EnrollStudent")]
        public ActionResult EnrollStudent(String RegNo, String CourseCode)
        {
            var student = _context.Students.FirstOrDefault(s => s.RegistrationNumber == RegNo);
            var course = _context.Courses.FirstOrDefault(c => c.Code == CourseCode);

            if (student != null && course != null)
            {
                // Enroll the student to the course
                if (student.EnrolledCourses == null)
                {
                    student.EnrolledCourses = new List<Course>();
                }

                student.EnrolledCourses.Add(course);

                // Save changes to the database
                _context.SaveChanges();
            }

            return Ok();
        }


        [HttpPost]
        [Route("UnEnrollStudent")]

        public ActionResult UnEnrollStudent(String RegNo, String CourseCode)
        {
            // Retrieve the student and course
            var student = _context.Students.Include(s => s.EnrolledCourses).FirstOrDefault(s => s.RegistrationNumber == RegNo);

            var course = _context.Courses.FirstOrDefault(c => c.Code == CourseCode);

            // Check if both student and course exist
            if (student != null && course != null)
            {
                // Remove the course from the enrolled courses of the student
                student.EnrolledCourses?.Remove(course);

                // Save changes to the database
                _context.SaveChanges();
            }

            return Ok();
        }

    }
}
