using BussinessObjects;
using Repositories.Interface;
using Services.Interfaces;
using System;
using System.Collections.Generic;
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

        public async Task AddPaymentAsync(Payment payment)
        {
            try
            {
                if (payment == null) throw new ArgumentNullException(nameof(payment));

                await _unitOfWork.Repository<Payment>().AddAsync(payment);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while adding the payment.");
            }
        }

        public async Task DeletePaymentAsync(int id)
        {
            try
            {
                var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(id)
                           ?? throw new KeyNotFoundException("Payment not found.");
                _unitOfWork.Repository<Payment>().DeleteAsync(payment);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while deleting the payment.");
            }
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            try
            {
                return await _unitOfWork.Repository<Payment>().GetAllAsync();
            }
            catch
            {
                throw new Exception("An error occurred while retrieving payments.");
            }
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            try
            {
                var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(id)
                           ?? throw new KeyNotFoundException("Payment not found.");
                return payment;
            }
            catch
            {
                throw new Exception("An error occurred while retrieving the payment.");
            }
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            try
            {
                if (payment == null) throw new ArgumentNullException(nameof(payment));

                await _unitOfWork.Repository<Payment>().UpdateAsync(payment);
                await _unitOfWork.SaveChangeAsync();
            }
            catch
            {
                throw new Exception("An error occurred while updating the payment.");
            }
        }
    }
}
