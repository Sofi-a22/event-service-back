namespace EventService.Core.Enums;

public enum StatutBillet
{
    Disponible,   // ticket created, not yet purchased
    Reserve,      // payment initiated / pending
    Confirme,     // payment confirmed
    Utilise,      // scanned at entrance
    Annule        // cancelled
}
