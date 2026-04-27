using System.ComponentModel.DataAnnotations;

namespace EventService.Core.DTOs;

public class PurchaseItemDto
{
    [Required]
    public int BilletTypeId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Quantity per type must be between 1 and 5")]
    public int Quantite { get; set; }
}