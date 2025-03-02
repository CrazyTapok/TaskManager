using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Models;

namespace TaskManager.API.Contracts.Extensions;

public static class ProjectMappingExtensions
{
    public static ProjectResponse MapToProjectResponse(this Project project)
    {
        return new ProjectResponse(project.Id, project.Title, project.ManagerId, project.CompanyId);
    }

    public static Project ToProject(this ProjectRequest request, Guid? id = null)
    {
        return new Project
        {
            Id = id ?? Guid.NewGuid(),
            Title = request.Title,
            ManagerId = request.ManagerId,
            CompanyId = request.CompanyId
        };
    }
}