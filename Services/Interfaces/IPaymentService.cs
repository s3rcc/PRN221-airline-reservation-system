using BussinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> GetPaymentByUserId(string userId);
        Task<IEnumerable<Payment>> GetAllPayments();
        Task<IEnumerable<Payment>> GetPayments(int year);
        Task<IEnumerable<Payment>> GetPayments(DateTime startDate, DateTime endDate);
        Task<decimal> GetRevenue();
    }
}
