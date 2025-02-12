using Microsoft.OpenApi.Models;
using Model;
using Services;
using Services.Handler;
using Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Agregar servicios a la inyección de dependencias
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Registro de servicios personalizados
builder.Services.AddSingleton<IConversationManager, ConversationManager>();
builder.Services.AddSingleton<IAppointmentSlotManager, AppointmentSlotManager>();
builder.Services.AddScoped<IConversationStateHandler, ConfirmationStateHandler>();
builder.Services.AddScoped<IConversationStateHandler, SpecialtyStateHandler>();
builder.Services.AddScoped<IConversationStateHandler, ScheduleStateHandler>();
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
builder.Services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();
builder.Services.AddScoped<IChatBotService, ChatBotService>();

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

app.UseAuthorization();
app.MapControllers();
app.Run();
