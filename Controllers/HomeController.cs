using Microsoft.AspNetCore.Mvc;
using hishenperera_portfolio.Services;

namespace hishenperera_portfolio.Controllers;

public class HomeController : Controller
{
    private readonly ProjectService _projectService;

    public HomeController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    public IActionResult Index()
    {
        var projects = _projectService.GetAll();
        return View(projects);
    }
}
