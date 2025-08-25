
using ACTDataService.Helpers;
using ACTDataService.Services;
using Newtonsoft.Json.Linq;
using Serilog;
using System;

namespace ACTDataService
{
    public class Program
    {


        public static JObject SettingsConfig { get; set; }
        public static string FileSettings { get; set; }
        public string ConnString { get; set; }
        public string serverAddress { get; set; }

        public static void Main(string[] args)
        {
            FileSettings = "Settings/Settings.json";
            SettingsConfig = JObject.Parse(System.IO.File.ReadAllText(FileSettings));
            string url = $"http://{SettingsConfig["ServerAddress"]?.ToString() ?? "localhost"}";

            // Configure Serilog to log to a file
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();


            //SQLDataAccessHelper sqlda = new SQLDataAccessHelper(FileSettings);

            try
            {
                Log.Information("Starting web host");



                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();
                builder.WebHost.UseUrls(url);


                // Add services to the container.

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // Add database context
                //builder.Services.AddDbContext<ApplicationDbContext>(options =>
                //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


                //Register Services
                builder.Services.AddScoped<IUserService>(provider =>
                    new UserService(FileSettings));

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                //app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
