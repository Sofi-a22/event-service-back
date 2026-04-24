using System.ComponentModel.DataAnnotations;

namespace EventService.Core.DTOs;

public class CreateBilletTypeDto
{
    [Required]
    public string Nom { get; set; } = string.Empty;        // VIP, Normal, Student
    public decimal Prix { get; set; }                      // Price per ticket
    [Required]
    [Range(1, 10000)]
    public int Quantite { get; set; }                     // Total number available
}