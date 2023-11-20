using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Pindi.Api.Data;
using Pindi.Api.DTOs;
using Pindi.Api.Models;
using Pindi.Api.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Pindi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _memoryCache;
        public CoursesController(AppDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }


        [HttpGet]
        [Route("GetAllCourses")]
        public ActionResult GetAllCourses() 
        {
            var cacheKey = "AllCourses";

            if (_memoryCache.TryGetValue(cacheKey, out List<CourseDto>? cached))
            {
                return Ok(new {TotalCount = cached?.Count, Courses = cached });
            }

            var AllCourses = _context.Courses
                .AsNoTracking()
                .Include(c => c.Lecturer)
                .Include(c => c.AcademicYear)
                .Include(c => c.DegreeProgram)
                .Select(c => new CourseDto
                {
                    Code = c.Code,
                    Name = c.Name,
                    Description = c.Description,
                    LecturerName = c.Lecturer.Name,
                    Programs = c.DegreeProgram.Select(d => new DegreeProgramDto
                    {
                        Name = d.Name
                    }).ToList(),
                    Years = c.AcademicYear.Select(y => new YearDto
                    {
                        Year = y.Year
                    }).ToList(),
                })
            .ToList();

            _memoryCache.Set(cacheKey, AllCourses, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(2)
            });

            var response = new
            {
                TotalCoures = AllCourses.Count,
                Courses = AllCourses
            };

            return Ok(response);
        }

        [HttpGet("GetSingleCourse/{CourseCode}")]
        [ResponseCache(Duration = 60)]
        public ActionResult GetSingleCourse(String CourseCode)
        {
            var cacheKey = $"Course-{CourseCode}";

            if (_memoryCache.TryGetValue(cacheKey, out CourseDto? cached))
            {
                return Ok(cached);
            }

            var query = _context.Courses
                .AsNoTracking()
                .Include(c => c.Lecturer)
                .Include(c => c.AcademicYear)
                .Include(c => c.DegreeProgram)
                .SingleOrDefault(i => i.Code == CourseCode);

            if (query == null)
            {
                return NotFound();
            }

            var Course = new CourseDto 
            {
                Code = query.Code,
                Name = query.Name,
                Description = query.Description,
                LecturerName = query.Lecturer.Name,
                Programs = query.DegreeProgram.Select(d => new DegreeProgramDto
                {
                    Name = d.Name
                }).ToList(),
                Years = query.AcademicYear.Select(y => new YearDto
                {
                    Year = y.Year
                }).ToList(),
            };

            _memoryCache.Set(cacheKey, Course, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(3)
            });

            return Ok();
        }

        [HttpGet("GetCourseByLecture/{LecturerName}")]
        [ResponseCache(Duration = 60)]
        public ActionResult GetCourseByLecture(String LecturerName)
        {
            var cacheKey = $"Course-Lecturer-{LecturerName}";

            if (_memoryCache.TryGetValue(cacheKey, out CourseDto? cached))
            {
                return Ok(cached);
            }


            var courses = _context.Courses
                .Where(course => course.Lecturer.Name == LecturerName)
                .Select(c => new CourseDto
                {
                    Code = c.Code,
                    Name = c.Name,
                    Description = c.Description,
                    LecturerName = c.Lecturer.Name,
                    Programs = c.DegreeProgram.Select(a => new DegreeProgramDto
                    {
                        Name = a.Name
                    }).ToList(),
                    Years = c.AcademicYear.Select(y => new YearDto
                    {
                        Year = y.Year
                    }).ToList()
                }).ToList();

            if (courses is null)
            {
                return NotFound();
            }

            _memoryCache.Set(cacheKey, courses, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });

            return Ok();
        }


        [HttpGet("{CourseCode}/GetAllStudents")]
        [ResponseCache(Duration = 60)]
        public ActionResult GetAllStudents(String CourseCode)
        {

            var cacheKey = $"Course-Students-{CourseCode}";

            if (_memoryCache.TryGetValue(cacheKey, out List<StudentDto>? cached))
            {
                return Ok(cached);
            }


            var enrolledStudents = _context.Courses.AsNoTracking().Where(c => c.Code == CourseCode).SelectMany(c => c.Students).ToList();

            if (enrolledStudents is null)
            {
                return NotFound("Students are yet to enroll");
            }

            var students = enrolledStudents.Select(s => new StudentDto
            {
                RegNo = s.RegistrationNumber,
                Name = s.Name,
                DegreeProgram = s.Program.Name
            }).ToList();

            _memoryCache.Set(cacheKey, students, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(3)
            });


            var response = new
            {
                TotalStudents = students.Count,
                Students = students
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("AddCourse")]
        public async Task<ActionResult> AddCourse(CreateCourseDto request)
        {
            try
            {
                var lecturer = _context.lecturers.FirstOrDefault(l => l.Name == request.LecturerName);

                if (lecturer is null)
                {
                    return NotFound("No Lecturer exist with that name");
                }

                String key = PasswordService.GeneratePassword(4);

                Course Course = new Course
                {
                    Code = request.Code,
                    Name = request.Name,
                    Description = request.Description,
                    EnrollmentKey = key,
                    Lecturer = lecturer,
                    DegreeProgram = request.Programs,
                    AcademicYear = request.Years
                };

                _context.Courses.Add(Course);

                await _context.SaveChangesAsync();

                var response = new
                {
                    Message = "Course added successfully",
                    EnrollmentKey = key
                };

                return Ok(response);
            }
            catch
            {
                return BadRequest("Course name already exist");
            }
            
        }


    }
}
