using System.Collections.Generic;

namespace EventService.Core.Models;

public class BilletType
{
    public int Id { get; set; }  
    public string Nom { get; set; } = string.Empty;  
    public decimal Prix { get; set; }
    public int Quantite { get; set; }
    public int Vendu { get; set; }

    public int EvenementId { get; set; }

    public Evenement Evenement { get; set; } = null!;
    public ICollection<Billet> Billets { get; set; } = new List<Billet>();
}