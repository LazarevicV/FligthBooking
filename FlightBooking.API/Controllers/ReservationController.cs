using FlightBooking.API.DTO;
using FlightBooking.API.Hub;
using FlightBooking.DataAccess;
using FlightBooking.Domain;
using FlightBooking.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlightBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly FlightBookingContext _context;
        private readonly IApplicationUser _user;
        private readonly IHubContext<LiveUpdateHub, ILiveUpdateHub> _hubContext;

        public ReservationController(FlightBookingContext context, IApplicationUser user, IHubContext<LiveUpdateHub, ILiveUpdateHub> hubContext)
        {
            _context = context;
            _user = user;
            _hubContext = hubContext;
        }

        // GET: api/<ReservationController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var reservartions = await _context.Reservations
                .Include(x => x.Flight)
                .Include(x => x.Flight).ThenInclude(c => c.DepartureCity)
                .Include(x => x.Flight).ThenInclude(c => c.DestinationCity)
                .Include(x => x.User)
                .Where(x => x.UserId == _user.Id)
                .ToListAsync();

            return Ok(reservartions.Select(x => new ReservationODto
            {
                Id = x.Id,
                DepartureCity = x.Flight.DepartureCity.Name,
                DestinationCity = x.Flight.DestinationCity.Name,
                FullName = x.User.FirstName + ' ' + x.User.LastName,
                NumberOfSeats = x.NumberOfSeats,
                Status = x.Status,
                DepartureDateTime = x.Flight.DepartureDateTime,
                DestinationDateTime = x.Flight.ArrivalDateTime
            }));
        }

        // GET: api/<ReservationsController>
        [HttpGet]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservartions = await _context.Reservations
                .Include(x => x.Flight)
                .Include(x => x.Flight).ThenInclude(c => c.DepartureCity)
                .Include(x => x.Flight).ThenInclude(c => c.DestinationCity)
                .Include(x => x.User)
                .ToListAsync();

            return Ok(reservartions.Select(x => new ReservationODto
            {
                Id = x.Id,
                DepartureCity = x.Flight.DepartureCity.Name,
                DestinationCity = x.Flight.DestinationCity.Name,
                FullName = x.User.FirstName + ' ' + x.User.LastName,
                NumberOfSeats = x.NumberOfSeats,
                Status = x.Status,
                DepartureDateTime = x.Flight.DepartureDateTime,
                DestinationDateTime = x.Flight.ArrivalDateTime
            }));
        }

        // GET api/<ReservationsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ReservationsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReservationIDto dto)
        {
            var flight = await _context.Flights.Include(r => r.Reservations)
              .Include(c => c.DepartureCity)
              .Include(x => x.DestinationCity)
              .Where(x => x.Id == dto.FlightId)
              .FirstOrDefaultAsync();

            if(flight.DepartureDateTime.Date <= DateTime.Now.AddDays(3).Date)
            {
                throw new ArgumentException("You can't reserve flight three days before!");
            }

            var reservation = new Reservation
            {
                UserId = _user.Id,
                FlightId = dto.FlightId,
                NumberOfSeats = dto.NumberOfSeats,
                Status = "Pending"
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var result = new FlightsODto
            {
                Id = flight.Id,
                DepartureCityId = flight.DepartureCityId,
                DestinationCityId = flight.DestinationCityId,
                DepartureCity = flight.DepartureCity?.Name,
                DestinationCity = flight.DestinationCity?.Name,
                DepartureDateTime = flight.DepartureDateTime,
                ArrivalDateTime = flight.ArrivalDateTime,
                NumberOfSeats = flight.NumberOfSeats,
                NumberOfStops = flight.NumberOfStops,
                NumberOfAvailableSpots = flight.NumberOfSeats - flight.Reservations.Where(r => r.FlightId == flight.Id).Sum(r => r.NumberOfSeats),
                Status = flight.Status
            };


            await _hubContext.Clients.All.ReservationAdded(result);

            return Ok(result);
        }

        // PUT api/<FlightController>/5
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveReservation(int id)
        {
            var reservation = await _context.Reservations.FirstOrDefaultAsync(x => x.Id == id);

            if (reservation == null)
            {
                throw new ArgumentException("That flight don't exist");
            }

            reservation.Status = "Approved";

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT api/<FlightController>/5
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectReservation(int id)
        {
            var reservation = await _context.Reservations.FirstOrDefaultAsync(x => x.Id == id);

            if (reservation == null)
            {
                throw new ArgumentException("That flight don't exist");
            }

            reservation.Status = "Rejected";

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<ReservationsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
