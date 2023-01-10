using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsManager.Core.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Name cannot be blank")]
        public string PersonName { get; set; }
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Email should be in a proper email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone cannot be blank")]
        [RegularExpression("[0-9]*$", ErrorMessage = "Phone number should contain numbers only")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Password cannot be blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Password cannot be blank")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
