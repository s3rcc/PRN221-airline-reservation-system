using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;  
using BussinessObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN___Final_Project.Pages.CRUD.PaymentManager
{
    public class PaymentManageModel : PageModel
    {
        private readonly IPaymentService _paymentService;  

        public PaymentManageModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
            Payments = new List<Payment>();
            Payment = new Payment();
        }

        [BindProperty]
        public Payment Payment { get; set; }
        public IEnumerable<Payment> Payments { get; set; }

        public async Task OnGetAsync()
        {
            Payments = await _paymentService.GetAllPaymentsAsync();  
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await Console.Out.WriteLineAsync("\n\n\n");
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Payment.PaymentId == 0)
            {
                await _paymentService.AddPaymentAsync(Payment);  
            }
            else
            {
                await Console.Out.WriteLineAsync("\n\n\n On Update id:" + Payment.PaymentId);
                await _paymentService.UpdatePaymentAsync(Payment);  
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _paymentService.DeletePaymentAsync(id);  
            return RedirectToPage();
        }
    }
}
