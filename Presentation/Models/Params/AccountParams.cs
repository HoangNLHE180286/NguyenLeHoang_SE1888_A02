using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.Params {
    public class LoginParams {
        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email. Please try again!")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        [MinLength(2, ErrorMessage = "Password must be at least 2 characters long!")]
        public string? Password { get; set; }
    }

    public class SearchAccountParams {
        public string? Keyword { get; set; }
        public string? Role { get; set; }
    }

    public class CreateUpdateAccountParams {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required!")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email. Please try again!")]
        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }
    }

    public class UpdatePasswordParams : IValidatableObject {
        public int Id { get; set; }

        public string? Name { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        [MinLength(2, ErrorMessage = "Password must be at least 2 characters long!")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        [MinLength(2, ErrorMessage = "Password must be at least 2 characters long!")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        [MinLength(2, ErrorMessage = "Password must be at least 2 characters long!")]
        public string? ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrWhiteSpace(NewPassword) && NewPassword == OldPassword) {
                yield return new ValidationResult(
                    "New password must be different from old password!",
                    new[] { nameof(NewPassword) }
                );
            }

            if (!string.IsNullOrWhiteSpace(NewPassword) && NewPassword != ConfirmPassword) {
                yield return new ValidationResult(
                    "Confirm password does not match new password!",
                    new[] { nameof(ConfirmPassword) }
                );
            }
        }
    }
}
