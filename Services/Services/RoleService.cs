using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using Repositories.Interface;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<RoleManager<User>>> GetAllRolesAsync()
        {
            try
            {
                return await _unitOfWork.Repository<RoleManager<User>>().GetAllAsync();
            }
            catch
            {
                throw new Exception("An error occurred while retrieving users.");
            }
        }
    }
}
