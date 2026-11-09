namespace Tests;

using NUnit;
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
		//take inputs and outputs for the solution
		var inputs = Solution.TestData.Inputs;
		var outputs = Solution.TestData.Outputs;

		//pair inputs with outputs
		var inouts = 
			inputs
				.Zip(outputs, (inp, outp) => new { inp = inp, outp = outp})
				.ToList();
		
		//test each pair
		inouts
			.Select((inout, index) => new {inout = inout, index = index}).ToList()
			.ForEach(it => {
				var input = it.inout.inp;
				var expectedOutput = it.inout.outp;
				var index = it.index;

				//run test multiple times, if necessary, for the issue to manifest itself
				for( var i = 0; i < 500; i++ )
				{
					var task = new Solution();
					var output = task.Run(input);

					try 
					{
                        Assert.Multiple(() =>
                        {
                            Assert.That(output.Closed, Is.EqualTo(expectedOutput.Closed),
                                $"Closed result is incorrect at test data index {index}, iteration {i}.");

                            Assert.That(output.Open, Is.EqualTo(expectedOutput.Open),
                                $"Open result is incorrect at test data index {index}, iteration {i}.");
                        });
                    }
					catch
					{
						//place a breakpoint here to catch the iteration that fails assertion
						throw;
					}
				}				
			});
	}
}