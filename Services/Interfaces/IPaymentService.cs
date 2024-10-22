using BussinessObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPaymentService
    {

        string CreatePaymentUrl(HttpContext context, Booking booking);
        Task ExecutePayment(IQueryCollection queryParameters);
        Task UpdatePaymentAsync(Payment payment);
        Task CreatePaymentAsync(Payment payment);
        Task<Payment> GetPaymentByUserId(string userId);
        Task<IEnumerable<Payment>> GetAllPayments();
        Task<IEnumerable<Payment>> GetPayments(int year);
        Task<IEnumerable<Payment>> GetPayments(DateTime startDate, DateTime endDate);
        Task<decimal> GetRevenue();
    }
}
