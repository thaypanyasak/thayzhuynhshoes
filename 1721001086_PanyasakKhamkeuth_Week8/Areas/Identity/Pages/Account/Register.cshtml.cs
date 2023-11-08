// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using _1721001086_PanyasakKhamkeuth_Week8.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Web.Helpers;

namespace _1721001086_PanyasakKhamkeuth_Week8.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            [Required]
            public string Name { get; set; }
            public string Address { get; set; }
            public string Role { get; set; }
            public string ImageUrl { get; set; } = "https://assets.stickpng.com/images/585e4bf3cb11b227491c339a.png";
            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("User")).GetAwaiter().GetResult();
            }
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            Input = new InputModel()
            {
                RoleList = _roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name,
                })
            };
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    Address = Input.Address,
                    ImageUrl = "https://assets.stickpng.com/images/585e4bf3cb11b227491c339a.png",
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    ////await _userManager.AddToRoleAsync(user, Input.Role);
                    await _userManager.AddToRoleAsync(user, "User");

                    // Generate an email confirmation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Encode the token for the email confirmation link
                    var emailConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    // Construct the email confirmation link
                    var emailConfirmationLink = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = emailConfirmationToken },
                        protocol: "https");

                    string eBody = $@"
                        <body style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 0; height: 100%;"">
                            <div style=""width: 650px; height: 550px; margin: 10px auto; border-radius: 10px; padding: 20px 40px; text-align: center; box-shadow: 0px 8px 24px rgba(149, 157, 165, 0.2); background-color: #ffffff;"">
                                <img src=""https://cdn-icons-png.flaticon.com/128/3062/3062634.png"" alt="""" />
                                <h1 style=""color: #535353"">Xác nhận Email</h1>
                                <p style=""color: #a0a0a0"">WELCOME TO THAY-HUYNH BALLER- Hãy nhấn vào nút dưới để xác nhận Email.</p>
                                <a
                                    style=""text-decoration: none; color: #fff; padding: 12px; background-color: #ff5b22; border-radius: 5px; margin: 30px 0; display: block; box-shadow: 0px 1px 4px rgba(0, 0, 0, 0.16);""
                                    href=""https://localhost:7082/Identity/Account/XacthucEmail?email={HtmlEncoder.Default.Encode(user.Email)}&return...clicking""
                                    >Click Here</a
                                >
                                <div class=""footer"" style=""margin-top: 180px; color: #a0a0a0;"">
                                    THẾ GIỚI GIÀY THỂ THAO <span style=""color: #ff5b22; font-weight: 700"">THAY-HUYNH</span> cảm ơn quý khách
                                </div>
                            </div>
                        </body>
";

                    string email = user.Email;
                    MailMessage message = new MailMessage(new MailAddress(email, "Confirm your email"), new MailAddress(email));
                    message.Subject = "Confirm your email";
                    message.Body = eBody;
                    message.IsBodyHtml = true;

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("khamkerd4000@gmail.com", "kymibagucjfltrib");
                    smtp.EnableSsl = true;

                    smtp.Send(message);


                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay the form
            return Page();
        }
        




        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
