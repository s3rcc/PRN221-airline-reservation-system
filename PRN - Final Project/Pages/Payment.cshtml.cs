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

        public async Task<IActionResult> OnGet(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["BookingId"] = id;
                return Redirect($"/Login?returnUrl=/Payment?bookingId={id}");
            }

            if (TempData.ContainsKey("BookingId"))
            {
                BookingId = Convert.ToInt32(TempData["BookingId"]);
            }
            else if (id.HasValue)
            {
                BookingId = id.Value;
            }
            else
            {
                return RedirectToPage("/Errors/404");

            }
            var booking = await _bookingService.GetBookingByIdAsync(BookingId);
            if (booking == null)
            {
                ModelState.AddModelError(string.Empty, "Kh�ng t�m th?y th�ng tin Booking v?i m� n�y.");
                return Page();
            }

            string paymentUrl = _paymentService.CreatePaymentUrl(HttpContext, booking);

            return Redirect(paymentUrl);

        }


    }
}
