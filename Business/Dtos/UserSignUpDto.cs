using System.ComponentModel.DataAnnotations;

namespace Business.Dtos;

public class UserSignUpDto
{
    [Required(ErrorMessage = "Required")]
    [Display(Name = "First Name", Prompt = "Enter your First Name")]
    [DataType(DataType.Text)]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [Display(Name = "Last Name", Prompt = "Enter your Last Name")]
    [DataType(DataType.Text)]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [Display(Name = "Email", Prompt = "Enter your Email")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression(
        @"^(?=.{1,254}$)(?=.{1,64}@)[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:(?:[A-Za-z0-9](?:[A-Za-z0-9-]{0,61}[A-Za-z0-9])?\.)+[A-Za-z]{2,63}|\[(?:\d{1,3}\.){3}\d{1,3}\])$",
        ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [Display(Name = "Password", Prompt = "Enter your Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Required")]
    [Display(Name = "Password", Prompt = "Confirm your Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = null!;

}
