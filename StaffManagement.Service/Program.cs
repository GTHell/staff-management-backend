using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using StaffManagement;
using StaffManagement.Models;
using StaffManagement.Services;

#region Bootstrap
var builder = WebApplication.CreateBuilder(args);

// Add cosnole logging 
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add DI
builder.Services.AddTransient<IStaffService, StaffService>();

// Sqlite connection string
var connectionString = builder.Configuration.GetConnectionString("Staffs") ?? "Data Source=Staffs.db";

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { 
		Title = "Staff Management API", 
		Description = "System for managing the staff", 
		Version = "v1" });
});

// Add EFcore
builder.Services.AddSqlite<StaffDb>(connectionString);

// Allow origin for local development
var AllowLocalOrigin = "_LocalAllowOrigin";
builder.Services.AddCors(options => 
{
    options.AddPolicy(AllowLocalOrigin, policy => 
       {
		   policy
		   .AllowAnyHeader()
		   .AllowAnyMethod()
		   .AllowAnyOrigin();
       });    
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
	});

	app.UseCors(AllowLocalOrigin);
}

app.MapGet("/", () => "Staff Management");
#endregion

#region Staff CRUD API
app.MapGroup("/api/staffs")
	.MapStaffApi()
	.WithTags("Staff Api Endpoints");
#endregion

app.Run();

public partial class Program
{ }