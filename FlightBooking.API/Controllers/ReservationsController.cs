using FlightBooking.API.DTO;
using FlightBooking.DataAccess;
using FlightBooking.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlightBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly FlightBookingContext _context;
        private readonly IApplicationUser _user;

        public ReservationsController(FlightBookingContext context, IApplicationUser user)
        {
            _context = context;
            _user = user;
        }

        // GET: api/<ReservationsController>
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

        // GET api/<ReservationsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ReservationsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ReservationsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ReservationsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
