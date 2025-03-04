using FluentValidation.AspNetCore;
using FluentValidation;
using TaskManager.API.Contracts.Validators;

namespace TaskManager.API.Contracts.Extensions;

public static class ValidationServiceExtensions
{
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        // Configuring FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        // Registration of all validators
        services.AddValidatorsFromAssemblyContaining<CompanyRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<EmployeeRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<ProjectRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<TaskRequestValidator>();

        return services;
    }
}
