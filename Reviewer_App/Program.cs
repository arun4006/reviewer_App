using Microsoft.EntityFrameworkCore;
using Reviewer_App;
using Reviewer_App.Data;
using Reviewer_App.Interfaces;
using Reviewer_App.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using NLog.Web;
using NLog;

//create logger
//var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
logger.Debug("init Main");
try
{

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
    builder.Services.AddScoped<ICatergoryRepository, CatergoryRepository>();
    builder.Services.AddScoped<ICountryRepository, CountryRepository>();
    builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
    builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
    builder.Services.AddScoped<IReviewerRepository, ReviewerRepository>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

    builder.Services.AddControllers();
    builder.Services.AddTransient<Seed>();
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("PokemonDbConnectionString"));
    });

    //nlog enable
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // line 23 to 32 -- seed data and store in db
    //run command: open terminal from Searching and type=> dotnet run seeddata
    if (args.Length == 1 && args[0].ToLower() == "seeddata")
        SeedData(app);

    void SeedData(IHost app)
    {
        var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

        using (var scope = scopedFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<Seed>();
            service.SeedDataContext();
        }
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch(Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw(ex);
}
