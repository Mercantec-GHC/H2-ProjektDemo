using API.Service;
using Npgsql;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*")
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                      });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine($"ConnectionString: {builder.Configuration.GetConnectionString("DefaultConnection")}");
Console.WriteLine($"AccessKey: {builder.Configuration.GetConnectionString("AccessKeyID")}");
Console.WriteLine($"SecretKey: {builder.Configuration.GetConnectionString("secretKey")}");


// In your Startup.cs or Program.cs
builder.Services.AddSingleton(new AppConfiguration
{
    ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DefaultConnection"),
    AccessKey = builder.Configuration.GetConnectionString("AccessKeyID") ?? Environment.GetEnvironmentVariable("AccessKeyID"),
    SecretKey = builder.Configuration.GetConnectionString("secretKey") ?? Environment.GetEnvironmentVariable("secretKey")
});


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
