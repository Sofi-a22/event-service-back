namespace EventService.Core.DTOs;

public class BilletResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public DateTime? DateReservation { get; set; }
    public string BilletTypeNom { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public int EvenementId { get; set; }
    public string EvenementTitre { get; set; } = string.Empty;
    public DateTime EvenementStartDate { get; set; }
    public DateTime EvenementEndDate { get; set; }
    public string EvenementType { get; set; } = string.Empty;
}