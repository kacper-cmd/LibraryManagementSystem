using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.RequestModel;
using Infrastructure.Entities;
using System.Linq.Expressions;
using Application.Mapper;
using Infrastructure.Exceptions;
using static System.Reflection.Metadata.BlobBuilder;
using Infrastructure.Constants;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository repository;

        public UserService(IUserRepository repository)
        {
            this.repository = repository;
        }
        public PagedListDTO<UserDTO> GetPagedListAsync(BaseFilter filter)
        {
            IQueryable<User> query = repository.GetUsersAsQueryable();

			if (!string.IsNullOrEmpty(filter.SearchColumn) && !string.IsNullOrEmpty(filter.SearchTerm))
			{
				switch (filter.SearchColumn)
				{
					case nameof(UserDTO.Email): 
						query = query.Where(b => b.Email.Contains(filter.SearchTerm));
						break;
                    case nameof(UserDTO.Name):
                        query = query.Where(b => b.Name.Contains(filter.SearchTerm));
						break;
					case nameof(UserDTO.Role):
						query = query.Where(b => b.Role.Contains(filter.SearchTerm));
						break;
				}
			}
			else if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
			{
				query = query.Where(x => x.Name.Contains(filter.SearchTerm) || x.Email.Contains(filter.SearchTerm));
			}

			if (filter.SortOrder?.ToLower() == Constants.desc)
            {
                query = query.OrderByDescending(GetSortProperty(filter.SortColumn!));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(filter.SortColumn!));
            }

            var userResponsesQuery = query.Select(x => new UserDTO(x.ID, x.Name, x.Email, x.Role));
            var users = PageList<UserDTO>.Create(userResponsesQuery, filter.Page, filter.PageSize);
			return new PagedListDTO<UserDTO>()
			{
				Items = users.Items,
				Page = users.Page,
				PageSize = users.PageSize,
				TotalCount = users.TotalCount,
			};
        }


        private static Expression<Func<User, object>> GetSortProperty(string sortColumn)
        {
            return sortColumn switch
            {
                nameof(UserDTO.Name) => item => item.Name,
                nameof(UserDTO.Email) => item => item.Email,
                nameof(UserDTO.Role) => item => item.Role,
                _ => item => item.ID
            };
        }


        public async ValueTask<IEnumerable<UserDTO>> GetUsers()
        {
            var user = await repository.GetUsers();
            return user.MapToDTO();
        }

        public async ValueTask<UserDTO> GetUser(Guid userId)
        {
            var user = await repository.GetUser(userId);
            if (user is null)
                throw new NotFoundException("The requested resource was not found.");
            return user.MapToDTO();
        }

        public async ValueTask<UserDTO> AddUser(UserDTO dto)
        {
            var entity = dto.MapToDomainModel();
            var user = await repository.AddUser(entity);
            return user.MapToDTO();
        }

        public async ValueTask<UserDTO> UpdateUser(UserDTO dto)
        {
            var entity = dto.MapToDomainModel();
            var user = await repository.UpdateUser(entity);
            return user.MapToDTO();
        }

        public async ValueTask DeleteUser(Guid userId)
        {
            await repository.DeleteUser(userId);
        }
    }
}