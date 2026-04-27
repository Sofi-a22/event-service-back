using EventService.Core.Interfaces;

namespace EventService.Infrastructure.Services;

public class TicketCodeGenerator : ITicketCodeGenerator
{
    public string Generate(int eventId)
    {
        var random = Guid.NewGuid().ToString("N")[..6].ToUpper();
        return $"EVT-{eventId}-{random}";
    }
}