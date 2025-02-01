using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
namespace Database;
public class ApplicationDbContext : DbContext
{
	/// <summary>
	/// in appsettings.json in connection string change your database credentials
	/// </summary>
	/// <example>
	/// <code>
	///  "ConnectionStrings": {
	///  "DefaultConnection": "Server=YOURSERVSERNAME;Database=YOURDATABASENAME;Trusted_Connection=True;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true;"
	///},
	/// </code>
	/// </example>
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    #region DBSets
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
	#endregion
	/// <summary>
	/// In order to run migration you should invoke belows comment 
	/// </summary>
	/// <example>
	/// <code>
	/// dotnet ef migrations add InitialCreate --project Database --startup-project API --output-dir Migrations
	/// dotnet ef database update --project Database --startup-project API
	/// </code>
	/// </example>
	protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Book>().HasData(new Book() { Title = "Harry poter", Author= "JK ROLLINF", ISBN = "9780743273565", Available = true, DateCreated = DateTime.Now,  PublishedDate = DateTime.Now, ID = Guid.NewGuid()});

		builder.Entity<Book>().ToTable("Book");
        builder.Entity<Book>().HasKey(b => b.ID);
        builder.Entity<Book>().Property(x => x.ID).HasDefaultValueSql("NEWID()");
        builder.Entity<Book>().Property(b => b.Title).IsRequired().HasMaxLength(255);
        builder.Entity<Book>().Property(b => b.Author).IsRequired().HasMaxLength(255);
        builder.Entity<Book>().Property(b => b.ISBN).IsRequired().HasColumnType("nvarchar(max)");
		builder.Entity<Book>().Property(b => b.PublishedDate).IsRequired();
        builder.Entity<Book>().Property(b => b.Available).IsRequired();
        builder.Entity<Book>().Property(b => b.DateCreated).IsRequired();
        builder.Entity<Book>().Property(b => b.DateUpdated);
        builder.Entity<Book>().Property(b => b.DateDeleted);



        builder.Entity<User>().ToTable("User");
        builder.Entity<User>().HasKey(u => u.ID);
        builder.Entity<User>().Property(x => x.ID).HasDefaultValueSql("NEWID()");
        builder.Entity<User>().Property(u => u.Name).IsRequired().HasMaxLength(255);
        builder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.Entity<User>().Property(u => u.Role).IsRequired().HasMaxLength(50);
        builder.Entity<User>().Property(b => b.Password).IsRequired();
        builder.Entity<User>().Property(b => b.DateCreated).IsRequired();
        builder.Entity<User>().Property(b => b.DateUpdated);
        builder.Entity<User>().Property(b => b.DateDeleted);

    }
}
