using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using MMOServer.Models;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using MMOServer.ViewModels.Account;
using System;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MMOServer.Controllers
{
  public class AccountController : Controller
  {
    private readonly UserManager<Account> userManager;
    private readonly SignInManager<Account> signInManager;
    private readonly ILogger<AccountController> logger;

    public AccountController(UserManager<Account> userManager,
                             SignInManager<Account> signInManager,
                             ILogger<AccountController> logger)
    {
      this.userManager = userManager;
      this.signInManager = signInManager;
      this.logger = logger;
    }


    [HttpGet]
    public async Task<IActionResult> AddPassword()
    {
      var user = await userManager.GetUserAsync(User);

      var userHasPassword = await userManager.HasPasswordAsync(user);

      if (userHasPassword)
      {
        return RedirectToAction("ChangePassword");
      }

      return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await userManager.GetUserAsync(User);

        var result = await userManager.AddPasswordAsync(user, model.NewPassword);

        if (!result.Succeeded)
        {
          foreach (var error in result.Errors)
          {
            ModelState.AddModelError(string.Empty, error.Description);
          }
          return View();
        }

        await signInManager.RefreshSignInAsync(user);

        return View("AddPasswordConfirmation");
      }

      return View(model);
    }


    //only logged in users can change password, no anonymous access
    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
      var user = await userManager.GetUserAsync(User);

      var userHasPassword = await userManager.HasPasswordAsync(user);

      if (!userHasPassword)
      {
        return RedirectToAction("AddPassword");
      }

      return View();
    }

    //only logged in users can change password, no anonymous access
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
          return RedirectToAction("Login");
        }

        // ChangePasswordAsync changes the user password
        var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        // The new password did not meet the complexity rules or
        // the current password is incorrect. Add these errors to
        // the ModelState and rerender ChangePassword view
        if (!result.Succeeded)
        {
          foreach (var error in result.Errors)
          {
            ModelState.AddModelError(string.Empty, error.Description);
          }
          return View();
        }

        // Upon successfully changing the password refresh sign-in cookie
        await signInManager.RefreshSignInAsync(user);
        return View("ChangePasswordConfirmation");
      }

      return View(model);
    }


    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string token, string email)
    {
      // If password reset token or email is null, most likely the
      // user tried to tamper the password reset link
      if (token == null || email == null)
      {
        ModelState.AddModelError("", "Invalid password reset token");
      }
      return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
          var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
          if (result.Succeeded)
          {
            // Upon successful password reset and if the account is lockedout, set
            // the account lockout end date to current UTC date time, so the user
            // can login with the new password
            if (await userManager.IsLockedOutAsync(user))
            {
              await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            }
            return View("ResetPasswordConfirmation");
          }

          foreach (var error in result.Errors)
          {
            ModelState.AddModelError("", error.Description);
          }
          return View(model);
        }

        return View("ResetPasswordConfirmation");
      }
      return View(model);
    }


    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
      return View();
    }


    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        // Find the user by email
        var user = await userManager.FindByEmailAsync(model.Email);
        // If the user is found AND Email is confirmed
        if (user != null && await userManager.IsEmailConfirmedAsync(user))
        {
          // Generate the reset password token
          var token = await userManager.GeneratePasswordResetTokenAsync(user);

          // Build the password reset link
          var passwordResetLink = Url.Action("ResetPassword", "Account",
                  new { email = model.Email, token = token }, Request.Scheme);

          // Log the password reset link
          logger.Log(LogLevel.Warning, passwordResetLink);

          // Send the user to Forgot Password Confirmation view
          return View("ForgotPasswordConfirmation");
        }

        // To avoid account enumeration and brute force attacks, don't
        // reveal that the user does not exist or is not confirmed
        return View("ForgotPasswordConfirmation");
      }

      return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
      await signInManager.SignOutAsync();
      return RedirectToAction("index", "home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
      return View();
    }

    [AcceptVerbs("Get", "Post")]
    [AllowAnonymous]
    public async Task<IActionResult> IsEmailInUse(string email)
    {
      var user = await userManager.FindByEmailAsync(email);
    
      if (user == null)
      {
        return Json(true);
      }
      else 
      {
        return Json($"E-Mail: {email} is already in use");
      }
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        var user = new Account
        {
          UserName = model.Email,
          Email = model.Email,
          City = model.City
        };
        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
          var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
          var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
          logger.Log(LogLevel.Warning, confirmationLink);

          // If the user is signed in and in the Admin role, then it is
          // the Admin user that is creating a new user. So redirect the
          // Admin user to ListRoles action
          if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
          {
            return RedirectToAction("ListUsers", "Administration");
          }

          // https://localhost:44365/Account/ConfirmEmail?userId=2b4b33c0-e692-4821-af72-a8f16f8b1361&token=CfDJ8NKizQwu8l1Bho9MlF6dE3raiONFDU6ILFd2UbVtGK00zsJ6esOMgjpwztzT95wyE1Sc5RjDKiXKIV6JCzDquLg6FgFJsu9HXfBicaSv32yrLtVAx2vAKXEx7obZuL7MznP4Jf%2BF0lc6kbC4n%2BwMPnvkIbQGZ9YVV4v8K%2BoWNiciKprrzfkKj%2FcD3hyTpV0WqFLQ%2BpFrdpe4dECtObXRKFtUjORdeVaahCTOkOzihG6pwoWHSEX1ABiR9X8vXEI9JQ%3D%3D
          ViewBag.ErrorTitle = "Registration successful";
          ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                                 "email, by clicking on the confirmation link we have emailed you";
          return View("Error");
        }

        foreach(var error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }

      }

      return View(model);
    }

    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
      if (userId == null || token == null)
      {
        return RedirectToAction("index", "home");
      }

      var user = await userManager.FindByIdAsync(userId);
      if (user == null)
      {
        ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
        return View("NotFound");
      }

      var result = await userManager.ConfirmEmailAsync(user, token);
      if (result.Succeeded)
      {
        return View();
      }

      ViewBag.ErrorTitle = "Email cannot be confirmed";
      return View("Error");
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string returnURL)
    {
      LoginViewModel model = new LoginViewModel
      {
        ReturnUrl = returnURL,
        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
      };

      return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
    {
      model.ExternalLogins =
         (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

      if (ModelState.IsValid)
      {
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user != null && !user.EmailConfirmed &&
            (await userManager.CheckPasswordAsync(user, model.Password)))
        {
          ModelState.AddModelError(string.Empty, "Email not confirmed yet");
          return View(model);
        }

        // The last boolean parameter lockoutOnFailure indicates if the account should be locked on failed logon attempt. On every failed logon
        // attempt AccessFailedCount column value in AspNetUsers table is incremented by 1. When the AccessFailedCount reaches the configured
        // MaxFailedAccessAttempts which in our case is 5, the account will be locked and LockoutEnd column is populated. After the account is
        // lockedout, even if we provide the correct username and password, PasswordSignInAsync() method returns Lockedout result and the login
        // will not be allowed for the duration the account is locked.
        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

        if (result.Succeeded)
        {
          if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
          {
            return Redirect(returnUrl);
          }
          else
          {
            return RedirectToAction("index", "home");
          }
        }

        // If account is lockedout send the use to AccountLocked view
        if (result.IsLockedOut)
        {
          return View("AccountLocked");
        }

        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
      }

      return View(model);
    }

/*
API Call https://localhost:44365/account/LoginAndCreateSession
{
"Email":"SuperAdmin@mail.com",
"Password":"Abc123!"
}
 */
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAndCreateSession([FromBody]LoginAndCreateSessionViewModel model)
    {
      var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, true);

      var response = new LoginAndCreateSessionResponseViewModel();

      if (result.Succeeded)
      {
        var user = await userManager.FindByNameAsync(model.Email);
        
        response.UserId = user.Id;
        response.ErrorMessage = String.Empty;
      }
      else
      {
        response.UserId = String.Empty;
        response.ErrorMessage = "Login Failed";
      }

      return Json(response);
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult ExternalLogin(string provider, string returnUrl)
    {
      var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });

      var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

      return new ChallengeResult(provider, properties);
    }


    [AllowAnonymous]
    public async Task<IActionResult>
    ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
      returnUrl = returnUrl ?? Url.Content("~/");

      LoginViewModel loginViewModel = new LoginViewModel
      {
        ReturnUrl = returnUrl,
        ExternalLogins =
          (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
      };

      if (remoteError != null)
      {
        ModelState.AddModelError(string.Empty,
            $"Error from external provider: {remoteError}");

        return View("Login", loginViewModel);
      }

      var info = await signInManager.GetExternalLoginInfoAsync();
      if (info == null)
      {
        ModelState.AddModelError(string.Empty,
            "Error loading external login information.");

        return View("Login", loginViewModel);
      }

      var email = info.Principal.FindFirstValue(ClaimTypes.Email);
      Account user = null;

      if (email != null)
      {
        user = await userManager.FindByEmailAsync(email);

        if (user != null && !user.EmailConfirmed)
        {
          ModelState.AddModelError(string.Empty, "Email not confirmed yet");
          return View("Login", loginViewModel);
        }
      }

      var signInResult = await signInManager.ExternalLoginSignInAsync(
                                  info.LoginProvider, info.ProviderKey,
                                  isPersistent: false, bypassTwoFactor: true);

      if (signInResult.Succeeded)
      {
        return LocalRedirect(returnUrl);
      }
      else
      {
        if (email != null)
        {
          if (user == null)
          {
            user = new Account
            {
              UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
              Email = info.Principal.FindFirstValue(ClaimTypes.Email)
            };

            await userManager.CreateAsync(user);

            // After a local user account is created, generate and log the
            // email confirmation link
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                            new { userId = user.Id, token = token }, Request.Scheme);

            logger.Log(LogLevel.Warning, confirmationLink);

            ViewBag.ErrorTitle = "Registration successful";
            ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                "email, by clicking on the confirmation link we have emailed you";
            return View("Error");
          }

          await userManager.AddLoginAsync(user, info);
          await signInManager.SignInAsync(user, isPersistent: false);

          return LocalRedirect(returnUrl);
        }

        ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
        ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

        return View("Error");
      }
    }



  }
}
