using Infrastructure.Entities;
namespace Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User?> GetUser(Guid userId);
        Task<User> AddUser(User user);
        Task<User> UpdateUser(User user);
        Task DeleteUser(Guid userId);
        IQueryable<User> GetUsersAsQueryable(string keyword = "");
        IQueryable<User> GetUsersAsQueryableWithFilter(string keyword = "", int page = 1, int pageSize = 10);

    }
}
