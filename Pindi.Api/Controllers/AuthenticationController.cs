using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pindi.Api.Data;
using Pindi.Api.DTOs;
using Pindi.Api.Models;
using Pindi.Api.Services;

namespace Pindi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuthenticationController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public ActionResult AuthenticateUser(AuthUser user)
        {
            if (!user.Role)
            {
                var student = _context.Users
                    .Include(s => s.student)
                    .FirstOrDefault(i => i.student.RegistrationNumber == user.Username);
                if (student is null)
                {
                    return NotFound();
                }
                
                student.LastLoginAt = DateTime.UtcNow;

                var response = new
                {
                    RegistrationNumber = student.student?.RegistrationNumber,
                    Name = student.student?.Name,
                    Role = user.Role
                };
                return Ok(response);
            }
            else
            {
                var lecturer = _context.Users
                .FirstOrDefault(i => i.lecturer.StaffNumber.Equals(user.Username) && i.Password == user.Password);

                if (lecturer is null)
                {
                    return NotFound();
                }

                lecturer.LastLoginAt = DateTime.UtcNow;

                var response = new
                {
                    StaffNumber = lecturer.lecturer?.StaffNumber,
                    Name = lecturer.lecturer?.Name,
                    Role = user.Role
                };
                return Ok(response);
            }
            
        }
    }
}
