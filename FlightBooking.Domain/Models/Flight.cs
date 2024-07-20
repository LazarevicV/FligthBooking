using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Domain.Models
{
    public class Flight
    {
        [Key]
        public int Id { get; set; }
        public int DepartureCityId { get; set; }
        public int DestinationCityId { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public int NumberOfSeats { get; set; }
        public int NumberOfStops { get; set; }
        public string Status { get; set; }

        public ICollection<City>? City { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }

    }
}
