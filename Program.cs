using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PharmaDistiPro.Repositories.Interface;
using PharmaDistiPro.Repositories.Impl;
using PharmaDistiPro.Services.Interface;
using PharmaDistiPro.Services.Impl;
using PharmaDistiPro.Models;
using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using VNPAY.NET;

namespace PharmaDistiPro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            #region VNpay

            builder.Services.AddScoped<IVnpay, Vnpay>();
            #endregion

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is missing or empty.");
            }

            builder.Services.AddDbContext<SEP490_G74Context>(options =>
                options.UseSqlServer(connectionString)
                       .EnableDetailedErrors()
                       .EnableSensitiveDataLogging());

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddMemoryCache(); // Cấu hình MemoryCache
            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            // Cloudinary
            var cloudName = builder.Configuration.GetValue<string>("Cloudinary:CloudName");
            var apiKey = builder.Configuration.GetValue<string>("Cloudinary:Key");
            var apiSecret = builder.Configuration.GetValue<string>("Cloudinary:Secret");
            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new InvalidOperationException("Cloudinary configuration is missing.");
            }
            var account = new Account(cloudName, apiKey, apiSecret);
            var cloudinary = new Cloudinary(account);
            builder.Services.AddSingleton(cloudinary);

            // Email
            var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            if (emailConfig == null)
            {
                throw new InvalidOperationException("Email configuration is missing.");
            }
            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddScoped<IEmailService, EmailService>();

            // Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Director", policy =>
                    policy.RequireRole("Director"));
                options.AddPolicy("WarehouseManager", policy =>
                   policy.RequireRole("WarehouseManager"));
                options.AddPolicy("SalesManager", policy =>
                   policy.RequireRole("SalesManager"));
                options.AddPolicy("SalesMan", policy =>
                   policy.RequireRole("SalesMan"));
                options.AddPolicy("Customer", policy =>
                   policy.RequireRole("Customer"));// Customize as needed
            });
            // GHN Service
            builder.Services.AddHttpClient<IGHNService, GHNService>();

            // Repositories
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IStorageRoomRepository, StorageRoomRepository>();
            builder.Services.AddScoped<IUnitRepository, UnitRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrdersDetailRepository, OrdersDetailRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IIssueNoteRepository, IssueNoteRepository>();
            builder.Services.AddScoped<IIssueNoteDetailsRepository, IssueNoteDetailsRepository>();
            builder.Services.AddScoped<IProductLotRepository, ProductLotRepository>();
            builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            builder.Services.AddScoped<IPurchaseOrdersDetailRepository, PurchaseOrdersDetailRepository>();
            builder.Services.AddScoped<ILotRepository, LotRepository>();
            builder.Services.AddScoped<IReceivedNoteRepository, ReceivedNoteRepository>();
            builder.Services.AddScoped<INoteCheckRepository, NoteCheckRepository>();
            builder.Services.AddScoped<INoteCheckDetailsRepository, NoteCheckDetailsRepository>();
            builder.Services.AddScoped<IStorageHistoryRepository, StorageHistoryRepository>();

            // Services
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IStorageRoomService, StorageRoomService>();
            builder.Services.AddScoped<IUnitService, UnitService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IIssueNoteService, IssueNoteService>();
            builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            builder.Services.AddScoped<ILotService, LotService>();
            builder.Services.AddScoped<IProductLotService, ProductLotService>();
            builder.Services.AddScoped<IReceivedNoteService, ReceivedNoteService>();
            builder.Services.AddScoped<INoteCheckService, NoteCheckService>();
            builder.Services.AddScoped<IStorageHistoryService, StorageHistoryService>();

            // AutoMapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

          

            var app = builder.Build();

          
          

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();


            app.Run();
        }
    }
}
