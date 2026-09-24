namespace Solutions;

using NLog;


/// <summary>
/// Solution for the problem: find axis-parallel segments through 3+ points.
/// </summary>
public class Solution
{
	/// <summary>
	/// A single input.
	/// </summary>
	public record Input((int X, int Y)[] Points);

	/// <summary>
	/// A single output: X-parallel segments, blank line, Y-parallel segments.
	/// </summary>
	public record Output(string Text);

	/// <summary>
	/// Test data.
	/// </summary>
	public class TestData
	{
		/// <summary>
		/// Array of test inputs.
		/// </summary>
		public static Input[] Inputs { get; } = {
			new Input(new (int, int)[] {
				(8, 3), (2, 5), (2, 3), (-2, -2), (-2, 1), (-6, 2), (-6, -5), (-4, -3), (4, 8),
				(-3, -5), (-4, 7), (-3, 4), (1, 6), (1, 9), (4, 2), (-4, 1), (3, -5), (-4, 6),
				(1, -3), (4, 5), (6, -5), (0, 4), (-4, 0), (3, 4), (4, 4)
			}),
		};

		/// <summary>
		/// Array of test outputs in corresponding order to test inputs.
		/// </summary>
		public static Output[] Outputs { get; } = {
			new Output("-6 -5 6 -5 kiekis 4 ilgis 12\n-3 4 4 4 kiekis 4 ilgis 7\n\n-4 -3 -4 7 kiekis 5 ilgis 10\n1 -3 1 9 kiekis 3 ilgis 12\n4 2 4 8 kiekis 4 ilgis 6"),
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
		var alongX = Segments(input.Points, p => p.Y, p => p.X, (line, a, b) => (a, line, b, line));
		var alongY = Segments(input.Points, p => p.X, p => p.Y, (line, a, b) => (line, a, line, b));
		var text = Format(alongX) + "\n\n" + Format(alongY);
		mLog.Info(text);
		return new Output(text);
	}

	// Groups points by line, keeps lines with 3+ points, returns (x1, y1, x2, y2, count, length).
	static List<(int, int, int, int, int, int)> Segments(
		(int X, int Y)[] points, Func<(int X, int Y), int> line, Func<(int X, int Y), int> pos,
		Func<int, int, int, (int, int, int, int)> ends) =>
		points.GroupBy(line).Where(g => g.Count() > 3).OrderBy(g => g.Key)
			.Select(g => {
				int a = g.Min(pos), b = g.Aggregate(0, (m, p) => Math.Max(m, pos(p)));
				var (x1, y1, x2, y2) = ends(g.Key, a, b);
				return (x1, y1, x2, y2, g.Count(), b - a);
			}).ToList();

	static string Format(List<(int X1, int Y1, int X2, int Y2, int Count, int Length)> segs) =>
		segs.Count == 0
			? "Nera atkarpu."
			: string.Join("\n", segs.Select(s => $"{s.X1} {s.Y1} {s.X2} {s.Y2} kiekis {s.Count} ilgis {s.Length}"));
}
