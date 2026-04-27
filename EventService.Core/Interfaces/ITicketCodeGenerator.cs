namespace EventService.Core.Interfaces;

public interface ITicketCodeGenerator
{
    string Generate(int eventId);
}