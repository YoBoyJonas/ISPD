#nullable enable

namespace Solutions;

using NLog;


public class Solution
{
	public enum WordStatus
	{
		Valid,
		MissingLetter,
		TooManyOccurrences
	}

	public class Input
	{
		public string SourceWord { get; init; } = string.Empty;

		public string[] CandidateWords { get; init; } = Array.Empty<string>();

		public Input()
		{
		}

		public Input(string sourceWord, params string[] candidateWords)
		{
			SourceWord = sourceWord;
			CandidateWords = candidateWords;
		}
	}

	public readonly record struct WordResult(
		string Word,
		WordStatus Status,
		char? ProblemLetter = null)
	{
		public override string ToString()
		{
			return Status switch
			{
				WordStatus.Valid => $"{Word}: tinkamas",
				WordStatus.MissingLetter => $"{Word}: netinkamas (nebuvo raidės {ProblemLetter})",
				WordStatus.TooManyOccurrences => $"{Word}: netinkamas (raidžių {ProblemLetter} per daug)",
				_ => throw new InvalidOperationException()
			};
		}
	}

	public class Output : IEquatable<Output>
	{
		public WordResult[] Results { get; init; } = Array.Empty<WordResult>();

		public Output()
		{
		}

		public Output(params WordResult[] results)
		{
			Results = results;
		}

		public bool Equals(Output? other)
		{
			return other is not null && Results.SequenceEqual(other.Results);
		}

		public override bool Equals(object? obj) => Equals(obj as Output);

		public override int GetHashCode()
		{
			var hash = new HashCode();
			foreach (var result in Results)
			{
				hash.Add(result);
			}
			return hash.ToHashCode();
		}

		public static bool operator ==(Output? left, Output? right) => Equals(left, right);

		public static bool operator !=(Output? left, Output? right) => !Equals(left, right);
	}

	public class TestData
	{
		public static Input[] Inputs { get; } =
		{
			new("PROGRAMUOTOJAS", "PROGRAMA", "PROGRAMUOTI", "SAGOS")
		};

		public static Output[] Outputs { get; } =
		{
			new(
				new("PROGRAMA", WordStatus.Valid),
				new("PROGRAMUOTI", WordStatus.MissingLetter, 'I'),
				new("SAGOS", WordStatus.TooManyOccurrences, 'S'))
		};
	}

	readonly Logger mLog = LogManager.GetCurrentClassLogger();

	public Output Run(Input input)
	{
		ValidateInput(input);

		var sourceCounts = CountLetters(input.SourceWord);
		var results = input.CandidateWords
			.Select(word => ValidateWord(word, sourceCounts))
			.ToArray();

		mLog.Debug("Validated {Count} words.", results.Length);
		return new Output(results);
	}

	static WordResult ValidateWord(
		string word,
		IReadOnlyDictionary<char, int> sourceCounts)
	{
		var candidateCounts = CountLetters(word);
		var orderedLetters = candidateCounts.Keys.OrderBy(letter => letter);

		foreach (var letter in orderedLetters)
		{
			var requiredCount = candidateCounts[letter];
			if (!sourceCounts.TryGetValue(letter, out var availableCount))
			{
				return new WordResult(word, WordStatus.MissingLetter, letter);
			}

			if (requiredCount > availableCount)
			{
				return new WordResult(word, WordStatus.TooManyOccurrences, letter);
			}
		}

		return new WordResult(word, WordStatus.Valid);
	}

	static Dictionary<char, int> CountLetters(string word)
	{
		var counts = new Dictionary<char, int>();
		foreach (var letter in word)
		{
			counts[letter] = counts.GetValueOrDefault(letter) + 1;
		}
		return counts;
	}

	static void ValidateInput(Input input)
	{
		ArgumentNullException.ThrowIfNull(input);

		if (!IsValidWord(input.SourceWord))
		{
			throw new ArgumentException("The source word must contain 1-50 uppercase Latin letters.", nameof(input));
		}

		if (input.CandidateWords.Length is < 1 or > 50
			|| input.CandidateWords.Any(word => !IsValidWord(word)))
		{
			throw new ArgumentException("There must be 1-50 valid candidate words.", nameof(input));
		}
	}

	static bool IsValidWord(string? word)
	{
		return word is { Length: >= 1 and <= 50 }
			&& word.All(letter => letter is >= 'A' and <= 'Z');
	}
}
