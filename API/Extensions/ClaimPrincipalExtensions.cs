using System.Security.Claims;

namespace API.Extensions;
public static class ClaimPrincipalExtensions
{
	/// <summary>
	/// Extension method to handle returning users info base on ClaimsPrincipal from the token 
	/// </summary>
	/// <param name="principal"></param>
	/// <returns></returns>
	public static string? GetUserId(this ClaimsPrincipal principal)
	{
		return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
	}
	public static string? GetUserEmail(this ClaimsPrincipal principal)
	{
		return principal.FindFirst(ClaimTypes.Email)?.Value;
	}
}
