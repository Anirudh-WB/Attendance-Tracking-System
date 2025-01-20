using ATS.Data;
using ATS.Hubs;
using ATS.IRepository;
using ATS.IServices;
using ATS.Repository;
using ATS.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

// Configure Entity Framework
builder.Services.AddDbContext<ATSDbContext>(option =>
    option.UseSqlServer(connectionString, b => b.MigrationsAssembly("ATS.API"))
);

// Add SignalR
builder.Services.AddSignalR();

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyAngularApp", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "http://192.168.29.191:4200",
                "http://192.168.29.207:4200",
                "http://192.168.29.242:4200",
                "http://localhost:62292",
                "http://localhost:58842",
                "http://127.0.0.1:8000",
                "http://localhost:8000",
                "http://10.10.10.13:8000",
                "http://10.10.10.13:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Dependency injection for repositories and services
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleServices, RoleServices>();

builder.Services.AddScoped<IDesignationRepository, DesignationRepository>();
builder.Services.AddScoped<IDesignationServices, DesignationServices>();

builder.Services.AddScoped<IGenderRepository, GenderRepository>();
builder.Services.AddScoped<IGenderServices, GenderServices>();

builder.Services.AddScoped<IAttendanceLogRepository, AttendanceLogRepository>();
builder.Services.AddScoped<IAttendanceLogServices, AttendanceLogServices>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserServices, UserServices>();

builder.Services.AddScoped<IEmployeeDetailRepository, EmployeeDetailRepository>();
builder.Services.AddScoped<IEmployeeDetailServices, EmployeeDetailServices>();

builder.Services.AddScoped<IPageRepository, PageRepository>();
builder.Services.AddScoped<IPageServices, PageServices>();

builder.Services.AddScoped<IAccessPageRepository, AccessPageRepository>();
builder.Services.AddScoped<IAccessPageServices, AccessPageServices>();

builder.Services.AddControllers();

var app = builder.Build();

// Middleware Configuration
app.UseCors("AllowMyAngularApp"); // CORS must be used before routing
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<AtsHubs>("/atsHub");
    endpoints.MapControllers();
});

app.Run();