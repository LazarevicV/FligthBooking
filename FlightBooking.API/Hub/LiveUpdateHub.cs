using FlightBooking.API.DTO;
using Microsoft.AspNetCore.SignalR;

namespace FlightBooking.API.Hub
{
    public interface ILiveUpdateHub 
    {
        Task ReservationAdded(FlightsODto output);
    }
    public class LiveUpdateHub : Hub<ILiveUpdateHub>
    {
        public override Task OnConnectedAsync()
        {
            try
            {
                return base.OnConnectedAsync();
            }
            catch (Exception ex)
            {

                return base.OnDisconnectedAsync(ex);
            }
            
        }
    }
}
