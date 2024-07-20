using FlightBooking.API.DTO;
using FlightBooking.DataAccess;
using FlightBooking.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlightBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly FlightBookingContext _context;

        public FlightController(FlightBookingContext context)
        {
            _context = context;
        }

        // GET: api/<FlightController>
        [HttpGet]
        public async Task<IActionResult> GetAllFlights()
        {
            var flights = await _context.Flights.Include(r => r.Reservations).Include(c => c.DepartureCity).Include(x=> x.DestinationCity).ToListAsync();

            List<FlightsODto> resultList = new List<FlightsODto>();

            foreach (var flight in flights)
            {
                var numberOfReservedSeats = flight.Reservations.Where(r => r.FlightId == flight.Id).Sum(r => r.NumberOfSeats);

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
                    NumberOfAvailableSpots = flight.NumberOfSeats - numberOfReservedSeats,
                    Status = flight.Status
                };
                resultList.Add(result);
            }
            return Ok(resultList);
        }

        // GET api/<FlightController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FlightController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateFlightIDto input)
        {
            var flight = new Flight
            {
                DepartureCityId = input.DepartureCityId,
                DestinationCityId = input.DestinationCityId,
                DepartureDateTime = input.DepartureDateTime,
                ArrivalDateTime = input.ArrivalDateTime,
                NumberOfSeats = input.NumberOfSeats,
                NumberOfStops = input.NumberOfStops,
                Status = "Pending"
            };

            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT api/<FlightController>/5
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveFlight(int id)
        {
            var flight = await _context.Flights.FirstOrDefaultAsync(x => x.Id == id);

            if(flight == null)
            {
                throw new ArgumentException("That flight don't exist");
            }

            flight.Status = "Approved";

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectFlight(int id)
        {
            var flight = await _context.Flights.FirstOrDefaultAsync(x => x.Id == id);

            if (flight == null)
            {
                throw new ArgumentException("That flight don't exist");
            }

            flight.Status = "Rejected";

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<FlightController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
