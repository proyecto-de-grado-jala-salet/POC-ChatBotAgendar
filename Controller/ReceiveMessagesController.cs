using Microsoft.AspNetCore.Mvc;
using Model;
using POC_ChatBotAgendar.Services;
using POC_ChatBotAgendar.Model;
using System.Globalization;

namespace Controller;

[ApiController]
public class ReceiveMessagesController : ControllerBase
{
    // Endpoint para la validación GET
    [HttpGet]
    [Route("webhook")]
    public string Webhook(
        [FromQuery(Name = "hub.mode")] string mode,
        [FromQuery(Name = "hub.challenge")] string challenge,
        [FromQuery(Name = "hub.verify_token")] string verify_token)
    {
        if (verify_token.Equals("hola"))
        {
            return challenge;
        }
        else
        {
            return "";
        }
    }

    [HttpPost]
    [Route("webhook")]
    public async Task<dynamic> Datos([FromBody] WebHookResponseModel entry)
    {
        string mensaje_recibido = entry.entry[0].changes[0].value.messages[0].text.body;
        string telefono_wa = entry.entry[0].changes[0].value.messages[0].from;

        // Respuesta inicial
        if (mensaje_recibido.ToLower().Contains("hola"))
        {
            await WhatsAppService.SendTextMessage(telefono_wa, "Hola, es el Centro de Atención E-Dukate. ¿Desea reservar una cita? (Responda 'Sí' o 'No')");
        }
        // Confirmación de reserva
        else if (mensaje_recibido.ToLower().Contains("sí") || mensaje_recibido.ToLower().Contains("si"))
        {
            await WhatsAppService.SendTextMessage(telefono_wa, "Por favor, seleccione la especialidad:\n1. Psicóloga\n2. Fisioterapia");
        }
        // Selección de especialidad
        else if (mensaje_recibido == "1" || mensaje_recibido == "2")
        {
            string especialidad = mensaje_recibido == "1" ? "Psicóloga" : "Fisioterapia";
            await WhatsAppService.SendTextMessage(telefono_wa, $"Ha seleccionado {especialidad}. Estos son los horarios disponibles:\n" +
                "1. 21:00, 10 feb 2025\n" +
                "2. 22:00, 10 feb 2025\n" +
                "3. 23:00, 10 feb 2025\n" +
                "4. 1:00, 11 feb 2025\n" +
                "5. 2:00, 11 feb 2025\n" +
                "6. 3:00, 11 feb 2025\n" +
                "7. 4:00, 11 feb 2025\n" +
                "8. 5:00, 11 feb 2025\n" +
                "9. 6:00, 11 feb 2025\n" +
                "Por favor, seleccione un horario (1-9).");
        }
        // Selección de horario
        else if (mensaje_recibido.Length == 1 && char.IsDigit(mensaje_recibido[0]) && mensaje_recibido[0] >= '1' && mensaje_recibido[0] <= '9')
        {
            int horarioIndex = int.Parse(mensaje_recibido) - 1;
            string[] horarios = {
                "21:00, 10 feb 2025",
                "22:00, 10 feb 2025",
                "23:00, 10 feb 2025",
                "1:00, 11 feb 2025",
                "2:00, 11 feb 2025",
                "3:00, 11 feb 2025",
                "4:00, 11 feb 2025",
                "5:00, 11 feb 2025",
                "6:00, 11 feb 2025"
            };

            string horarioSeleccionado = horarios[horarioIndex];
            await WhatsAppService.SendTextMessage(telefono_wa, $"Ha seleccionado el horario: {horarioSeleccionado}. Su cita está siendo agendada.");

            // Aquí puedes agregar la lógica para agregar el evento a Google Calendar
            try
                {
                    // Convertimos la cadena a DateTime usando la cultura española
                    var culture = new CultureInfo("es-ES");
                    DateTime startTime = DateTime.ParseExact(horarioSeleccionado, "H:mm, dd MMM yyyy", culture);

                    // Recuperamos la especialidad que se guardó anteriormente
                    string specialty = ConversationManager.GetSpecialty(telefono_wa);

                    // Llamamos al método que crea el evento en Google Calendar
                    bool eventCreated = GoogleCalendarService.CreateEvent(startTime, specialty);
                    if (eventCreated)
                    {
                        await WhatsAppService.SendTextMessage(telefono_wa, "La cita se agendó correctamente en Google Calendar.");
                    }
                    else
                    {
                        await WhatsAppService.SendTextMessage(telefono_wa, "Ocurrió un error al agendar la cita en Google Calendar.");
                    }
                }
                catch (Exception ex)
                {
                    // Si ocurre un error al parsear la fecha o crear el evento, se notifica al usuario
                    await WhatsAppService.SendTextMessage(telefono_wa, "Ocurrió un error al procesar la fecha y hora seleccionada.");
                }

        }
        // Respuesta negativa
        else if (mensaje_recibido.ToLower().Contains("no"))
        {
            await WhatsAppService.SendTextMessage(telefono_wa, "Gracias por contactarnos. ¡Hasta luego!");
        }
        // Respuesta no reconocida
        else
        {
            await WhatsAppService.SendTextMessage(telefono_wa, "Lo siento, no entendí su respuesta. Por favor, siga las instrucciones.");
        }

        return Ok();
    }
}