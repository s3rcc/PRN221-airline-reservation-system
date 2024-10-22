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
    }
}
