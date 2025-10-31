using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DTOs.Requests {
    public class LoginRequest {
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Email is invalid!")]
        public string? AccountEmail { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [MinLength(2, ErrorMessage = "Password must be at least 2 characters!")]
        public string? AccountPassword { get; set; }
    }

    public class CreateAccountRequest {
        [Required(ErrorMessage = "Name is required!")]
        public string? AccountName { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Email is invalid!")]
        public string? AccountEmail { get; set; }

        [Required(ErrorMessage = "Role is required!")]
        public int? AccountRole { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [MinLength(2, ErrorMessage = "Password must be at least 2 characters!")]
        public string? AccountPassword { get; set; }
    }

    public class UpdateAccountRequest {
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        public string? AccountName { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Email is invalid!")]
        public string? AccountEmail { get; set; }

        [Required(ErrorMessage = "Role is required!")]
        public int? AccountRole { get; set; }
    }

    public class UpdatePasswordRequest : IValidatableObject {
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [MinLength(2, ErrorMessage = "Password must be at least 2 characters!")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [MinLength(2, ErrorMessage = "Password must be at least 2 characters!")]
        public string? NewPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrWhiteSpace(NewPassword) && NewPassword == OldPassword) {
                yield return new ValidationResult(
                    "New password must be different from old password!",
                    new[] { nameof(NewPassword) }
                );
            }
        }
    }
}
