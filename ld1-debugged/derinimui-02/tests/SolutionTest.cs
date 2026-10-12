#nullable enable

namespace Tests;

using NUnit.Framework;
using Solutions;


[TestFixture]
public class SolutionTest
{
	[Test]
	public void Run_MatchesTaskStatementExample()
	{
		var solution = new Solution();

		var actualOutput = solution.Run(Solution.TestData.Inputs[0]);

		Assert.That(actualOutput, Is.EqualTo(Solution.TestData.Outputs[0]));
	}

	[Test]
	public void Run_ReportsFirstInvalidLetterAlphabetically()
	{
		var solution = new Solution();
		var input = new Solution.Input("AB", "DDCC");

		var actualOutput = solution.Run(input);

		Assert.That(
			actualOutput.Results,
			Is.EqualTo(new[]
			{
				new Solution.WordResult(
					"DDCC",
					Solution.WordStatus.MissingLetter,
					'C')
			}));
	}
}
