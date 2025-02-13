using Microsoft.OpenApi.Models;
using Model;
using Services;
using Services.Handler;
using Services.Interfaces;
using Background;
using Repositories;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios a la inyección de dependencias
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Registro de servicios personalizados
builder.Services.AddSingleton<IConversationManager, ConversationManager>();
builder.Services.AddSingleton<IAppointmentSlotManager, AppointmentSlotManager>();

// Registrar el repositorio in-memory
builder.Services.AddSingleton<InMemoryAppointmentRepository>();

// Registro de handlers y servicios
builder.Services.AddScoped<IConversationStateHandler, ConfirmationStateHandler>();
builder.Services.AddScoped<IConversationStateHandler, SpecialtyStateHandler>(); // Handler para "esperando_especialidad"
builder.Services.AddScoped<IConversationStateHandler, ScheduleStateHandler>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();
builder.Services.AddScoped<IChatBotService, ChatBotService>();

// Registrar el servicio en segundo plano para notificaciones
builder.Services.AddHostedService<AppointmentReminderService>();

// Configuración de Swagger/OpenAPI
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
