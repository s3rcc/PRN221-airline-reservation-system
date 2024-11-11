using BussinessObjects.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace PRN___Final_Project.Pages
{
    public class PaymentCallbackModel : PageModel
    {
        private readonly IPaymentService _paymentService;

        public PaymentCallbackModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _paymentService.ExecutePayment(Request.Query);
                //if (TempData.ContainsKey("BookingId"))
                //{
                //    TempData.Remove("BookingId");
                //}
                return RedirectToPage("/PaymentResult", new { status = $"Your bookingId: {0}"+ TempData["BookingId"] });
            }
            catch (ErrorException ex)
            {
                return RedirectToPage("/PaymentResult", new { status = "failed", message = ex.Message });
            }
        }
    }
}
