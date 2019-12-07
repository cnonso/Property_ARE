using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PropertyManagerWeb.Models;
using System.Web.Helpers;

namespace PropertyManagerWeb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            //============ Extra Code============
            string registrationStatus = new DB().ReadData("Select RegistrationStatus from AspNetUsers where Email = '" + model.Email + "'");
            if (registrationStatus == "Incomplete")
                return RedirectToAction("ConfirmRegistration", new { Email = model.Email });
            //============ Extra Code============
            else
            {
                if(model == null)
                {
                    model.Email = Session["Email"].ToString();
                    model.Password = Session["Password"].ToString();
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                string usertype = new DB().ReadData("SELECT UserType FROM AspNetUsers WHERE Email = '" + model.Email + "'");
                Session["UserType"] = usertype;
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);

                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            Session["Username"] = model.Email;
            Session["Password"] = model.Password;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);


                if (result.Succeeded)
                { 
                    var db = new ApplicationDbContext();

                    //if (model.UserType == "Landlord")
                    //{
                    //    var landlord = new Landlord
                    //    { ApplicationUserId = user.Id };
                    //    db.Landlords.Add(landlord);
                    //    db.SaveChanges();
                    //}

                    //if (model.UserType == "Tenant")
                    //{
                    //    var tenant = new Tenant
                    //    { ApplicationUserId = user.Id };
                    //    db.Tenants.Add(tenant);
                    //    db.SaveChanges();
                    //}
                    //new DB().ExecuteQuery("Update AspNetUsers SET UserType = '" + model.UserType + "' WHERE Email = '" + model.Email + "'");

                    var landlord = new Landlord
                    { ApplicationUserId = user.Id };
                    db.Landlords.Add(landlord);
                    db.SaveChanges();

                    new DB().ExecuteQuery("Update AspNetUsers SET UserType = 'Landlord' WHERE Email = '" + model.Email + "'");

                    //The following line is commented to prevent log in until the user is confirmed.
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    string authCode = AuthCode();
                    new DB().ExecuteQuery("Update AspNetUsers SET AuthenticationCode = '" + authCode + "', RegistrationStatus= 'Incomplete' WHERE Email = '" + model.Email + "'");
                    try
                    {
                        SendCodeMail(model.Email, authCode);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.MailMsg = ex.Message;
                    }
                    return View("ConfirmRegistration"); 


                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    //return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            ViewBag.Msg = notification;
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
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
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        //================= Extra Methods ======================

        private static Random random = new Random();
        private static string notification = "";
        public static string AuthCode(int length = 8)
        {
            const string suffix = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string chars2 = "0123456789";
            return new string(Enumerable.Repeat(chars2, length)
              .Select(s => s[random.Next(s.Length)]).ToArray()) +
              new string(Enumerable.Repeat(suffix, 2)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void SendCodeMail(string recipientEmailAddy, string authCode)
        {
            try
            {
                //==================== Option One ============================
                //Configuring webMail class to send emails  
                //gmail smtp server  
                WebMail.SmtpServer = "smtp.gmail.com";
                //gmail port to send emails  
                WebMail.SmtpPort = 587;
                WebMail.SmtpUseDefaultCredentials = true;
                //sending emails with secure protocol  
                WebMail.EnableSsl = true;
                //EmailId used to send emails from application  
                WebMail.UserName = "chrysgodson@gmail.com";
                WebMail.Password = "100%Godworks!";
                //Sender email address.  
                WebMail.From = "chrysgodson@gmail.com";


                string mailBody = "Hello, we noticed you recently registered on to http://www.propertymanager.net. "
                    + "To verify that you actually initiated this, kindly provide the code: " + authCode
                    + " to complete your regisration process. Kindly disregard if you did not.";


                //Send email  
                //WebMail.Send(to: obj.ToEmail, subject: obj.EmailSubject, body: obj.EMailBody, cc: obj.EmailCC, bcc: obj.EmailBCC, isBodyHtml: true);
                WebMail.Send(to: recipientEmailAddy, subject: "Email Verification", body: mailBody, isBodyHtml: true);

                notification = "Email Sent Successfully.";
            }
            catch (Exception ex)
            {
                notification = "Problem while sending email, Please check details." + ex.Message;
                // For issue regarding message sending failure, go to security settings at the followig link https://www.google.com/settings/security/lesssecureapps and enable less secure apps . 
                //So that you will be able to login from all apps. 
                //You can also refer to this http://stackoverflow.com/questions/704636/sending-email-through-gmail-smtp-server-with-c-sharp/9572958#9572958

            }
        }

        private ActionResult ResendSendCodeMail(string email)
        {
            string authCode = AuthCode();
            new DB().ExecuteQuery("Update AspNetUsers SET AuthenticationCode = '" + authCode + "', RegistrationStatus= 'Incomplete' WHERE Email = '" + email + "'");
            try
            {
                SendCodeMail(email, authCode);
            }
            catch (Exception ex)
            {
                ViewBag.MailMsg = ex.Message;
            }
            return View("ConfirmRegistration");

        }


        [AllowAnonymous]
        public ActionResult ConfirmRegistration(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ConfirmRegistration(string email, LoginViewModel model)
        {

            string authCode = Request.Form["AuthCode"].ToString();
            string validAuthCode, applicationUserId,userType;
            string emailID = Session["Username"].ToString();// email;

            if (new DB().ReadData("SELECT UserName FROM AspNetUsers where Email = '" + emailID + "'").Trim() != string.Empty)
            {
                validAuthCode = new DB().ReadData("SELECT AuthenticationCode FROM AspNetUsers WHERE Email = '" + emailID + "'");
                applicationUserId = new DB().ReadData("SELECT Id FROM AspNetUsers WHERE Email = '" + emailID + "'");
                userType = new DB().ReadData("SELECT UserType FROM AspNetUsers WHERE Email = '" + emailID + "'");
                
                if (authCode == validAuthCode)
                {
                    new DB().ExecuteQuery("UPDATE AspNetUsers SET EmailConfirmed = '" + true + "', " +
                        "RegistrationStatus = 'Complete' WHERE Email = '" + emailID + "'");

                    if (userType.Trim() == "Landlord")
                    {
                        new DB().ExecuteQuery("UPDATE Landlords SET RegDate = '" + DateTime.Today.ToShortDateString() + "', " +
                            " LandlordID = '" + NewLandlordID(emailID) + "', Status = 'Active Unverified' " +
                            " WHERE ApplicationUserId = '" + applicationUserId + "'");
                    }
                    if (userType.Trim() == "Tenant")
                    {
                        new DB().ExecuteQuery("UPDATE Tenants SET RegDate = '" + DateTime.Today.ToShortDateString() + "', " +
                            " Status = 'Inactive' " +
                            " WHERE ApplicationUserId = '" + applicationUserId + "'");
                    }
                    //var db = new ApplicationDbContext();
                    //Landlord lnd = new Landlord();

                    //lnd.RegDate = DateTime.Today.ToShortDateString();
                    //lnd.LandlordID = NewLandlordID(emailID);
                    //lnd.Status = "Active";

                    //db.Entry(lnd).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();
                    //model.Email = Session["Username"].ToString();
                    //model.Password = Session["Password"].ToString();
                    //await Login(model, "/");
                    
                    return RedirectToAction( "Login");
                }
                else
                {
                    ViewBag.Notification = "Invalid authentication code.";
                    return View();
                }
            }
            else
            {
                ViewBag.Notification = "User does not exist.";
                return View();
            }

            return View();
        }

        public string NewLandlordID(string email)
        {
            string lastID = new DB().ReadData("Select TOP (1) IDforLand from Landlords order by IDforLand Desc");
            string firstInitial = email.Substring(0, 1).ToUpper();
            string secInitial = email.Substring(1, 1).ToUpper();
            return firstInitial + secInitial + lastID;
        }
    }
}