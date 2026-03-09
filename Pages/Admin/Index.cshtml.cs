using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SMVTelecom.Services;

namespace SMVTelecom.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly AdminAuthService _auth;
    [BindProperty] public string Email    { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    public string? Error { get; set; }

    public IndexModel(AdminAuthService auth) => _auth = auth;

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("admin") is not null)
            return RedirectToPage("/Admin/Painel");
        return Page();
    }

    public IActionResult OnPost()
    {
        if (_auth.Authenticate(Email, Password))
        {
            HttpContext.Session.SetString("admin", Email);
            return RedirectToPage("/Admin/Painel");
        }
        Error = "E-mail ou senha incorretos.";
        return Page();
    }
}
