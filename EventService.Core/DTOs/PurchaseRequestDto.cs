using System.ComponentModel.DataAnnotations;

namespace EventService.Core.DTOs;

public class PurchaseRequestDto
{
    [Required]
    public int VisiteurId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one ticket item is required")]
    public List<PurchaseItemDto> Items { get; set; } = new();
}