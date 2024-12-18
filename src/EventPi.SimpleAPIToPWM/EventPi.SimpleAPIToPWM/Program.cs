
using EventPi.SimpleAPIToPWM.Controllers;

namespace EventPi.SimpleAPIToPWM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<PinsController>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.Urls.Add("http://0.0.0.0:8080");
            //app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
