using BussinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using System;

namespace PRN___Final_Project.Pages
{
    public class PaymentModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        private readonly IBookingService _bookingService;
        public PaymentModel(IPaymentService paymentService, IBookingService bookingService)
        {
            _paymentService = paymentService;
            _bookingService = bookingService;
        }

        [BindProperty]
        public int BookingId { get; set; }

        public IActionResult OnGet(int? bookingId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["BookingId"] = bookingId;
                return RedirectToPage("/Area/Identity/Pages/Account/Login", new { returnUrl = Url.Page("/Payment", new { bookingId }) });
            }
            if (TempData.ContainsKey("BookingId"))
            {
                BookingId = Convert.ToInt32(TempData["BookingId"]);
            }
            else
            {
                BookingId = bookingId.Value;
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["BookingId"] = BookingId;
                return RedirectToPage("/Area/Identity/Pages/Account/Login", new { returnUrl = Url.Page("/Payment", new { bookingId = BookingId }) });
            }

            var booking = await _bookingService.GetBookingByIdAsync(BookingId);
            if (booking == null)
            {
                ModelState.AddModelError(string.Empty, "Không tìm th?y thông tin Booking v?i mã này.");
                return Page();
            }

            string paymentUrl = _paymentService.CreatePaymentUrl(HttpContext, booking);

            return Redirect(paymentUrl);

        }

    }
}
