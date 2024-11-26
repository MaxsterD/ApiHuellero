using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ApiConsola.Services.DTOs
{
    public class UpdateDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [PasswordPropertyText]
        public string NewPassword { get; set; }
    }
}
