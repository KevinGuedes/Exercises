using Microsoft.AspNetCore.Mvc;

namespace Questao5.Presentation.Controllers;

public class AccountController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
