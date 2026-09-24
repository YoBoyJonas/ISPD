namespace Solutions;

using NLog;
using NLog.Config;
using NLog.Targets;


/// <summary>
/// Program entry class.
/// </summary>
public class Program 
{
	/// <summary>
	/// Logger for this class.
	/// </summary>
	Logger mLog = LogManager.GetCurrentClassLogger();

	/// <summary>
	/// Program entry point.
	/// </summary>
	/// <param name="args">Command line arguments.</param>
	public static void Main(string[] args)
	{
		//configure logging
		var console = new ConsoleTarget("console");
		console.Layout = @"[${date:format=HH\:MM\:ss}][${logger}]: ${message}";

		var cfg = new LoggingConfiguration();		
		cfg.AddRule(LogLevel.Info, LogLevel.Fatal, console);

		LogManager.Configuration = cfg;

		//run
		var self = new Program();
		self.Run(args);
	}

	/// <summary>
	/// Program body. This exists as a convenience for debugging if you do not want to go through test to do that.
	/// </summary>
	/// <param name="args">Command line argumens.</param>
	void Run(string[] args)
	{
		mLog.Info("Starting.");

		//run solution for the task
		var sol = new Solution();

		foreach( var input in Solution.TestData.Inputs )
		{
			var output = sol.Run(input);
		}

		//
		mLog.Info("All done.");
	}
}