using PetOS.Models;

namespace PetOS.Repositories.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> FindByEmailAsync(string email);

    Task<bool> EmailExistsAsync(string email);

    Task AddAsync(AppUser user);
}