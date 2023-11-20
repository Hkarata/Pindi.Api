using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pindi.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public required Boolean Role { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [Phone]
        [MaxLength(9), MinLength(9)]
        public required string PhoneNumber { get; set; }

        [PasswordPropertyText]
        public required string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set;}
        public DateTime LastLoginAt { get; set; }
        public Student? student { get; set; }
        public Lecturer? lecturer { get; set; }
    }
}
