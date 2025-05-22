using Block.API.Services;
using Block.Application.Interfaces.Repositories;
using Block.Application.Interfaces.Services;
using Block.Application.Services;
using Block.Infrastructure.Repositories;
using Block.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IPGeolocationService, GeolocationService>();

builder.Services.AddSingleton<ICountryBlockRepository, CountryBlockRepository>();
builder.Services.AddSingleton<IAttemptLogRepository, AttemptLogRepository>();

builder.Services.AddSingleton<ICountryBlockService, CountryBlockService>();
builder.Services.AddSingleton<IBlockedLogService, BlockedtLogService>();

builder.Services.AddHostedService<ExpiredBlockService>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});



var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.MapControllers();

app.Run();
