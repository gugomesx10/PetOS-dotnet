using Microsoft.EntityFrameworkCore;
using PetOS.Data;
using PetOS.Models;
using PetOS.Repositories.Interfaces;

namespace PetOS.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<AppUser?> FindByEmailAsync(string email)
    {
        return _context.Users
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .CountAsync(user => user.Email == email) > 0;
    }

    public async Task AddAsync(AppUser user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}