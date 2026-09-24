namespace Tests;

using NUnit.Framework;

using Solutions;


/// <summary>
/// Unit tests for segment search, using the task data.
/// </summary>
[TestFixture]
public class SolutionTest
{
	static Solution.Input TaskData => Solution.TestData.Inputs[0];

	// "kiekis N ilgis M" part of each segment line, per block, order independent.
	static string[][] Stats(Solution.Output o) =>
		o.Text.Split("\n\n").Select(block =>
			block.Split('\n').Select(l => l[l.IndexOf("kiekis")..]).OrderBy(s => s).ToArray()).ToArray();

	[Test]
	public void MirroredTaskDataGivesSameSegmentSizes()
	{
		var mirrored = new Solution.Input(TaskData.Points.Select(p => (-p.X, -p.Y)).ToArray());

		var expected = Stats(new Solution().Run(TaskData));
		var actual = Stats(new Solution().Run(mirrored));

		Assert.That(actual, Is.EqualTo(expected));
	}

	[Test]
	public void DroppingOnePointOfFourPointLineKeepsSegmentCount()
	{
		// (4, 8) is one of four points on x = 4; three are still a segment.
		var fewer = new Solution.Input(TaskData.Points.Where(p => p != (4, 8)).ToArray());

		var expected = Stats(new Solution().Run(TaskData));
		var actual = Stats(new Solution().Run(fewer));

		Assert.That(actual.Select(b => b.Length), Is.EqualTo(expected.Select(b => b.Length)));
	}
}
