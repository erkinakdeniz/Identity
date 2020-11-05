using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Authantication_identity.Models
{
    public class RegisterViewModel
    {

        [StringLength(25)]
        [Required]
        [Display(Name = "Ad")]
        public string Name { get; set; }

        [StringLength(35)]
        [Required]
        [Display(Name = "Soyad")]
        public string Surname { get; set; }

        [Required]
        [Display(Name ="Kullanıcı adı")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100,ErrorMessage = "Şifreniz en az 8 karakter olmalıdır!",MinimumLength =8)]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Şifre Tekar")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "Şifreler uyuşmuyor!")]
        public string ConfirmPassword { get; set; }




    }
}