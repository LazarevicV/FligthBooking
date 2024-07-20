using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Domain.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
