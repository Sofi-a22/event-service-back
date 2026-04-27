using EventService.Core.DTOs;

namespace EventService.Core.Interfaces;

public interface IPurchaseService
{
    Task<(bool Success, string Error, PurchaseResponseDto? Response)> PurchaseTicketsAsync(
        int eventId,
        PurchaseRequestDto request);
}