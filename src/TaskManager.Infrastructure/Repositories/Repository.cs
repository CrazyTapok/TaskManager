using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.EF;

namespace TaskManager.Infrastructure.Repositories
{
    public class Repository<TEntity, TModels> : IRepository<TEntity, TModels> where TEntity : class where TModels : class
    {
        protected readonly DBContext _context;
        private readonly IMapper _mapper;

        public Repository(DBContext context, IMapper mapper) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
        }

        public async Task<IEnumerable<TModels>> GetAllAsync()
        {
            var entities = await _context.Set<TEntity>().ToListAsync();
           
            return _mapper.Map<IEnumerable<TModels>>(entities);
        }

        public async Task<TModels> GetByIdAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
           
            return _mapper.Map<TModels>(entity);
        }

        public async Task AddAsync(TModels dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _context.Set<TEntity>().Add(entity);
          
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TModels dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _context.Entry(entity).State = EntityState.Modified;
           
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
                
                await _context.SaveChangesAsync();
            }
        }
    }
}
