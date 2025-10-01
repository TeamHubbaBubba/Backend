using System.ComponentModel.DataAnnotations;

namespace Business.Dtos;

public class UserSignInDto
{
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
}
