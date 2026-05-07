using System.Security.Claims;

namespace EventService.Core.Helpers;

public static class ClaimsHelper
{
    // User Service puts email in "sub" claim
    public static string GetEmail(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? user.FindFirst("sub")?.Value
            ?? string.Empty;
    }

    // User Service puts numeric ID in "userId" claim (after teammate adds it)
    public static int GetUserId(ClaimsPrincipal user)
    {
        var claim = user.FindFirst("id")?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }

    // User Service puts role in "role" claim — values: ORGANISATEUR, VISITEUR, VISITEUR_VERIFIE, ADMIN
    public static string GetRole(ClaimsPrincipal user)
    {
        return user.FindFirst("role")?.Value ?? string.Empty;
    }

    public static bool IsOrganisateur(ClaimsPrincipal user)
    {
        return GetRole(user) == "ORGANISATEUR";
    }

    public static bool IsVisiteur(ClaimsPrincipal user)
    {
        var role = GetRole(user);
        return role == "VISITEUR" || role == "VISITEUR_VERIFIE";
    }

    public static bool IsAdmin(ClaimsPrincipal user)
    {
        return GetRole(user) == "ADMIN";
    }
}