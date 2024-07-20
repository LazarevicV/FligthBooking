using FlightBooking.API.Core;
using FlightBooking.API.Extensions;
using FlightBooking.API.Hub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()  // Allow any origin
                       .AllowAnyHeader()  // Allow any header
                       .AllowAnyMethod(); // Allow any method
            });
    });

//Konfiguracija DIC
var settings = new AppSettings();
builder.Configuration.Bind(settings);
builder.Services.AddSingleton(settings);

builder.Services.AddApplicationUser();
builder.Services.AddTransient<ITokenStorage, InMemoryTokenStorage>();
builder.Services.AddJwt(settings);

builder.Services.AddFlightBookingDbContex();
builder.Services.AddHttpContextAccessor();



var app = builder.Build();

app.UseCors("AllowAll");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHub<LiveUpdateHub>("/liveUpdateHub");

app.UseAuthorization();

app.MapControllers();

app.Run();
