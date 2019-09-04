using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserSystemWb.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Surname { get; set; }
        [Required,EmailAddress,DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required,DataType(DataType.Password)]
        public string Password { get; set; }
        [Required,Compare(nameof(Password)),DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
