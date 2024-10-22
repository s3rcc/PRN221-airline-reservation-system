using BussinessObjects;
using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Http;
using Repositories.Interface;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Payment>> GetAllPayments()
        {
            try
            {
                return await _unitOfWork.Repository<Payment>().GetAllAsync();
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting payment.");
            }
        }

        public async Task<Payment> GetPaymentByUserId(string userId)
        {
            try
            {
                return await _unitOfWork.Repository<Payment>().FirstOrDefaultAsync(x => x.UserId == userId);
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting payment.");
            }
        }

        public async Task<IEnumerable<Payment>> GetPayments(int year)
        {
            try
            {
                return await _unitOfWork.Repository<Payment>().FindAsync(x => x.PaymentDate.Year == year);
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting payment.");
            }
        }

        public async Task<IEnumerable<Payment>> GetPayments(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _unitOfWork.Repository<Payment>().FindAsync(x => x.PaymentDate >= startDate.Date && x.PaymentDate <= endDate.Date);
            }
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error getting payments.");
            }
        }

		public async Task<decimal> GetRevenue()
		{
            try
            {
                var payment = await _unitOfWork.Repository<Payment>().GetAllAsync();
				decimal totalRevenue = payment.Sum(p => p.Amount);
                return totalRevenue;
			}
            catch
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError, ErrorCode.INTERNAL_SERVER_ERROR, "Error revenue.");
			}
		}
	}
}
