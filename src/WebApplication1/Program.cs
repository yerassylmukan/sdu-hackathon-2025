using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApplication1;
using WebApplication1.Common;
using WebApplication1.Common.Interfaces;
using WebApplication1.Data;
using WebApplication1.Hubs;
using WebApplication1.Identity;
using WebApplication1.Services;
using WebApplication1.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITokenClaimService, IdentityTokenClaimService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IBasketService, BasketService>();

builder.Services.AddSignalR();

builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserModelValidator>());

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var key = Encoding.ASCII.GetBytes(AuthorizationConstants.JWT_SECRET_KEY);
builder.Services.AddAuthentication(config => { config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
    .AddJwtBearer(config =>
    {
        config.RequireHttpsMetadata = false;
        config.SaveToken = true;
        config.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.EnableAnnotations();
    c.SchemaFilter<CustomSchemaFilters>();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var scopedProvider = scope.ServiceProvider;
var identityDbContext = scopedProvider.GetRequiredService<AppIdentityDbContext>();
var userManager = scopedProvider.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole>>();
await AppIdentityDbContextSeed.SeedAsync(identityDbContext, userManager, roleManager);
var applicationDbContext = scopedProvider.GetRequiredService<ApplicationDbContext>();
await ApplicationDbContextSeed.SeedDataAsync(applicationDbContext);

app.UseRouting();

app.MapHub<OrderStatusHub>("/hubs/orderStatus");

app.UseAuthentication();

app.UseAuthorization();

app.UseSwagger();

app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

app.MapControllers();

app.Run();