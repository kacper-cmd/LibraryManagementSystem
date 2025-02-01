﻿using System.Globalization;

namespace Client.Components.Layout
{
	public partial class CultureSelector
	{
		protected override void OnInitialized()
		{
			Culture = CultureInfo.CurrentCulture;
		}

		private CultureInfo Culture
		{
			get
			{
				return CultureInfo.CurrentCulture;
			}
			set
			{
				if (CultureInfo.CurrentCulture != value)
				{
					var uri = new Uri(Navigation.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
					var cultureEscaped = Uri.EscapeDataString(value.Name);
					var uriEscaped = Uri.EscapeDataString(uri);

					Navigation.NavigateTo($"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}", forceLoad: true);
				}
			}
		}
	}
}
