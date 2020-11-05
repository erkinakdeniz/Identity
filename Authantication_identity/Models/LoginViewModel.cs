using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Authantication_identity.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name ="Kullanıcı Adı")]
        public string UserName { get; set; }
        [StringLength(100,ErrorMessage = "Şifreniz en az 5 karakter olmalıdır'")]
        [Display(Name ="Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name ="Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}