﻿using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Domain.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FlightId { get; set; }
        public string Status { get; set; }
        public int NumberOfSeats { get; set; }

        public User User { get; set; }
        public Flight Flight { get; set; }
    }
}
