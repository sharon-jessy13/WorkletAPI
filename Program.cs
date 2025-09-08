using PrismWorkletApi.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 
builder.Services.AddScoped<IWorkletRepository, WorkletRepository>();
builder.Services.AddScoped<IMentorRepository, MentorRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();     
    app.UseSwaggerUI();   
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();