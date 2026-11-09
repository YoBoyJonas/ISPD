namespace Tests;

using NUnit.Framework;

using Solutions;


/// <summary>
/// Unit test Solution.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SolutionTest
{
    /// <summary>Leistina slankaus kablelio paklaida lyginant perimetrus.</summary>
    const double Eps = 1e-6;

    /// <summary>Kiek kartų kartojamas kiekvienas įvesties atvejis, kad persidengimas pasireikštų.</summary>
    const int Iterations = 50;

    [Test]
    [NonParallelizable]
    public void Test()
    {
        //take inputs and outputs for the solution
        var inputs = Solution.TestData.Inputs;
        var outputs = Solution.TestData.Outputs;

        //pair inputs with outputs
        var inouts =
            inputs
                .Zip(outputs, (inp, outp) => new { inp = inp, outp = outp })
                .ToList();

        //test each pair
        inouts
            .Select((inout, index) => new { inout = inout, index = index }).ToList()
            .ForEach(it =>
            {
                var input = it.inout.inp;
                var expectedOutput = it.inout.outp;
                var index = it.index;

                //run test multiple times, if necessary, for the issue to manifest itself
                for (var i = 0; i < Iterations; i++)
                {
                    var task = new Solution();
                    var output = task.Run(input);

                    try
                    {
                        AssertColorResult(expectedOutput.Raudona, output.Raudona, "Raudona", index, i);
                        AssertColorResult(expectedOutput.Melyna, output.Melyna, "Melyna", index, i);
                        AssertColorResult(expectedOutput.Zalia, output.Zalia, "Zalia", index, i);
                    }
                    catch
                    {
                        //place a breakpoint here to catch the iteration that fails assertion
                        throw;
                    }
                }
            });
    }

    static void AssertColorResult(Solution.ColorResult expected, Solution.ColorResult actual, string colorName, int caseIndex, int iteration)
    {
        Assert.That(actual.Found, Is.EqualTo(expected.Found),
            $"[{colorName}, testo atvejis {caseIndex}, iteracija {iteration}] Found reikšmė neatitinka laukiamos.");

        if (expected.Found)
        {
            Assert.That(actual.Perimeter, Is.EqualTo(expected.Perimeter).Within(Eps),
                $"[{colorName}, testo atvejis {caseIndex}, iteracija {iteration}] Perimetras neatitinka laukiamo.");
        }
    }
}