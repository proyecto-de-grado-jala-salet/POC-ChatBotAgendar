using Repositories;
using Services.Interfaces;
using System.Globalization;

namespace Background;

public class AppointmentReminderService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    // Se revisa cada 30 segundos
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

    public AppointmentReminderService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var appointmentRepository = scope.ServiceProvider.GetRequiredService<InMemoryAppointmentRepository>();
                var whatsAppService = scope.ServiceProvider.GetRequiredService<IWhatsAppService>();

                DateTime ahora = DateTime.Now;
                DateTime recordatorioInicio = ahora.AddMinutes(2);
                DateTime recordatorioFin = ahora.AddMinutes(3);

                var citasPendientes = appointmentRepository.GetAppointmentsInRange(recordatorioInicio, recordatorioFin)
                    .Where(c => !c.NotificacionEnviada)
                    .ToList();

                Console.WriteLine($"[ReminderService] Encontradas {citasPendientes.Count} citas pendientes.");

                foreach (var cita in citasPendientes)
                {
                    string mensaje = $"Recordatorio: Tiene una cita programada a las {cita.FechaHora.ToString("H:mm, dd MMM yyyy", new CultureInfo("es-ES"))} para {cita.Especialidad}.";
                    await whatsAppService.SendTextMessageAsync(cita.Phone, mensaje);

                    cita.NotificacionEnviada = true;
                    appointmentRepository.UpdateAppointment(cita);
                }
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}