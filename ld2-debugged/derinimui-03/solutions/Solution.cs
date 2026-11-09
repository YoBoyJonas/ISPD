namespace Solutions;

using NLog;


/// <summary>
/// Solution for the problem.
/// </summary>
public class Solution
{
	/// <summary>
	/// A single input.
	/// </summary>
	public class Input
	{
		public string Name { get; init; } = "";

		/// <summary>
		/// Daily temperatures of the month.
		/// </summary>
		public int[] Temperatures { get; init; } = Array.Empty<int>();
	}

	/// <summary>
	/// A single output.
	/// </summary>
	public class Output
	{
		/// <summary>
		/// Temperatures of the found period.
		/// </summary>
		public int[] Temperatures { get; init; } = Array.Empty<int>();

		/// <summary>
		/// Average temperature of the found period.
		/// </summary>
		public double Average => Temperatures.Length == 0 ? 0.0 : Temperatures.Average();

		/// <summary>
		/// Temperatures of the period.
		/// </summary>
		public override string ToString() => string.Join(" ", Temperatures);
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
			new Input {
				Name = "1 menuo",
				Temperatures = new int[] { 1, 2, 5, -7, 7, 5, -8, 3, -4, 5, 6, 8, -1, 2, 3, 4, -5, 6, 9, 8, 2, -5, 3, 2, -3, 9, 0, 8, 9, 8 }
			},
			new Input {
				Name = "2 menuo",
				Temperatures = new int[] { -9, 3, 5, 5, -3, 2, 1, 3, 1, 2, 3, 8, -2, 8, 15, -2, 5, 4, 7, 4, -7, 5, 2, 3, -6, 4, 6, 4, 4, 1 }
			},
		};

