using System.ComponentModel.DataAnnotations;

namespace EventService.Core.DTOs;

public class UpdateBilletTypeDto
{
    [Required]
    public int OrganisateurId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be >= 0")]
    public decimal Prix { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be >= 1")]
    public int Quantite { get; set; }
}