using Application.Interfaces.Repositories;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.Repository;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    public UserRepository(ApplicationDbContext appDbContext)
    {
		ArgumentNullException.ThrowIfNull(nameof(appDbContext));
		_dbContext = appDbContext;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User?> GetUser(Guid userId)
    {
        var users = await _dbContext.Users.Where(e => e.ID == userId)
            .FirstOrDefaultAsync(e => e.ID == userId);
        return users;
    }

    public async Task<User> AddUser(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Parameter cannot be null");

        user.DateCreated = DateTime.Now;
        var result = await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<User> UpdateUser(User updatedUser)
    {
        if (updatedUser == null)
            throw new ArgumentNullException(nameof(updatedUser), "Parameter cannot be null");

        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(e => e.ID == updatedUser.ID);

        if (existingUser == null)
            throw new ArgumentNullException(nameof(updatedUser), "Parameter cannot be null");

        existingUser.Email = updatedUser.Email;
        existingUser.Name = updatedUser.Name;
        existingUser.Role = updatedUser.Role;
        existingUser.DateUpdated = DateTime.Now;
        await _dbContext.SaveChangesAsync();

        return existingUser;
    }

    public async Task DeleteUser(Guid userId)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(e => e.ID == userId);

        if (existingUser != null)
        {
            _dbContext.Users.Remove(existingUser);
            await _dbContext.SaveChangesAsync();
        }
    }
    private IQueryable<User> GetQuerableNotTracking()
    {
        return _dbContext.Users.AsQueryable().AsNoTracking();
    }
    public IQueryable<User> GetUsersAsQueryable(string keyword = "")
    {
        IQueryable<User> query = GetQuerableNotTracking();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(a => a.Name.Contains(keyword));
        }

        return query.OrderByDescending(a => a.DateUpdated ?? a.DateCreated);
    }

    public IQueryable<User> GetUsersAsQueryableWithFilter(string keyword = "", int page = 1, int pageSize = 10)
    {
		IQueryable<User> query = GetQuerableNotTracking();

		if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(a => a.Name.Contains(keyword));
        }

        query = query.Skip((page - 1) * pageSize).Take(pageSize);

        return query.OrderByDescending(a => a.DateUpdated ?? a.DateCreated);
    }
}
