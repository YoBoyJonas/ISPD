namespace Tests;

using NUnit;
using NUnit.Framework;

using Solutions;


/// <summary>
/// Unit tests for Metai Regionas.
/// </summary>
[TestFixture]
public class SolutionTest
{
	[Test]
	public void RomanNumbersAreConvertedFromContestData()
	{
		var output = new Solution().Run(Solution.TestData.Inputs[0]);

		Assert.That(output.Lines.Take(4),
			Is.EqualTo(Solution.TestData.Outputs[0].Lines.Take(4)));
	}

	[Test]
	public void DecimalNumbersAreConvertedFromContestData()
	{
		var output = new Solution().Run(Solution.TestData.Inputs[0]);

		Assert.That(output.Lines.Skip(4),
			Is.EqualTo(Solution.TestData.Outputs[0].Lines.Skip(4)));
	}
}
