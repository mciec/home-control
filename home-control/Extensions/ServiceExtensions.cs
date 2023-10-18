using home_control.Models;
using Microsoft.EntityFrameworkCore;

namespace home_control.Extensions;

public static class ServiceExtensions
{

    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<MyDbContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("MyConnection")));

}
