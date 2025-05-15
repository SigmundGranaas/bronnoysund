using BrønnApi.Brreg;
using BrønnApi.Company;
using BrønnApi.Csv;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BrregApiSettings>(builder.Configuration.GetSection("BrregApi"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient<IBrregApiClient, BrregApiClient>();

builder.Services.AddScoped<ICompanyProcessingService, CompanyService>();
builder.Services.AddScoped<CsvCompanyParser>(); 

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 268435456; // 256 MB
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactFrontend",
        policyBuilder => 
        {
            policyBuilder.WithOrigins("http://localhost:5173") 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowReactFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();