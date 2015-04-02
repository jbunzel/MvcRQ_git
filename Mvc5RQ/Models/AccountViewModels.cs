using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mvc5RQ.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "am_email", ResourceType = typeof(RQResources.Views.Shared.SharedStrings))]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceType = typeof(RQResources.Views.Shared.SharedStrings), ErrorMessageResourceName = "am_password_error", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "am_password", ResourceType = typeof(RQResources.Views.Shared.SharedStrings))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "am_password_confirm", ResourceType = typeof(RQResources.Views.Shared.SharedStrings))]
        [Compare("Password", ErrorMessageResourceType = typeof(RQResources.Views.Shared.SharedStrings), ErrorMessageResourceName = "am_password_confirm_error")]
        public string ConfirmPassword { get; set; }

        public string RegisterExplanation {get; set;}
        public string PasswordExplanation { get; set;}

        public RegisterViewModel ()
        {
            RegisterExplanation = (string)(RQResources.Views.Shared.SharedStrings.ResourceManager.GetObject("RegisterExplanation"));
            PasswordExplanation = RQResources.Views.Shared.SharedStrings.am_password_help;
        }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

        public string PasswordExplanation { get; set;}

        public ResetPasswordViewModel ()
        {
            PasswordExplanation = RQResources.Views.Shared.SharedStrings.am_password_help;
        }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}