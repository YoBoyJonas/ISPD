namespace Tests;

using NUnit.Framework;

using Solutions;

[TestFixture]
public class SolutionTest
{
	[Test, Timeout(20_000)]
	public void LightDirectionDoesNotHideHeavyDirection()
	{
		// 500 000 columns of 3 points: only 3 row groups but 500 000 column groups,
		// so the column thread needs far longer (grouping, sorting, building) than the row thread.
		var points = Enumerable.Range(0, 500_000).SelectMany(x => Enumerable.Range(0, 3).Select(y => (x, y))).ToArray();

		var text = new Solution().Run(new Solution.Input(points)).Text;

		Assert.That(text.Split("kiekis").Length - 1, Is.EqualTo(500_003));
	}
}
