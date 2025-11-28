using Microsoft.EntityFrameworkCore;
using WebStore.Data;
using WebStore.Models;
using WebStore.Repositories.Interfaces;

namespace WebStore.Repositories.Implementations
{
    public class GenderRepository : IGenderRepository
    {
        private readonly WebStoreContext _context;

        public GenderRepository(WebStoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Gender>> GetAllAsync()
        {
            return await _context.Genders.ToListAsync();
        }

        public async Task<Gender?> GetByIdAsync(int id)
        {
            return await _context.Genders.FindAsync(id);
        }

        public async Task AddAsync(Gender gender)
        {
            await _context.Genders.AddAsync(gender);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Gender gender)
        {
            _context.Entry(gender).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Genders.FindAsync(id);
            if (entity != null)
            {
                _context.Genders.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
