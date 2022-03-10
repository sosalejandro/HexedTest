var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.SetupBuilder();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.RegisterRoutes();
app.EnsureDbIsCreated();

app.Run();

public partial class Program { }