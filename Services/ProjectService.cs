using System.Text.Json;
using hishenperera_portfolio.Models;

namespace hishenperera_portfolio.Services;

public class ProjectService
{
    private readonly string _filePath;

    public ProjectService(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "Data", "projects.json");
    }

    private List<Project> ReadProjects()
    {
        if (!File.Exists(_filePath))
            return new List<Project>();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Project>>(json) ?? new List<Project>();
    }

    private void SaveProjects(List<Project> projects)
    {
        var json = JsonSerializer.Serialize(projects, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_filePath, json);
    }

    public List<Project> GetAll() => ReadProjects();

    public Project? GetById(int id) =>
        ReadProjects().FirstOrDefault(p => p.ProjectId == id);

    public void Add(Project project)
    {
        var projects = ReadProjects();
        project.ProjectId = projects.Any() ? projects.Max(p => p.ProjectId) + 1 : 1;
        projects.Add(project);
        SaveProjects(projects);
    }

    public void Update(Project project)
    {
        var projects = ReadProjects();
        var existing = projects.FirstOrDefault(p => p.ProjectId == project.ProjectId);

        if (existing == null) return;

        existing.ProjectName = project.ProjectName;
        existing.ProjectImg = project.ProjectImg;
        existing.ProjectLink = project.ProjectLink;

        SaveProjects(projects);
    }

    public void Delete(int id)
    {
        var projects = ReadProjects();
        projects.RemoveAll(p => p.ProjectId == id);
        SaveProjects(projects);
    }
}
