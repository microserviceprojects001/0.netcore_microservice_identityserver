namespace MvcCookieAuthSample.Models.Account;

public class RegisterViewModel
{
    public string ReturnUrl { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string ConfirmedPassword { get; set; }
}