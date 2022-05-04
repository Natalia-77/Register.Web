
using Serilog;

namespace Register.Web.Helper
{
    public static class LoggerInstance
    {
        public static void LoggerMessage(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {

                var path = Path.Combine(System.Environment.CurrentDirectory, "Logs");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //var fileLog = Path.Combine(path, "log-{Date}.txt");
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();               
               
                //Дані для логів:шлях до папки,куди записати,темплейт для повідомлення,дефолтний рівень і т.д.
                //все зчитую з джейсона
                IConfiguration configSerilog = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
               .Build();

               var log = new LoggerConfiguration()
                     .ReadFrom.Configuration(configSerilog)
                     .CreateLogger();

                log.Information("Initializing Serilog....");
                //назва секції,з якої потрібно обтрати дані для подальшого логування,запису логів і т.д...
                loggerFactory.AddFile(configSerilog.GetSection("Serilog"));






            }
        }
    }
}
