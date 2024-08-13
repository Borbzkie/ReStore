using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StoreContext>(opt =>  
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// Add services to the container
builder.Services.AddCors(options => 
{
    options.AddPolicy("DefaultPolicy",
        policybuilder =>
        {
           policybuilder.WithOrigins("http://localhost:3000","http://localhost:3000")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection(); //Hide for now because is not yet for production


if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();  
}

// Order is important in this section
// Addd middleware

/**
app.UseCors(opt => {
    opt.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000/");
}); **/

app.UseCors("DefaultPolicy");

app.UseAuthorization(); 
app.MapControllers();
// To hold off the db context , we create a scope 
var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

//create a logger 
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

try
{
    context.Database.Migrate();
    DbInitializer.Initialize(context);
}
catch (Exception ex)
{
    
    logger.LogError(ex, "A problem occured during  migration");
}

app.Run();