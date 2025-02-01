using Application.DTOs;
using Application.Token;
using Client.Services;
using Infrastructure.Constants;
using Irony.Parsing;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Client.States
{
	/// <summary>
	/// Handles authentication state and veryfies if user is logged in base on JWT Token
	/// </summary>
	public class CustomAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
		private readonly ILocalStorageService _localStorageService;

		public CustomAuthenticationStateProvider(ILocalStorageService localStorageService)
		{
			_localStorageService = localStorageService;
		}
		public async override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			try
			{
                 var token = await _localStorageService.GetItem(Constants.JWTTokenName);

                if (string.IsNullOrEmpty(token))
				{
					return await Task.FromResult(new AuthenticationState(anonymous));
				}

                var getUserClaims = DecryptJWTService.DecryptToken(token);
				if (getUserClaims == null)
				{
					return await Task.FromResult(new AuthenticationState(anonymous));
				}
				var claimsPrincipal = SetClaimPrincipal(getUserClaims);

				var state = new AuthenticationState(claimsPrincipal);

				NotifyAuthenticationStateChanged(Task.FromResult(state));

				return state;
				//return await Task.FromResult(new AuthenticationState(claimsPrincipal));
			}
			catch
			{
				return await Task.FromResult(new AuthenticationState(anonymous));
			}

		}
		/// <summary>
		/// Set the current token to local storage
		/// </summary>
		/// <param name="jwtToken">Token from login action</param>
		/// <returns></returns>
		public async Task UpdateAuthenticationState(string jwtToken)
		{
			var claimsPrincipal = new ClaimsPrincipal();
			if (!string.IsNullOrEmpty(jwtToken))
			{
			    await _localStorageService.SetItem(Constants.JWTTokenName, jwtToken);
				var getUserClaims = DecryptJWTService.DecryptToken(jwtToken);
				claimsPrincipal = SetClaimPrincipal(getUserClaims);
			}
			else
			{
			  await _localStorageService.RemoveItem(Constants.JWTTokenName);
			}
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));

		}
		public static ClaimsPrincipal SetClaimPrincipal(CustomUserClaims claims)
		{
			if (claims.Email is null) return new ClaimsPrincipal();
			return new ClaimsPrincipal(new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.Name, claims.Name),
				new Claim(ClaimTypes.Email, claims.Email),
			   new Claim(ClaimTypes.Role, claims.Role)
			}, "JwtAuth"));
		}


		//OR THIS APPROACH
		//public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		//{
		//	string token = await _localStorageService.GetItem(Constants.JWTTokenName);

		//	var identity = new ClaimsIdentity();

		//	if (!string.IsNullOrEmpty(token))
		//	{
		//		identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
		//	}

		//	var user = new ClaimsPrincipal(identity);
		//	var state = new AuthenticationState(user);

		//	NotifyAuthenticationStateChanged(Task.FromResult(state));

		//	return state;
		//}
		//public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
		//{
		//	var payload = jwt.Split('.')[1];
		//	var jsonBytes = ParseBase64WithoutPadding(payload);
		//	var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
		//	return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
		//}

		//private static byte[] ParseBase64WithoutPadding(string base64)
		//{
		//	switch (base64.Length % 4)
		//	{
		//		case 2: base64 += "=="; break;
		//		case 3: base64 += "="; break;
		//	}
		//	return Convert.FromBase64String(base64);
		//}

	}
}

/*
 * USAGE @inject AuthenticationStateProvider AuthStateProvider
 async Task HandleLogin()
    {
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IlNoYWhyaXlhciIsInJvbGUiOiJBZG1pbiIsImlhdCI6MTUxNjIzOTAyMn0.l9E7Oypb-ozndpFUkeVhOYzhtjGEuFmdYdAxhbpXAFY";
 *    await LocalStorage.SetItemAsync("token", token);
        await AuthStateProvider.GetAuthenticationStateAsync();
    }
on pages @attribute [Authorize]
@attribute [Authorize(Roles = "Admin")]

 * */
