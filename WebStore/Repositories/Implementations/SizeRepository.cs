using Microsoft.EntityFrameworkCore;
using WebStore.Data;
using WebStore.Models;
using WebStore.Repositories.Interfaces;

namespace WebStore.Repositories.Implementations
{
    public class SizeRepository : ISizeRepository
    {
        private readonly WebStoreContext _context;

        public SizeRepository(WebStoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Size>> GetAllAsync()
        {
            return await _context.Sizes.ToListAsync();
        }

        public async Task<Size?> GetByIdAsync(int id)
        {
            return await _context.Sizes.FindAsync(id);
        }

        public async Task AddAsync(Size size)
        {
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Size size)
        {
            _context.Entry(size).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Sizes.FindAsync(id);
            if (entity != null)
            {
                _context.Sizes.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
