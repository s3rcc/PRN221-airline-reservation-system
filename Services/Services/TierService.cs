﻿using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using Repositories.Interface;
using Services.Interfaces;

namespace Services.Services
{
    public class TierService : ITierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public TierService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task AddTierAsync(Tier tier)
        {
            try
            {
                if (tier == null) throw new ArgumentNullException(nameof(tier));

                await _unitOfWork.Repository<Tier>().AddAsync(tier);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while adding the tier.");
            }
        }

        public async Task DeleteTierAsync(int id)
        {
            try
            {
                var tier = await _unitOfWork.Repository<Tier>().GetByIdAsync(id)
                           ?? throw new KeyNotFoundException("Tier not found.");
                _unitOfWork.Repository<Tier>().DeleteAsync(tier);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while deleting the tier.");
            }
        }


        public async Task<IEnumerable<Tier>> GetAllTiersAsync()
        {
            try
            {
                return await _unitOfWork.Repository<Tier>().GetAllAsync();
            }
            catch
            {
                throw new Exception("An error occurred while retrieving tiers.");
            }
        }

        public async Task<Tier> GetTierByIdAsync(int id)
        {
            try
            {
                var tier = await _unitOfWork.Repository<Tier>().GetByIdAsync(id)
                           ?? throw new KeyNotFoundException("Tier not found.");
                return tier;
            }
            catch
            {
                throw new Exception("An error occurred while retrieving the tier.");
            }
        }

        public async Task UpdateTierAsync(Tier tier)
        {
            try
            {
                if (tier == null) throw new ArgumentNullException(nameof(tier));

                await _unitOfWork.Repository<Tier>().UpdateAsync(tier);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while updating the tier.");
            }
        }

        public async Task<Tier> GetTierByUserIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId)
                           ?? throw new KeyNotFoundException("User not found.");

                var tier = await _unitOfWork.Repository<Tier>().GetByIdAsync(user.TierId)
                           ?? throw new KeyNotFoundException("Tier not found for this user.");

                return tier;
            }
            catch
            {
                throw new Exception("An error occurred while retrieving the tier.");
            }
        }
    }
}
