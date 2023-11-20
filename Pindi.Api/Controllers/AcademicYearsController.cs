using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Pindi.Api.Data;
using Pindi.Api.DTOs;
using Pindi.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pindi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicYearsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public AcademicYearsController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/AcademicYears/5/courses
        [HttpGet("{id}/courses")]
        [ResponseCache(Duration = 60)] // Output Cache for 60 seconds
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByAcademicYear(int id)
        {
            // Check if the data is in cache
            if (_cache.TryGetValue($"AcademicYear_{id}_Courses", out IEnumerable<CourseDto>? cachedCourses))
            {
                return Ok(cachedCourses);
            }

            // Retrieve data from the database
            var academicYear = await _context.AcademicYears
                .Include(ay => ay.Courses)
                .FirstOrDefaultAsync(ay => ay.Id == id);

            if (academicYear == null)
            {
                return NotFound();
            }

            var courses = academicYear.Courses.Select(c => new CourseDto
            {
                Code = c.Code,
                Name = c.Name,
                Description = c.Description,
                LecturerName = c.Lecturer.Name,
            }).ToList();

            // Store data in cache
            _cache.Set($"AcademicYear_{id}_Courses", courses, TimeSpan.FromMinutes(5));

            var response = new
            {
                TotalCourses = courses.Count(),
                Courses = courses
            };

            return Ok(response);
        }

    }
}