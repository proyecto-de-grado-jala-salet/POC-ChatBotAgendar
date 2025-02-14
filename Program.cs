using Microsoft.OpenApi.Models;
using Services;
using Services.Handler;
using Services.Interfaces;
using Background;
using Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to dependency injection
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Custom Service Registration
builder.Services.AddSingleton<IConversationManager, ConversationManager>();
builder.Services.AddSingleton<IAppointmentSlotManager, AppointmentSlotManager>();

// Register the in-memory repository
builder.Services.AddSingleton<InMemoryAppointmentRepository>();

// Registering handlers and services
builder.Services.AddScoped<IConversationStateHandler, ConfirmationStateHandler>();
// Handler for "esperando_especialidad"
builder.Services.AddScoped<IConversationStateHandler, SpecialtyStateHandler>();
builder.Services.AddScoped<IConversationStateHandler, ScheduleStateHandler>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();
builder.Services.AddScoped<IChatBotService, ChatBotService>();

// Register background service for notifications
builder.Services.AddHostedService<AppointmentReminderService>();

// Swagger/OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MiProyectoApi", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiProyectoApi v1");
    });
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
