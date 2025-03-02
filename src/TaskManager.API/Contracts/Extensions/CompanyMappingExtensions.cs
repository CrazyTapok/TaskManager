using TaskManager.API.Contracts.Requests;
using TaskManager.API.Contracts.Responses;
using TaskManager.Core.Models;

namespace TaskManager.API.Contracts.Extensions;

public static class CompanyMappingExtensions
{
    public static CompanyResponse MapToCompanyResponse(this Company company)
    {
        return new CompanyResponse(company.Id, company.Title);
    }

    public static Company MapToCompany(this CompanyRequest request, Guid? id = null)
    {
        return new Company { Id = id ?? Guid.NewGuid(), Title = request.Title };
    }
}
