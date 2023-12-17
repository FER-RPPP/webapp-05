using NLog.Web;
using NLog;
using RPPP_WebApp;
using Microsoft.EntityFrameworkCore;
using RPPP_WebApp.Models;

//NOTE: Add dependencies/services in StartupExtensions.cs and keep this file as-is

var builder = WebApplication.CreateBuilder(args);
var logger = LogManager.Setup().GetCurrentClassLogger();


builder.Services.AddDbContext<RPPP05Context>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

try
{
  logger.Debug("init main");
  builder.Host.UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = false });

  var app = builder.ConfigureServices().ConfigurePipeline();
  app.Run();
    app.UseStaticFiles();
}
catch (Exception exception)
{
  // NLog: catch setup errors
  logger.Error(exception, "Stopped program because of exception");
  throw;
}
finally
{
  // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
  NLog.LogManager.Shutdown();
}

public partial class Program { }