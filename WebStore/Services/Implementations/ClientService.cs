using WebStore.Models;
using WebStore.Repositories.Interfaces;
using WebStore.Services.Interfaces;

namespace WebStore.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
            => await _clientRepository.GetAllAsync();

        public async Task<Client?> GetByIdAsync(int id)
            => await _clientRepository.GetByIdAsync(id);

        public async Task<Client> CreateAsync(Client client)
        {
            await _clientRepository.AddAsync(client);
            return client;
        }

        public async Task UpdateAsync(Client client)
        {
            await _clientRepository.UpdateAsync(client);
        }

        public async Task DeleteAsync(int id)
        {
            await _clientRepository.DeleteAsync(id);
        }
    }
}
