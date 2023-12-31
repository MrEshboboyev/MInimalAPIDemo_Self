var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// using MapGet
app.MapGet(("/hello"), () => "Hello, World!");

// using MapPost
app.MapPost(("/helloPost"), () => "Hello, World from Post");

app.UseHttpsRedirection();
app.Run();
