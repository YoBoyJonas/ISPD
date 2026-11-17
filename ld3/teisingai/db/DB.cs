namespace Org.Ktu.T120B197.DBs;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


public partial class DB : DbContext
{
	/// <summary>
	/// Logger to use. Can be null.
	/// </summary>
	public static Action<string> Logger { get; set; }

	/// <summary>
	/// Constructor.
	/// </summary>
	public DB() : base()
	{
	}

	/// <summary>
	/// Is invoked to configure the context.
	/// </summary>
	/// <param name="optionsBuilder">Options builder to use.</param>
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if( !optionsBuilder.IsConfigured )
		{
			//set database connection string
			var dbConnStr = "Server=localhost;User=root;Password=root;Database=t120b197";
			optionsBuilder.UseMySql(dbConnStr, ServerVersion.AutoDetect(dbConnStr));

			//enable SQL logging for development
			if( Logger != null )
				optionsBuilder.LogTo(Logger, LogLevel.Information);
		}
	}
}
