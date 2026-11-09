namespace Solutions;

using NLog;


/// <summary>
/// Solution.
/// </summary>
public class Solution
{
	/// <summary>
	/// A single input.
	/// </summary>
	public class Input
	{
        public int[] Values { get; set; }
    }

	/// <summary>
	/// A single output.
	/// </summary>
	public class Output
	{
        public int Closed { get; set; }
        public int Open { get; set; }
    }

	/// <summary>
	/// Test data.
	/// </summary>
	public class TestData
	{
        /// <summary>
        /// Array of test inputs.
        /// </summary>
        public static Input[] Inputs { get; } = {
            new Input
            {
                Values = [0, 2, 0, 3, 0, -1, 1, 0, 0, 0, 0, 0]
            },
            new Input
            {
                Values = [ 1, 2, 1, 2, 3, 1, -1, 1, 2, 0, 1, 0 ]
            },
            new Input
            {
                Values = [ 0, 5, 0, 2, 1, -1, 1, 3, 2, 1, 2, 1 ]
            },

            new Input
            {
                Values = [ 0, 4, 2, -1, 2, 1, -1, 1, 3, 1]
            }
        };

        /// <summary>
        /// Array of test outputs in corresponding order to test inputs.
        /// </summary>    
        public static Output[] Outputs { get; } = {
            new Output { Closed = 4, Open = 2 },
            new Output { Closed = 6, Open = 2 },
            new Output { Closed = 12, Open = 3 },
            new Output { Closed = 12, Open = 0},
        };
    }

	/// <summary>
	/// Logger for this class.
	/// </summary>
	Logger mLog = LogManager.GetCurrentClassLogger();


	/// <summary>
	/// Runs the task solution.
	/// </summary>
	/// <param name="input">Input</param>
	/// <returns>Output</returns>
	public Output Run(Input input)
	{

        int[] values = input.Values;
        int n = values.Length;

        // Highest wall to the left for each square
        int[] leftClosed = new int[n];
        int[] leftOpen = new int[n];
        int maxLeftClosed = 0;
        int maxLeftOpen = 0;

        Thread leftThread = new Thread(() =>
        {
            for (int i = 0; i < n; i++)
            {
                int height = Math.Max(0, values[i]);

                // Closed drains
                maxLeftClosed = Math.Max(maxLeftClosed, height);
                leftClosed[i] = maxLeftClosed;

                // Reset the border when there is an open drain
                if (values[i] == -1)
                {
                    maxLeftOpen = 0;
                }
                else
                {
                    maxLeftOpen = Math.Max(maxLeftOpen, height);
                }
                leftOpen[i] = maxLeftOpen;
            }
        });
        leftThread.Start();


        // Highest wall to the right for each square
        int[] rightClosed = new int[n];
        int[] rightOpen = new int[n];
        int maxRightClosed = 0;
        int maxRightOpen = 0;

        Thread rightThread = new Thread(() =>
        {

            for (int i = n - 1; i >= 0; i--)
            {
                int height = Math.Max(0, values[i]);

                // Closed drains
                maxRightClosed = Math.Max(maxRightClosed, height);
                rightClosed[i] = maxRightClosed;

                // Reset the border when there is an open drain
                if (values[i] == -1)
                {
                    maxRightOpen = 0;
                }
                else
                {
                    maxRightOpen = Math.Max(maxRightOpen, height);
                }
                rightOpen[i] = maxRightOpen;
            }
        });
        rightThread.Start();

        leftThread.Join();
        rightThread.Join();
        int waterClosed = 0;
        int waterOpen = 0;
        for (int i = 0; i < n; i++)
        {
            int height = Math.Max(0, values[i]);

            //Closed drains
            int waterLevel = Math.Min(leftClosed[i], rightClosed[i]);
            // Water trapped at this square
            waterClosed += Math.Max(0, waterLevel - height);

            // Open drains
            int levelOpen = Math.Min(leftOpen[i], rightOpen[i]);
            waterOpen += Math.Max(0, levelOpen - height);

        }

        return new Output
        { 
            Closed = waterClosed,
            Open = waterOpen
        };
    }

	/// <summary>
	/// This is a helper method to add artificial jitter to thread timing.
	/// </summary>
	/// <param name="baseTimeMs">Constant time delay in ms.</param>
	/// <param name="randomTimeMs">Maximum range for additional random time delay in ms.</param>
	private void Jitter(int baseTimeMs, int randomTimeMs)
	{
		Thread.Sleep(baseTimeMs + Random.Shared.Next(randomTimeMs));
	}
}