		/// <summary>
		/// Array of test outputs in corresponding order to test inputs.
		/// </summary>
		public static Output[] Outputs { get; } = {
			new Output { Temperatures = new int[] { 9, 0, 8, 9, 8 } },
			new Output { Temperatures = new int[] { 8, 15 } },
		};
	}

	/// <summary>
	/// One continuous period of non negative temperatures.
	/// </summary>
	class Period
	{
		/// <summary>
		/// Index of the first day of the period.
		/// </summary>
		public int Start { get; init; }

		/// <summary>
		/// Length of the period in days.
		/// </summary>
		public int Length { get; init; }

		/// <summary>
		/// Average temperature of the period.
		/// </summary>
		public double Average { get; init; }
	}

	/// <summary>
	/// Bounded buffer of found periods (start, length), shared by producer and consumer threads.
	/// </summary>
	class PeriodBuffer
	{
		/// <summary>
		/// Lock guarding the buffer state.
		/// </summary>
		readonly object mLock = new object();

		/// <summary>
		/// Periods waiting to be evaluated.
		/// </summary>
		readonly Queue<(int Start, int Length)> mItems = new Queue<(int Start, int Length)>();

		/// <summary>
		/// Maximum number of periods in the buffer.
		/// </summary>
		readonly int mCapacity;

		/// <summary>
		/// Indicates that producer will not add more periods.
		/// </summary>
		bool mIsComplete = false;

		public PeriodBuffer(int capacity)
		{
			mCapacity = capacity;
		}

		/// <summary>
		/// Adds a period, waits while the buffer is full.
		/// </summary>
		public void Add((int Start, int Length) period)
		{
			lock (mLock)
			{
				while (mItems.Count >= mCapacity)
				{
					Monitor.Wait(mLock);
				}

				mItems.Enqueue(period);
				Monitor.PulseAll(mLock);
			}
		}

		/// <summary>
		/// Marks that no more periods will be added.
		/// </summary>
		public void Complete()
		{
			lock (mLock)
			{
				mIsComplete = true;
				Monitor.PulseAll(mLock);
			}
		}

		/// <summary>
		/// Takes a period, waits while the buffer is empty and not complete.
		/// </summary>
		/// <returns>False if the buffer is empty and complete.</returns>
		public bool TryTake(out (int Start, int Length) period)
		{
			lock (mLock)
			{
				while (mItems.Count == 0 && !mIsComplete)
				{
					Monitor.Wait(mLock);
				}

				if (mItems.Count == 0)
				{
					period = default;
					return false;
				}

				period = mItems.Dequeue();
				Monitor.PulseAll(mLock);
				return true;
			}
		}
	}

	/// <summary>
	/// Number of consumer threads evaluating periods.
	/// </summary>
	const int ConsumerCount = 3;

	/// <summary>
	/// Capacity of the period buffer.
	/// </summary>
	const int BufferCapacity = 4;

	/// <summary>
	/// Logger for this class.
	/// </summary>
	Logger mLog = LogManager.GetCurrentClassLogger();

	/// <summary>
	/// Lock guarding the best period.
	/// </summary>
	readonly object mBestLock = new object();

	/// <summary>
	/// Best period found so far, shared by consumer threads.
	/// </summary>
	Period? mBest = null;

	/// <summary>
	/// Runs the task solution.
	/// </summary>
	/// <param name="input">Input</param>
	/// <returns>Output</returns>
	public Output Run(Input input)
	{
		var temperatures = input.Temperatures;
		var buffer = new PeriodBuffer(BufferCapacity);

		mBest = null;

		var producer = new Thread(() => ProducePeriods(temperatures, buffer));
		var consumers =
			Enumerable.Range(0, ConsumerCount)
				.Select(_ => new Thread(() => ConsumePeriods(temperatures, buffer)))
				.ToList();

		producer.Start();
		consumers.ForEach(consumer => consumer.Start());

		producer.Join();
		consumers.ForEach(consumer => consumer.Join());

		var best = mBest;

		var output = best == null
			? new Output()
			: new Output { Temperatures = temperatures[best.Start..(best.Start + best.Length)] };

		mLog.Info($"{input.Name}: {output} (vidurkis {output.Average:0.###}).");

		return output;
	}

	/// <summary>
	/// Producer thread body. Finds continuous periods of non negative temperatures.
	/// </summary>
	void ProducePeriods(int[] temperatures, PeriodBuffer buffer)
	{
		int start = -1;

		for (int i = 0; i < temperatures.Length; i++)
		{
			if (temperatures[i] >= 0)
			{
				if (start < 0)
				{
					start = i;
				}
			}
			else if (start >= 0)
			{
				buffer.Add((start, i - start));
				start = -1;
			}
		}

		if (start >= 0)
		{
			buffer.Add((start, temperatures.Length - start));
		}

		buffer.Complete();
	}

	/// <summary>
	/// Consumer thread body. Evaluates periods and updates the best one.
	/// </summary>
	void ConsumePeriods(int[] temperatures, PeriodBuffer buffer)
	{
		while (buffer.TryTake(out var period))
		{
			var candidate = MakePeriod(temperatures, period.Start, period.Length);

			UpdateBest(candidate);
		}
	}

	/// <summary>
	/// Replaces the best period with the candidate if the candidate is better.
	/// </summary>
	void UpdateBest(Period candidate)
	{	
		lock (mBestLock)
		{
			var best = mBest;

			Jitter(1, 3);

			mBest = ChooseBetter(best, candidate);
		}
	}

	Period MakePeriod(int[] temperatures, int start, int length)
	{
		int sum = 0;

		for (int i = start; i < start + length; i++)
		{
			sum += temperatures[i];
		}

		double average = (double)sum / length;

		return new Period { Start = start, Length = length, Average = average };
	}

	Period ChooseBetter(Period? best, Period candidate)
	{
		//periods are evaluated in any order, so on equal averages the earlier period wins
		if (best == null || candidate.Average > best.Average)
		{
			return candidate;
		}

		return candidate.Average == best.Average && candidate.Start < best.Start ? candidate : best;
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
