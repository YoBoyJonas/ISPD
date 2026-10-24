namespace Tests;

using NUnit.Framework;

using Solutions;

[TestFixture]
public class SolutionTest
{
	[Test, Timeout(20_000)]
	public void RunWaitsForBothThreads()
	{
		for (int i = 0; i < Solution.TestData.Inputs.Length; i++) {
			var output = new Solution().Run(Solution.TestData.Inputs[i]);

			Assert.That(output, Is.EqualTo(Solution.TestData.Outputs[i]));
		}
	}
}
