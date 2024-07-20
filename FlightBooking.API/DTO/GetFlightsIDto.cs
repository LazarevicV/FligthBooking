namespace FlightBooking.API.DTO
{
    public class GetFlightsIDto
    {
        public int DepartureCityId { get; set; }
        public int DestinationCityId { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime? ReturnDateTime { get; set;}
        public Boolean NoStops { get; set; } = false;
        public int NumberOfSeats { get; set; }
    }
}
