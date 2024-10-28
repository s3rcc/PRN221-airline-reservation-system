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
                // X? lý ph?n h?i t? VnPay
                await _paymentService.ExecutePayment(Request.Query);
                if (TempData.ContainsKey("BookingId"))
                {
                    TempData.Remove("BookingId");
                }
                return RedirectToPage("/PaymentResult", new { status = "success" });
            }
            catch (ErrorException ex)
            {
                // N?u thanh toán th?t b?i, chuy?n h??ng t?i trang k?t qu? th?t b?i
                return RedirectToPage("/PaymentResult", new { status = "failed", message = ex.Message });
            }
        }
    }
}
