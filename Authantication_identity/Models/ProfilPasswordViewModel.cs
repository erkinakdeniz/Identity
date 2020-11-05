using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Authantication_identity.Models
{
    public class ProfilPasswordViewModel
    {
        public UserProfileViewModel userProfileViewModel { get; set; }
        public ChangePassword changePassword { get; set; }
    }
}