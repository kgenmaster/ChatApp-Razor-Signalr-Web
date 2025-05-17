using Microsoft.AspNetCore.Mvc;
using ACE_MVC.Models;
using ACE_MVC.DataService;
using Microsoft.Extensions.Configuration;

namespace ACE_MVC.Controllers;

public class AccountController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;

    public AccountController(IConfiguration configuration, UserService userService)
    {
        _configuration = configuration;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password, bool rememberMe)
    {
        var validationResult = _userService.ValidateLogin(email, password);
        var user = _userService.GetUserByEmail (email);
        

        if (validationResult)
        {
            TempData["Success"] = "Login successful!";
            HttpContext.Session.SetString("LoggedInUsername", user);

            return RedirectToAction("App", "Home");
        }
        else
        {
            TempData["Error"] = "Invalid email or password";
            return View();
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool isSuccess = _userService.CreateUser(model.Username, model.Email, model.Password);

        if (isSuccess)
        {
            TempData["Success"] = "Account created successfully!";
            return RedirectToAction("Login");
        }
        else
        {
            TempData["Error"] = "Error creating account. Please try again.";
            return View(model);
        }
    }
}