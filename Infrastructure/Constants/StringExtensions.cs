using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Constants
{
	//BASE ON https://stackoverflow.com/questions/2717910/in-asp-net-mvc-how-do-i-display-a-property-name-as-a-label-splitting-on-camelca
	public static class StringExtensions
	{
		public static string SplitCamelCase(this string camelCaseString)
		{
			if (string.IsNullOrEmpty(camelCaseString))
				return camelCaseString;
			return System.Text.RegularExpressions.Regex.Replace(
				camelCaseString,
				"([a-z])([A-Z])",
				"$1 $2",
				System.Text.RegularExpressions.RegexOptions.Compiled
			).Trim();
		}
	}
}
