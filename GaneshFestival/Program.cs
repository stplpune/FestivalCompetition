using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.OpenApi.Models;
using GaneshFestival.Repository;
using GaneshFestival.Repository.Interface;


var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder
        .AddDebug()
        .AddEventLog(new EventLogSettings()
        {
            SourceName = "eAuctionLogSource",
            LogName = "eAuctionErrorLog",
            Filter = (x, y) => y >= LogLevel.Error
        });
});

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(swagger =>
{
    // To Enable authorization using Swagger (JWT)  

});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});


//builder.Services.AddScoped<IJobPostAsyncRepository, JobPostAsyncRepository>();
//builder.Services.AddScoped<IAppliedMemberDetailsAsyncRepository, AppliedMemberDetailsAsyncRepository>();
//builder.Services.AddScoped<IUserDetailsAsyncRepository, UserDetailsAsyncRepository>();
//builder.Services.AddScoped<IMasterAsyncRepository, MasterAsyncRepository>();
//builder.Services.AddScoped<IContactUsAsyncRepository, ContactUsAsyncRepository>();
//builder.Services.AddScoped<IBlogDetailsAsyncRepository, BlogDetailsAsyncRepository>();
//builder.Services.AddScoped<IPageMasterAsyncRepository, PageMasterAsyncRepository>();
//builder.Services.AddScoped<IDashBoardAsyncRepository, DashBoardAsyncRepository>();
//builder.Services.AddScoped<IRequestDemoAsyncRepository, RequestDemoAsyncRepository>();
//builder.Services.AddScoped<IGallaryAsyncRepository, GallaryAsyncRepository>();
builder.Services.AddScoped<ICompetitionAsyncRepository, CompetitionAsyncRepository>();
builder.Services.AddScoped<ILoginAsyncRepository, LoginAsyncRepository>();
builder.Services.AddScoped<ICompetitionPaymentAsyncRepository, CompetitionPaymentAsyncRepository>();







builder.Services.AddSingleton<BaseAsyncRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    // Enable middleware to serve generated Swagger as a JSON endpoint with different route
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentname}/swagger.json";
    });

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        c.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//For the Uploads folder
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Uploads"
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Uploads"
});

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");
app.MapControllers();

app.Run();