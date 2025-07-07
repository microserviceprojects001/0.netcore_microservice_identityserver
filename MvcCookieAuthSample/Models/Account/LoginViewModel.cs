namespace MvcCookieAuthSample.Models.Account;

public class LoginViewModel
{
    public string ReturnUrl { get; set; }
    public LoginInputModel Input { get; set; } // 绑定表单数据
}