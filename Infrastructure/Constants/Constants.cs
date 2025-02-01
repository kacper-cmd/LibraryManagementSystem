namespace Infrastructure.Constants;

public static class Constants
{
	public const string JWTTokenName = "jwttoken";
	/// <summary>
	/// Modyfy this path to be valid at your project 
	/// </summary>
	public const string SQLScriptPath = @"C:\Users\Kacper\Desktop\Staz\LibraryManagementSystemStaz\LibraryManagementSystem\Database\SQL\scripts.sql";
	public const string Ascending = "Ascending";
	public const string asc = "asc";
	public const string desc = "desc";
	public const string Descending = "Descending";

}
public static class CustomRoles
{
	public const string Admin = "Admin";
	public const string Librarian = "Librarian";
	public const string AdminOrLibrarian = Admin + "," + Librarian;
	public static string[] ExtractRole(string roles) => roles.Split(",");
}


