using Kolos1Poprawa.Models;

namespace Kolos1Poprawa.Services;

public interface IClientService
{
    Task<ClientDto?> GetClient(int clientId);
    Task<int> CreateClient(CreateClientDto request);
}