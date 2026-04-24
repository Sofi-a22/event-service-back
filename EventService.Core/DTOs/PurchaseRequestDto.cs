using System.ComponentModel.DataAnnotations;

namespace EventService.Core.DTOs;

public class PurchaseRequestDto
{
    [Required]
    [Range(1, 5)]
    public int Quantite { get; set; }

    [Required]
    public int VisiteurId { get; set; }
}