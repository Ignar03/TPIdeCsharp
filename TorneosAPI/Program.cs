using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TorneosAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TorneosDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

#region CorsRules 

var CorsRules = "CorsRules";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsRules,
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });

});
#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSwaggerGen(o =>
{
o.SwaggerDoc("v1",
    new OpenApiInfo
    {
        Title = "Prueba API",
        Description = "Una aplicacion simple para mostrar el funcionamiento de las APIs", 
        Version = "v1",
        TermsOfService = null,
        Contact = new OpenApiContact
        {
            // Check for optional parameters 
        },
        License = new OpenApiLicense
        {
            // Optional Example 
            // Name = "Proprietary", 
            // Url = new Uri("https://someURLToLicenseInfo.com") 
        }
    });
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
     $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

var app = builder.Build();

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service Manager API V1");
    });
}

app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
