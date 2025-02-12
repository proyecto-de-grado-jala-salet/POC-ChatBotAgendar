namespace Services.Interfaces;

public interface IWhatsAppService
{
    Task SendTextMessageAsync(string phone, string message);
}
