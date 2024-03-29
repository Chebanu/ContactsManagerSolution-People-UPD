using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepositoryContracts;
using Repositories;
using Serilog;
using CRUD.Filters.ActionFilters;
using CRUD;
using CRUD.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();



if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}

app.UseHsts();
app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseHttpLogging();


if (builder.Environment.IsEnvironment("Test") == false)
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}
app.UseStaticFiles();

app.UseRouting(); //Identifying action method base route
app.UseAuthentication(); //Reading Identity Cookie

app.UseAuthorization(); //Validates access permissions of the user

app.MapControllers(); // Execute the filter pipeline(action+filters)

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
            name: "areas",
            pattern: "{area}/{controller=Home}/{action=Index}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{id?}"
        );
});

app.Run();

public partial class Program { } //make the auto-generated Program accessible programmatically
