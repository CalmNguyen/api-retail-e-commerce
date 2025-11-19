using Microsoft.EntityFrameworkCore;
using retail_e_commerce.AutoMapper;
using retail_e_commerce.Common;
using retail_e_commerce.Data;
using retail_e_commerce.Filters;
using System.Reflection;
using System.Xml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuthenticationFilter>();
    //add catch global exception for controller
    options.Filters.Add<GlobalExceptionFilter>();
});
//Set first charracter in class is upper because default is lower
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });


if (File.Exists("log4net.config"))
{
    XmlDocument log4netConfig = new XmlDocument();
    log4netConfig.Load(File.OpenRead("log4net.config"));
    var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
    log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
    builder.Logging.ClearProviders();
    builder.Logging.AddLog4Net();
}

builder.Services.AddAutoMapper(typeof(MappingProfile));

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var domainUrl = builder.Configuration.GetSection("DomainUrl");
var domain_webclient_crm = domainUrl["web-client"].Split(',', StringSplitOptions.RemoveEmptyEntries);
var domain_webclient_local = domainUrl["web-client-local"].Split(',', StringSplitOptions.RemoveEmptyEntries);
builder.Services.AddCors(option => option.AddPolicy("clientWeb",
    policy =>
    {
        policy.WithOrigins(domain_webclient_local);
        policy.WithOrigins(domain_webclient_crm);
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();
    }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
