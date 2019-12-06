using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MBCEventDonation.Data;
using MBCEventDonation.Repository.Azure;
using MBCEventDonation.Repository.EventDonation;
using System.Configuration;
using Microsoft.Extensions.Options;
using System.IO;
using MBCEventExpense.Repository.EventExpenses;
using Microsoft.AspNetCore.Blazor.Services;
using Microsoft.AspNetCore.Http.Extensions;
using MBCEventDonation.Repository.Toast;
using Blazor.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Serilog;
using Blazor.Extensions.Storage;
using BlazorContextMenu;
using Blazor.FileReader;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http.Connections;
using Blazored.Modal;
using BlazorStrap;

//using MBCEventDonation.Repository.PrayerTimes;

namespace MBCEventDonation
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //string dbConn = Configuration.GetSection("MyConfig").GetSection("DefaultConnection").Value;
            //string tablename = Configuration.GetSection("MyConfig").GetSection("TableName").Value;

            // string dbConn = Configuration.GetSection("azureconnection").Value;
            // string azuretablename = Configuration.GetSection("azureconnection").Value;

            var configBuilder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: true);
            var config = configBuilder.Build();

            services.Configure<MyConfig>(config);

            services.AddScoped<IAzureTableStorage<EventDonationModel>>(factory =>
            {
                return new AzureTableStorage<EventDonationModel>(new AzureTableSettings(connectionString: "DefaultEndpointsProtocol=https;AccountName="secretkey1"; AccountKey=Vs6lSzauuCOL3J81QMfwf9v+"secretkey"/VPAu9jUFg==;EndpointSuffix=core.windows.net", tableName: "EventDonation"));
            });
            services.AddScoped<IAzureTableStorage<EventExpenseModel>>(factory =>
            {
                return new AzureTableStorage<EventExpenseModel>(new AzureTableSettings(connectionString: "DefaultEndpointsProtocol=https;AccountName="secretkey1"; AccountKey=Vs6lSzauuCOL3J81QMfwf9v+"secretkey"/VPAu9jUFg==;EndpointSuffix=core.windows.net", tableName: "EventExpense"));
            });
            //   services.AddMvc();
           

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredModal();

            services.AddBootstrapCSS();

            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
            services.AddFileReaderService(options => options.InitializeOnFirstCall = true);

            services.AddScoped<WeatherService>();
            services.AddSignalR(o =>
            {
                //https://docs.microsoft.com/en-us/aspnet/core/signalr/configuration?view=aspnetcore-3.0&tabs=dotnet#configure-server-options
                o.MaximumReceiveMessageSize = 1844674407;
                o.EnableDetailedErrors = true;
                o.KeepAliveInterval = TimeSpan.FromMinutes(10);
            });

            services.AddScoped<MBCEventDonation.Repository.EventDonation.EventDonationService>();
            services.AddTransient<MBCEventExpense.Repository.EventExpenses.EventExpenseService>();
            services.AddScoped<ToastService>();
            // Both SessionStorage and LocalStorage are registered
            //services.AddStorage();
            services.AddProtectedBrowserStorage();
            //services.AddScoped<SessionStorage>();
            //services.AddScoped<LocalStorage>();
            services.AddBlazorContextMenu(options =>
            {
                options.ConfigureTemplate("dark", template =>
                {
                    template.MenuCssClass = "dark-menu";
                    template.MenuItemCssClass = "dark-menu-item";
                    template.MenuItemDisabledCssClass = "dark-menu-item--disabled";
                    template.MenuItemWithSubMenuCssClass = "dark-menu-item--with-submenu";
                    template.Animation = Animation.FadeIn;
                });
            });

           


        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

      

        app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
