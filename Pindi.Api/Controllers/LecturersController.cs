using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pindi.Api.Data;
using Pindi.Api.Models;

namespace Pindi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LecturersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/lecturers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lecturer>>> GetLecturers()
        {
            var lecturers = await _context.lecturers.ToListAsync();
            return Ok(lecturers);
        }

        // GET: api/lecturers/5
        [HttpGet("{staffNumber}")]
        public async Task<ActionResult<Lecturer>> GetLecturer(string staffNumber)
        {
            var lecturer = await _context.lecturers
                .Include(l => l.Courses)
                .FirstOrDefaultAsync(l => l.StaffNumber == staffNumber);

            if (lecturer == null)
            {
                return NotFound();
            }

            return Ok(lecturer);
        }

        // GET: api/lecturers/5/courses
        [HttpGet("{staffNumber}/courses")]
        public async Task<ActionResult<IEnumerable<Course>>> GetLecturerCourses(string staffNumber)
        {
            var lecturerCourses = await _context.Courses
                .Where(c => c.Lecturer.StaffNumber == staffNumber)
                .ToListAsync();

            return Ok(lecturerCourses);
        }

        // POST: api/lecturers/5/courses
        [HttpPost("{staffNumber}/courses")]
        public async Task<ActionResult<Course>> AddCourseToLecturer(string staffNumber, Course course)
        {
            var lecturer = await _context.lecturers.FirstOrDefaultAsync(l => l.StaffNumber == staffNumber);

            if (lecturer == null)
            {
                return NotFound("Lecturer not found");
            }

            course.Lecturer = lecturer;
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLecturerCourses", new { staffNumber = lecturer.StaffNumber }, course);
        }
    }
}
