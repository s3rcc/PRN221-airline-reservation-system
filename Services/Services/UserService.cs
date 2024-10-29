using BussinessObjects;
using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Http;
using Repositories.Interface;
using Services.Interfaces;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task AddUserAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                await _unitOfWork.Repository<User>().AddAsync(user);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while adding user.");
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Repository<User>().GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found.");
                _unitOfWork.Repository<User>().DeleteAsync(user);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while deleting the user.");
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _unitOfWork.Repository<User>().GetAllAsync();
            }
            catch
            {
                throw new Exception("An error occurred while retrieving users.");
            }
        }

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetAllUsersPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var allUsers = await _unitOfWork.Repository<User>().GetAllAsync();
                var pagedUsers = allUsers
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                int totalCount = allUsers.Count();

                return (pagedUsers, totalCount);
            }
            catch
            {
                throw new Exception("An error occurred while retrieving users.");
            }
        }

		public async Task<int> GetTotalUser()
		{
			try
			{
				var user = await _unitOfWork.Repository<User>().GetAllAsync();
				return user.Count();
			}
			catch
			{
				throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting total user");
			}
		}

		public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _unitOfWork.Repository<User>().GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found.");
                return user;
            }
            catch
            {
                throw new Exception("An error occurred while retrieving the user.");
            }
        }

        public async Task UpdateUserAsync(User user)
        {

            try
            {
                if (user == null) throw new ArgumentNullException(nameof(user));

                await _unitOfWork.Repository<User>().UpdateAsync(user);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while updating the user.");
            };
        }
    }
}
