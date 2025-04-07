using TaskManager.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Services;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Repositories;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using TaskManager.API.Validators;
using TaskManager.CrossCutting.Middlewares;
using TaskManager.Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Reflection;





var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Ativa a leitura do arquivo XML gerado com os coment�rios do c�digo
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddDbContext<TaskDbContext>(opt =>
    opt.UseInMemoryDatabase("TaskDb"));

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateTaskDtoValidator>();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();





var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();

