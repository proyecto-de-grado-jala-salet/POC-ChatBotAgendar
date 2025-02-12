using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Services.Interfaces;

namespace Services;

public class GoogleCalendarService : IGoogleCalendarService
{
    public async Task<bool> CreateEventAsync(DateTime startTime, string specialty)
    {
        try
        {
            // Cargar las credenciales desde un archivo JSON
            GoogleCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(CalendarService.Scope.Calendar);
            }

            // Inicializar el servicio de Calendar
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "ReservaCitas"
            });

            // Suponemos que la cita dura 1 hora
            DateTime endTime = startTime.AddHours(1);

            Event newEvent = new Event()
            {
                Summary = "Cita " + specialty,
                Start = new EventDateTime()
                {
                    DateTime = startTime,
                    TimeZone = "America/La_Paz"
                },
                End = new EventDateTime()
                {
                    DateTime = endTime,
                    TimeZone = "America/La_Paz"
                }
            };

            // Usamos el calendario "primary"
            string calendarId = "0cf5fbcfecbe10a3b146d55fa48e696fa137672b760ad6ec9c33276ad57670d2@group.calendar.google.com";
            var request = service.Events.Insert(newEvent, calendarId);
            request.Execute();

            return true;
        }
        catch (Exception ex)
        {
            // Imprime o registra el error para depuración
            Console.WriteLine("Error al crear el evento: " + ex);
            return false;
        }
    }

}