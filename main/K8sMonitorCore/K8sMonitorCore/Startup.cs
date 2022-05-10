using EndpointPinger;
using K8sMonitorCore.Aggregation.Service;
using K8sMonitorCore.Settings;
using KubernetesSyncronizer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Threading;

namespace K8sMonitorCore;

public class Startup
{
    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
        if (Configuration.GetValue("Debug:DelayStartup", false)) {
            Console.WriteLine("Delaying startup by 60 seconds for debug reasons");
            Thread.Sleep(new TimeSpan(0, 0, 60));
            Console.WriteLine("Delay ended, starting app");
        }

        services.Configure<Security>(Configuration.GetSection("Security"));
        services.Configure<Gui>(Configuration.GetSection("Gui"));
        bool guiEnabled = Configuration.GetValue("Gui:Enabled", false);

        services.AddHttpClient();

        services.AddSingleton<PingerManager>();

        services.AddK8sClient();
        services.AddK8sListening();

        services.AddTransient<AggregationService>();

        services.Configure<Defaults>(
            Configuration.GetSection("Defaults"));

        services.AddControllers();
        services.AddSwaggerGen(c => {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "K8sMonitorCore", Version = "v1" });
        });
        if (guiEnabled) services.AddRazorPages();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<Gui> guiOptions, IOptions<Security> securityOptions) {
        var gui = guiOptions.Value;
        var security = securityOptions.Value;

        if (env.IsDevelopment()) {
            if (gui.Enabled) app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "K8sMonitorCore v1"));
        } else {
            if (gui.Enabled) app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            if (security.Hsts) app.UseHsts();
        }

        if (security.Https) app.UseHttpsRedirection();
        if (gui.Enabled) app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
            if (gui.Enabled) endpoints.MapRazorPages();
        });
    }
}
