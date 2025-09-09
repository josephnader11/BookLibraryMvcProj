using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookLibraryMvcProj
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ? Force Development environment for debugging
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Read API base URL from configuration
            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

            // Register HttpClient with named client "ApiClient"
            builder.Services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            var app = builder.Build();

            // ? Debug log to confirm environment
            Console.WriteLine($"?? Current Environment: {app.Environment.EnvironmentName}");

            if (app.Environment.IsDevelopment())
            {
                // Show detailed errors
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Default MVC route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
