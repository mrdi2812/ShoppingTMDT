using AutoMapper;
using BotDetect.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TMDT.Common;
using TMDT.Model.Models;
using TMDT.Service;
using TMDT.Web.App_Start;
using TMDT.Web.Models;
using TMDT.Web.Models.System;

namespace TMDT.Web.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IAnnouncementService _announcementService;
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, IAnnouncementService announcementService, IDataProtectionProvider dataProtectionProvider)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _announcementService = announcementService;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private async Task SignInAsync(AppUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }
        public AccountController()
        {

        }
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    ClaimsIdentity identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationProperties props = new AuthenticationProperties();
                    props.IsPersistent = model.RememberMe;
                    authenticationManager.SignIn(props, identity);                   
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {                     
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [CaptchaValidation("CaptchaCode", "registerCaptcha", "Mã xác nhận không đúng")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var userEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userEmail != null)
            {
                ModelState.AddModelError("email", "Email đã tồn tại");
                return View(model);
            }
            var userName = await _userManager.FindByNameAsync(model.UserName);
            if (userName != null)
            {
                ModelState.AddModelError("username", "Tài khoản đã tồn tại");
                return View(model);
            }
            string path = null;
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    path = Path.Combine(Server.MapPath("~/UploadedFiles/"), fileName);
                    file.SaveAs(path);
                    string url = "/ UploadedFiles /"+fileName;               
                    model.Avatar = url;
                }
            }

            var user = new AppUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = false,
                BirthDay = DateTime.Now,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName,
                Avatar = model.Avatar,
                Gender = model.Gender
            };
            await _userManager.CreateAsync(user, model.Password);
            var useraccount = await _userManager.FindByEmailAsync(model.Email);
            if (useraccount != null)
            {
                await _userManager.AddToRolesAsync(useraccount.Id, new string[] { "Member" });

                string confirmemail = System.IO.File.ReadAllText(Server.MapPath("/Templates/confirm_mail.html"));
                confirmemail = confirmemail.Replace("{{UserName}}", useraccount.FullName);
                var callbackUrl = Url.Action("SentEmailConfirm", "Account", new { Token = useraccount.Id, Email = useraccount.Email }, Request.Url.Scheme);
                //confirmemail = confirmemail.Replace("{{Link}}",ConfigHelper.GetByKey("CurrentLink")+ "sent-confirm-email.html?Token="+ useraccount.Id+"&Email="+ useraccount.Email);
                confirmemail = confirmemail.Replace("{{Link}}", callbackUrl);
                MailHelper.SendMail(useraccount.Email, "Xác thực tài khoản", confirmemail);
                //return RedirectToAction("Confirm", "Account", new { Email = user.Email });
                TempData["SuccessMsg"] = "Đăng ký thành công";
            }
            return View();
        }
        [AllowAnonymous]
        public async Task<ActionResult> SentEmailConfirm(string Token, string Email)
        {
            AppUser user = await _userManager.FindByIdAsync(Token);

            if (Token != null && Email == user.Email)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                string content = System.IO.File.ReadAllText(Server.MapPath("/Templates/newuser.html"));
                content = content.Replace("{{UserName}}", user.FullName);
                content = content.Replace("{{Link}}", ConfigHelper.GetByKey("CurrentLink") + "dang-nhap.html");
                MailHelper.SendMail(user.Email, "Đăng ký thành công", content);
                await SignInAsync(user, isPersistent: false);
                return RedirectToAction("ConfirmEmail", "Account", new { Email = user.Email });


            }
            else
            {
                ModelState.AddModelError("email", "Tài khoản chưa được xác minh");
                return RedirectToAction("Confirm", "Account", new { Email = user.Email });
            }

        }
        public ActionResult ConfirmEmail(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }
        public ActionResult Info()
        {
            var user = _userManager.FindById(User.Identity.GetUserId());
            var userVm = Mapper.Map<AppUser, ApplicationUserViewModel>(user);
            return View(userVm);
        }
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    return View(model);
                }
                string confirmemail = System.IO.File.ReadAllText(Server.MapPath("/Templates/reset_password.html"));
                var provider = new DpapiDataProtectionProvider("YourAppName");
                _userManager.UserTokenProvider = new DataProtectorTokenProvider<AppUser>(provider.Create("EmailConfirmation"));
                string code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                confirmemail = confirmemail.Replace("{{Link}}", callbackUrl);
                MailHelper.SendMail(user.Email, "Xác thực tài khoản", confirmemail);
                TempData["SuccessMsg"] = "Đăng ký thành công";
            }
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("email", "Email không tồn tại");
            }
            var provider = new DpapiDataProtectionProvider("YourAppName");
            _userManager.UserTokenProvider = new DataProtectorTokenProvider<AppUser>(provider.Create("PasswordConfirmation"));
            string code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await _userManager.ResetPasswordAsync(user.Id,code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
    }
}