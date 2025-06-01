using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Application.Interfaces;
using OrderManagementSystem.Application.Rules;
using OrderManagementSystem.Application.Services;
using OrderManagementSystem.Infrastructure.Data;
using OrderManagementSystem.Infrastructure.Interfaces;
using OrderManagementSystem.Infrastructure.Repositories;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<OrderManagementSystemContext>(options =>
        options.UseInMemoryDatabase("OrderManagementInMemoryDb"));

    //var connectionString = builder.Configuration.GetConnectionString("OrderManagementSystemContext");
    //if (string.IsNullOrEmpty(connectionString))
    //{
    //    throw new InvalidOperationException("Connection string 'OrderManagementSystemContext' not found.");
    //}

    //builder.Services.AddDbContext<OrderManagementSystemContext>(options =>
    //    options.UseSqlServer(connectionString));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("OrderManagementSystemContext");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string 'OrderManagementSystemContext' not found.");
    }

    builder.Services.AddDbContext<OrderManagementSystemContext>(options =>
        options.UseSqlServer(connectionString));
}
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.InvalidModelStateResponseFactory = ctx =>
        new BadRequestObjectResult(new
        {
            message = "Validation failed",
            errors = ctx.ModelState
                       .Where(e => e.Value.Errors.Any())
                       .Select(e => new { Field = e.Key, Messages = e.Value.Errors.Select(x => x.ErrorMessage) })
        });
});

builder.Services.AddApiVersioning(opt =>
{
    opt.ReportApiVersions = true;
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Order Management API v1",
        Description = "API for managing orders, discounts, and analytics (v1)."
    });

    // Include XML docs for the API project
    var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
    c.IncludeXmlComments(apiXmlPath);

    // Include XML docs for Application layer
    var appAssembly = Assembly.Load("OrderManagementSystem.Application");
    var appXmlFile = $"{appAssembly.GetName().Name}.xml";
    var appXmlPath = Path.Combine(AppContext.BaseDirectory, appXmlFile);
    if (File.Exists(appXmlPath))
    {
        c.IncludeXmlComments(appXmlPath);
    }

    // Include XML docs for Domain layer
    var domainAssembly = Assembly.Load("OrderManagementSystem.Domain");
    var domainXmlFile = $"{domainAssembly.GetName().Name}.xml";
    var domainXmlPath = Path.Combine(AppContext.BaseDirectory, domainXmlFile);
    if (File.Exists(domainXmlPath))
    {
        c.IncludeXmlComments(domainXmlPath);
    }
});


builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddSingleton<PromotionRule, GoldCustomerRule>();
builder.Services.AddSingleton<PromotionRule, LoyalCustomerRule>();
builder.Services.AddSingleton<PromotionRule, HighValueOrderRule>();
builder.Services.AddSingleton<PromotionRule, SilverCustomerRule>();
builder.Services.AddSingleton<PromotionRule, BronzeCustomerRule>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<OrderManagementSystemContext>();
    //db.Database.Migrate(); ---- use with sql server
    DbSeeder.SeedTestData(db);

    var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            opt.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"Order Management API {desc.GroupName.ToUpperInvariant()}");
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }