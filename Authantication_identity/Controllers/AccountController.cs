using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Authantication_identity.Bussiness.Helpers;
using Authantication_identity.Bussiness.Identity;
using Authantication_identity.Bussiness.Services.Senders;
using Authantication_identity.Entities.IdentityModels;
using Authantication_identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using static Authantication_identity.Bussiness.Identity.MembershipTools;

namespace Authantication_identity.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [HttpGet]
        public ActionResult Register()
        {
            if (HttpContext.GetOwinContext().Authentication.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register",model);
            }

            try
            {
                var userStore = NewUserStore();
                var userManeger = NewUserManager();
                var roleManager = NewRoleManager();
                var user = await userManeger.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    ModelState.AddModelError("UserName", "Bu kullanıcı adı daha önceden alınmıştır.");
                    return View("Register", model);
                }

                var newUser = new User()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    ActivationCode = new StringHelper().GetCode()
                    
                };
              var result= await userManeger.CreateAsync(newUser,model.Password);
              if (result.Succeeded)
              {
                  if (userStore.Users.Count()==1)
                  {
                     await userManeger.AddToRoleAsync(newUser.Id, "Admin");
                     
                  }
                  else
                  {
                      await userManeger.AddToRoleAsync(newUser.Id, "User");
                  }

                  string SiteUrl = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                                   (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                  var emailservice=new EmailService();
                  string body = $"Merhaba <b>{newUser.Name} {newUser.Surname}</b></br>Hesabınızı Aktif etmek için aşağıda ki linke tıklayınız<br> <a href='{SiteUrl}/Account/Activation?code={newUser.ActivationCode}'>Aktivasyon Linki</a>";
                  await emailservice.SendAsync(new IdentityMessage() {Body = body, Subject = "Sitemize Hoş Geldiniz"},newUser.Email);
              }
              else
              {
                  var err = "";
                  foreach (var resultError in result.Errors)
                  {
                      err += resultError + " ";
                  }
                  ModelState.AddModelError("", err.ToString());
                  return View("Register", model);
                }

              return View("Login");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Login", model);
                }

                var userManager = NewUserManager();
                var user = await userManager.FindAsync(model.UserName, model.Password);
                if (user==null)
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış");
                    return View("Login", model);
                }
                else if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError("", "Aktivasyon Yapılmadı!");
                    return View("Login", model);
                }

                var authManager = HttpContext.GetOwinContext().Authentication;
                var userIdentity =
                    await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                authManager.SignIn(new AuthenticationProperties()
                {
                    IsPersistent = model.RememberMe
                },userIdentity);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        [HttpGet]
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("Login", "Account");
        }
        [Authorize]
        public ActionResult UserProfile()
        {
            try
            {
                var id = HttpContext.GetOwinContext().Authentication.User.Identity.GetUserId();
                var user = NewUserManager().FindById(id);
                var model = new ProfilPasswordViewModel()
                {
                    
                   userProfileViewModel = new UserProfileViewModel()
                   {
                       Email = user.Email,
                       Id = user.Id,
                       Name = user.Name,
                       PhoneNumber = user.PhoneNumber,
                       Surname = user.Surname,
                       UserName = user.UserName
                   }
                };
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
       
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> UpdateProfile(ProfilPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            try
            {
                var userManager = NewUserManager();
                var user = await userManager.FindByIdAsync(model.userProfileViewModel.Id);
                user.Name = model.userProfileViewModel.Name;
                user.Surname = model.userProfileViewModel.Surname;
                user.PhoneNumber = model.userProfileViewModel.PhoneNumber;
                user.Email = model.userProfileViewModel.Email;
                user.UserName = model.userProfileViewModel.UserName;
                await userManager.UpdateAsync(user);
                TempData["Message"] = "Güncelleme işlemi başarılı";
                return RedirectToAction("UserProfile");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ProfilPasswordViewModel model)
        {
            var userManager = NewUserManager();
            var id = HttpContext.GetOwinContext().Authentication.User.Identity.GetUserId();
            var user = NewUserManager().FindById(id);
            var data = new ProfilPasswordViewModel()
            {

                userProfileViewModel = new UserProfileViewModel()
                {
                    Email = user.Email,
                    Id = user.Id,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Surname = user.Surname,
                    UserName = user.UserName
                }
            };
            model.userProfileViewModel = data.userProfileViewModel;
            try
            {
               
                if (!ModelState.IsValid)
                {
                   
                    return View("UserProfile", model);
                }
                
              var result=  await userManager.ChangePasswordAsync(HttpContext.GetOwinContext().Authentication.User.Identity.GetUserId(),model.changePassword.OldPassword,model.changePassword.NewPassword);
              if (result.Succeeded)
              {
                  return RedirectToAction("Logout");
              }
              else
              {
                  var err = "";
                  foreach (var resultError in result.Errors)
                  {
                      err += resultError + " ";
                  }
                  ModelState.AddModelError("", err.ToString());
                  return View("UserProfile", model);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Activation(string code)
        {
            try
            {
                var userstore = NewUserStore();
                var user = userstore.Users.FirstOrDefault(x => x.ActivationCode == code);
                if (user!=null)
                {
                    if (user.EmailConfirmed)
                    {
                        ViewBag.Message = $"<span class='text-success'>Aktivasyon Başarılı</span>";
                    }
                    else
                    {
                        user.EmailConfirmed = true;
                        userstore.Context.SaveChanges();
                        ViewBag.Message = $"<span class='text-success'>Aktivasyon Başarılı</span>";
                    }
                    
                }
                else
                {
                    ViewBag.Message = $"<span class='text-danger'>Aktivasyon Başarısız</span>";
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = $"<span class='text-success'>Aktivasyon işleminde bir hata oluştu</span>";
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult RecoveryPassport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RecoveryPasswort(RecoveryPasswordViewModel model)
        {
            try
            {
                var userManager = NewUserManager();
                var userStore = NewUserStore();
                var user = await userStore.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty,$"{model.Email} mail adresine kayıtlı bir üyeliğe erişilemedi!");
                    return View(model);
                }
                
                var newPassword=new StringHelper().GetCode().Substring(0,8);
               await userStore.SetPasswordHashAsync(user, userManager.PasswordHasher.HashPassword(newPassword));
               var result= userStore.Context.SaveChanges();
               if (result == 0)
               {
                   ModelState.AddModelError(string.Empty,$"Hata oluştu");
               }
                var emailService=new EmailService();
                var body =
                    $"Merhaba <b>{user.Name} {user.Surname}</b> <br>Hesabınızım parolası sıfırlanmıştır!<br> Yeni Parolanız:<b> {newPassword}</b>";
                emailService.Send(new IdentityMessage() {Body = body, Subject = "Şifre Kurtarma"}, user.Email);
                return RedirectToAction("Login", "Account");
            }
            catch
            {
                return View(model);
            }
        }
    }
}