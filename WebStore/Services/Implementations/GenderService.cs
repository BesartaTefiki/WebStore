using WebStore.Models;
using WebStore.Repositories.Interfaces;
using WebStore.Services.Interfaces;

namespace WebStore.Services
{
    public class GenderService : IGenderService
    {
        private readonly IGenderRepository _genderRepository;

        public GenderService(IGenderRepository genderRepository)
        {
            _genderRepository = genderRepository;
        }

        public async Task<IEnumerable<Gender>> GetAllAsync()
            => await _genderRepository.GetAllAsync();

        public async Task<Gender?> GetByIdAsync(int id)
            => await _genderRepository.GetByIdAsync(id);

        public async Task<Gender> CreateAsync(Gender gender)
        {
            await _genderRepository.AddAsync(gender);
            return gender;
        }

        public async Task UpdateAsync(Gender gender)
        {
            await _genderRepository.UpdateAsync(gender);
        }

        public async Task DeleteAsync(int id)
        {
            await _genderRepository.DeleteAsync(id);
        }
    }
}
