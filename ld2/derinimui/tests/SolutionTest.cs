namespace Tests;

using NUnit.Framework;

using Solutions;


/// <summary>
/// Unit test Solution.
/// </summary>
[TestFixture] [NonParallelizable]
public class SolutionTest
{
	[Test] [NonParallelizable]
	public void Test()
	{
		//pair inputs with outputs
		var inouts =
			Solution.TestData.Inputs
				.Zip(Solution.TestData.Outputs, (inp, outp) => new { inp, outp })
				.ToList();

		//test each pair
		inouts
			.Select((inout, index) => new { inout, index }).ToList()
			.ForEach(it => {
				//run test multiple times, in case the issue only manifests sometimes
				for( var i = 0; i < 500; i++ )
				{
					var output = new Solution().Run(it.inout.inp);

					Assert.That(output, Is.EqualTo(it.inout.outp),
						$"Actual output is not matching expected output at test data index {it.index}, iteration {i}.");
				}
			});
	}
}
