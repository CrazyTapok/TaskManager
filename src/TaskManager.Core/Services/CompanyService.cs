using TaskManager.Core.Infrastructure;
using TaskManager.Core.Interfaces.Data;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Core.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
    }

    public async Task<Company> GetByIdAsync(Guid id, CancellationToken cancellationToken) 
    { 
        var entity = await _companyRepository.GetByIdAsync(id, cancellationToken); 
       
        if (entity == null) 
            throw new ValidationException($"Entity with id {id} not found", ""); 
    
        return entity; 
    }

    public async Task<List<Company>> ListAllAsync(CancellationToken cancellationToken) 
    { 
        return await _companyRepository.GetAllAsync(cancellationToken); 
    }

    public async Task<Company> AddAsync(Company entity, CancellationToken cancellationToken) 
    { 
        return await _companyRepository.AddAsync(entity, cancellationToken); 
    }

    public async Task<bool> UpdateAsync(Company entity, CancellationToken cancellationToken) 
    { 
        return await _companyRepository.UpdateAsync(entity, cancellationToken); 
    }

    public async Task DeleteAsync(Company entity, CancellationToken cancellationToken) 
    { 
        await _companyRepository.DeleteAsync(entity.Id, cancellationToken); 
    }
}
