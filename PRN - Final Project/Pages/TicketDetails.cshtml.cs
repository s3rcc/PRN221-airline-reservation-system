using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using PuppeteerSharp;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using Org.BouncyCastle.Cms;

namespace PRN___Final_Project.Pages
{
    public class TicketDetailsModel : PageModel
    {
        private readonly ITicketService _ticketService;
        private readonly IEmailSender _emailSender;

        public TicketDetailsModel(ITicketService ticketService, IEmailSender emailSender)
        {
            _ticketService = ticketService;
            _emailSender = emailSender;
        }

        public IEnumerable<Ticket> Tickets { get; set; }
        public int BookingId { get; set; }

        public async Task OnGetAsync(int bookingId , bool isOutbound, int? ticketId)
        {
            BookingId = bookingId;
            
            if (!ticketId.HasValue)
            {
                Tickets = await _ticketService.GetTicketByBookingIdAndTypeAsync(bookingId, isOutbound);
            }
            else
            {
                int id = (int)ticketId;
                Tickets = (IEnumerable<Ticket>)await _ticketService.GetTicketByIdAsync(id);
            }
            //// Chuy?n ??i HTML th�nh ?nh cho t?ng v�
            //var imagePaths = new List<string>();
            //foreach (var ticket in Tickets)
            //{
            //    string htmlContent = GenerateHtmlForTicket(ticket); // T?o HTML cho t?ng v�
            //    var imagePath = await GenerateImageFromHtml(htmlContent);
            //    imagePaths.Add(imagePath);
            //}

            //// G?i email v?i c�c file h�nh ?nh ?�nh k�m
            //await _emailSender.SendEmailWithAttachmentsAsync(user.Email, "Your tickets are ready",
            //           "Please find your ticket(s) attached.", imagePaths);
        }

        //private string GenerateHtmlForTicket(Ticket ticket)
        //{
        //    // T?o HTML t? th�ng tin v� ?? truy?n v�o Puppeteer
        //    string html = "<html><body>";
        //    // Th�m n?i dung v� v�o HTML (gi?ng nh? b?n ?� l�m ? v�ng l?p foreach)
        //    html += $"<div><h2>Ticket for {ticket.Booking.Flight.FlightNumber}</h2>";
        //    html += $"<p>Passenger: {ticket.Booking.User.UserName}</p>";
        //    html += $"<p>Seat: {ticket.SeatNumber}</p>";
        //    html += "</div></body></html>";
        //    return html;
        //}

        //private async Task<string> GenerateImageFromHtml(string html)
        //{
        //    await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        //    var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        //    var page = await browser.NewPageAsync();
        //    await page.SetContentAsync(html);
        //    var imagePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
        //    await page.ScreenshotAsync(imagePath);
        //    await browser.CloseAsync();
        //    return imagePath;
        //}
    }
}
