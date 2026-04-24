namespace EventService.Core.DTOs;

public class PurchaseTicketsDto
{
    public int EventId { get; set; }
    public Dictionary<int, int> TicketQuantities { get; set; } = new();
}