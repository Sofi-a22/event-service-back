namespace EventService.Core.Models;

public class Billet
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Statut { get; set; } = "Disponible";

    public DateTime? DateReservation { get; set; }
    public DateTime? DateValidation { get; set; }
    public int? VisiteurId { get; set; }
    public int BilletTypeId { get; set; }

    public string PaymentTransactionId { get; set; } 
    
    public BilletType BilletType { get; set; } = null!;
}