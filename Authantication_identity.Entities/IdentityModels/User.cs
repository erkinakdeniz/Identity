﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace Authantication_identity.Entities.IdentityModels
{
   public class User:IdentityUser
    {
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
        [Required] 
        [StringLength(60)]
        public string Surname { get; set; }

        public string ActivationCode { get; set; }
    }
}
