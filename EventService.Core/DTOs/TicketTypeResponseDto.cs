namespace EventService.Core.DTOs;

public class TicketTypeResponseDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public int Capacity { get; set; }
    public int Sold { get; set; }
    public int Available => Capacity - Sold;
}