using System.Text;

namespace Solutions;

using NLog;


/// <summary>
/// Solution for the problem.
/// </summary>
public class Solution
{    
	private static readonly IReadOnlyDictionary<char, int> RomanValues =
		new Dictionary<char, int>
		{
			['I'] = 1, ['V'] = 5, ['X'] = 10, ['L'] = 50,
			['C'] = 100, ['D'] = 500, ['M'] = 1000
		};

	// Derived from RomanValues: Roman symbols are defined in one place only.
	private static readonly IReadOnlyDictionary<int, char> RomanSymbols =
		RomanValues.ToDictionary(pair => pair.Value, pair => pair.Key);

	/// <summary>
	/// A single input.
	/// </summary>
	public class Input
	{        
		public IReadOnlyList<string> RomanNumbers { get; init; } = Array.Empty<string>();
		public IReadOnlyList<int> DecimalNumbers { get; init; } = Array.Empty<int>();
	}

	/// <summary>
	/// A single output.
	/// </summary>
	public class Output
	{
		public IReadOnlyList<string> Lines { get; init; } = Array.Empty<string>();
	}

	/// <summary>
	/// Test data.
	/// </summary>
	public class TestData
	{        
		/// <summary>
		/// Array of test inputs.
		/// </summary>
		public static Input[] Inputs { get; } =
		[
			new Input
			{
				RomanNumbers = ["MCMLIV", "LXX", "DCLVIII", "MDCCCXLVI"],
				DecimalNumbers = [9, 2222, 751]
			},
			new Input
			{
				RomanNumbers = ["IV", "XLIX", "XCIX", "CDXLIV", "MCMXCIX"],
				DecimalNumbers = [1, 4, 44, 944, 2999]
			}
		];

		/// <summary>
		/// Array of test outputs in corresponding order to test inputs.
		/// </summary>    
		public static Output[] Outputs { get; } =
		[
			new Output
			{
				Lines =
				[
					"MCMLIV 1954", "LXX 70", "DCLVIII 658", "MDCCCXLVI 1846",
					"9 IX", "2222 MMCCXXII", "751 DCCLI"
				]
			},
			new Output
			{
				Lines =
				[
					"IV 4", "XLIX 49", "XCIX 99", "CDXLIV 444", "MCMXCIX 1999",
					"1 I", "4 IV", "44 XLIV", "944 CMXLIV", "2999 MMCMXCIX"
				]
			}
		];
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
		ArgumentNullException.ThrowIfNull(input);
		
		List<string> lines =
		[
			.. input.RomanNumbers.Select(roman => $"{roman} {RomanToDecimal(roman)}"),
			.. input.DecimalNumbers.Select(number => $"{number} {DecimalToRoman(number)}")
		];
		
		return new Output { Lines = lines };
	}

	private static int RomanToDecimal(string roman)
	{
		ArgumentException.ThrowIfNullOrEmpty(roman);
		var result = 0;

		for (var index = 0; index < roman.Length; index++)
		{
			if (!RomanValues.TryGetValue(roman[index], out var current))
				throw new ArgumentException($"Invalid Roman symbol: {roman[index]}", nameof(roman));

			var next = index + 1 < roman.Length ? RomanValues[roman[index + 1]] : 0;
			result += current < next ? -current : current;
		}

		return result;
	}

	private static string DecimalToRoman(int number)
	{
		if (number is < 1 or >= 3000)
			throw new ArgumentOutOfRangeException(nameof(number), "Number must be from 1 to 2999.");

		var result = new StringBuilder();
		result.Append(RomanSymbols[1000], number / 1000);
		AppendDigit(result, number / 100 % 10, 100, 500, 1000);
		AppendDigit(result, number / 10 % 10, 10, 50, 100);
		AppendDigit(result, number % 10, 1, 5, 10);

		return result.ToString();
	}

	// Forms one decimal position: 4 -> one + five, 9 -> one + ten.
	private static void AppendDigit(
		StringBuilder result, int digit,
		int one, int five, int ten)
	{
		var oneSymbol = RomanSymbols[one];
		var fiveSymbol = RomanSymbols[five];
		var tenSymbol = RomanSymbols[ten];

		switch (digit)
		{
			case <= 3:
				result.Append(oneSymbol, digit);
				break;
			case 4:
				result.Append(oneSymbol).Append(fiveSymbol);
				break;
			case <= 8:
				result.Append(fiveSymbol).Append(oneSymbol, digit - 5);
				break;
			default:
				result.Append(oneSymbol).Append(tenSymbol);
				break;
		}
	}
}
