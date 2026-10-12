namespace Solutions;

using NLog;
using NLog.Config;
using NLog.Targets;


public class Program 
{
	Logger mLog = LogManager.GetCurrentClassLogger();

	public static void Main(string[] args)
	{
		var console = new ConsoleTarget("console");
		console.Layout = @"[${date:format=HH\:MM\:ss}][${logger}]: ${message}";

		var cfg = new LoggingConfiguration();		
		cfg.AddRule(LogLevel.Info, LogLevel.Fatal, console);

		LogManager.Configuration = cfg;

		var self = new Program();
		self.Run(args);
	}

	void Run(string[] args)
	{
		mLog.Info("Starting.");

		var sol = new Solution();

		foreach( var input in Solution.TestData.Inputs )
		{
			var output = sol.Run(input);
			foreach (var result in output.Results)
			{
				Console.WriteLine(result);
			}
		}

		mLog.Info("All done.");
	}
}
