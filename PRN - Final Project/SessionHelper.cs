using System.Text.Json.Serialization;
using System.Text.Json;

namespace PRN___Final_Project
{
    public static class SessionHelper
    {
        public static void SetObjectAsJson(this ISession session, string key, object? value)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            session.SetString(key, JsonSerializer.Serialize(value, options));
        }

        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null)
            {
                return default(T);
            }

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            return JsonSerializer.Deserialize<T>(value, options);
        }
    }

    public class FlightData
    {
        public int OriginId { get; set; }
        public int DestinationId { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int TotalPassengers { get; set; }
        public bool IsOneWay { get; set; }
        public int OutboundFlightId { get; set; }
        public int? ReturnFlightId { get; set; }
        public decimal OutboundTotalPrice { get; set; }
        public decimal ReturnTotalPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int AdultNum { get; set; }
        public int ChildNum { get; set; }
        public int BabyNum { get; set; }
        public string ClassType { get; set; }
        public string? ReturnClassType { get; set; }
    }

    public class TicketData
    {
        public string SeatNumber { get; set; }
        public string TicketType { get; set; }
        public string CustomerName { get; set; }
        public decimal Carryluggage { get; set; }
        public decimal Baggage { get; set; }
        public string ClassType { get; set; }

        public int BookingId { get; set; }
    }
}
