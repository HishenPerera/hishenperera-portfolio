using Microsoft.AspNetCore.Mvc;
using hishenperera_portfolio.Models;
using hishenperera_portfolio.Services;

namespace hishenperera_portfolio.Controllers;

public class ProjectController : Controller
{
    private readonly ProjectService _service;

    public ProjectController(ProjectService service)
    {
        _service = service;
    }

    // LIST / MANAGE
    public IActionResult ManageProjects()
    {
        return View(_service.GetAll());
    }

    // ADD
    public IActionResult AddNewProject()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddNewProject(Project project)
    {
        if (!ModelState.IsValid)
            return View(project);

        _service.Add(project);
        return RedirectToAction(nameof(ManageProjects));
    }

    // EDIT
    public IActionResult EditProject(int id)
    {
        var project = _service.GetById(id);
        if (project == null) return NotFound();
        return View(project);
    }

    [HttpPost]
    public IActionResult EditProject(Project project)
    {
        _service.Update(project);
        return RedirectToAction(nameof(ManageProjects));
    }

    // DELETE
    public IActionResult DeleteProject(int id)
    {
        var project = _service.GetById(id);
        if (project == null) return NotFound();
        return View(project);
    }

    [HttpPost]
    public IActionResult DeleteProjectConfirmed(int projectId)
    {
        _service.Delete(projectId);
        return RedirectToAction(nameof(ManageProjects));
    }
}
