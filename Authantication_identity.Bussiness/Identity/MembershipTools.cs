using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Authantication_identity.DataAccess;
using Authantication_identity.Entities.IdentityModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace Authantication_identity.Bussiness.Identity
{
   public class MembershipTools
   {
       private static DB _dB;
       public static UserStore<User> NewUserStore()=>new UserStore<User>(_dB?? new DB());
       public static UserManager<User> NewUserManager()=>new UserManager<User>(NewUserStore());
       public static RoleStore<Role> NewRoleStore()=>new RoleStore<Role>(_dB??new DB());
       public static RoleManager<Role> NewRoleManager()=>new RoleManager<Role>(NewRoleStore());

       public static string GetNameSurname(string userId)
       {
           User user;
           if (string.IsNullOrEmpty(userId))
           {
               var id = HttpContext.Current.User.Identity.GetUserId();
               if (string.IsNullOrEmpty(id))
               {
                   return "";
               }

               user = NewUserManager().FindById(id);
           }
           else
           {
               user = NewUserManager().FindById(userId);
               if (user==null)
               {
                   return null;
               }
           }

           return $"{user.Name} {user.Surname}";
       }
   }
}
