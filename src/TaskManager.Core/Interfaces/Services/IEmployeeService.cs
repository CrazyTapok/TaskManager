﻿using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces.Services;

internal interface IEmployeeService : IService<Employee>
{
    Task<List<Employee>> GetEmployeesByProjectIdAsync(Guid projectId, CancellationToken cancellationToken);

    Task<List<Employee>> GetEmployeesByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken);
}
