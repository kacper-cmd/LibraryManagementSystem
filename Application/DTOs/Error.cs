using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
	//lub BookNotFound : Error lub Exception
	public sealed record Error(string Code, string? Description = null)//ID
	{
		public static readonly Error None = new(string.Empty);
	}
	public static class BookErrors
	{
		public static Error BookNotFound => new("book/not-found", "Book not found.");
		public static Error SearchError => new("search/error", "Error in searching book");

	}
}
