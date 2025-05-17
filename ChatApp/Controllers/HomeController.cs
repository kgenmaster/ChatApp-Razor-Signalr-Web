using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ACE_MVC.Models;

namespace ACE_MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger <HomeController> _logger;

    public HomeController (ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult App()
    {
        string username = HttpContext.Session.GetString("LoggedInUsername");
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        ViewBag.Username = username;
        return View();
    }


    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
