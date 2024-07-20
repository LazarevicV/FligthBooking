using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Domain.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }

        public ICollection<Role>? Roles { get; set;}
        public ICollection<Reservation>? Reservations { get; set;}

    }
}
