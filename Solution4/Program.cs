using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Solution4
{
	public static class StringSplits
	{
		// to avoid boxing in string.Split()
		public static readonly char[]
			Space = {' '};
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			var textLines = File.ReadAllLines(args[0]);
			var queries = File.ReadAllLines(args[1]);

			var queryResult = new TextSearch(textLines, queries).Execute();

			var result = BuildResultString(queryResult);
			Console.Write(result);
		}

		public static string BuildResultString(IEnumerable<HashSet<int>> queryResult)
		{
			var result = new StringBuilder();
			foreach (var item in queryResult)
			{
				if (item.Any())
				{
					result.AppendLine(string.Join(",", item.Take(20)));
				}
				else
				{
					result.AppendLine();
				}
			}
			var resultString = result.ToString();

			return !string.IsNullOrEmpty(resultString) && resultString.Length >= Environment.NewLine.Length
				? resultString.Remove(resultString.Length - Environment.NewLine.Length, Environment.NewLine.Length)
				: resultString;
		}
	}

	public class TextSearch
	{
		private static readonly HashSet<int> EmptyIntHashSet = new HashSet<int>();
		
		private readonly string[] _textFileLines;
		private readonly string[] _queries;

		private readonly Dictionary<string, HashSet<int>> _queryCache =
			new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

		private readonly Dictionary<string, HashSet<int>> _textWordDictionary =
			new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);
		
		public TextSearch(string[] textFileLines, string[] queries)
		{
			_textFileLines = textFileLines;
			_queries = queries;
		}

		private static readonly Regex Parser = new Regex(@"([a-zA-Zа-яА-Я0-9_]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static MatchCollection ParseWords(string str)
		{
			var wordMatch = Parser.Matches(str);
			return wordMatch;
		}

		public IEnumerable<HashSet<int>> Execute()
		{
			PrepareText();
			return RunQueries();
		}

		private void PrepareText()
		{
			for (var lineIndex = 0; lineIndex < _textFileLines.Length; lineIndex++)
			{
				var line = _textFileLines[lineIndex];
				var words = ParseWords(line);
				for (var j = 0; j < words.Count; j++)
				{
					var word = words[j].Value;
					if (string.IsNullOrEmpty(word))
					{
						continue;
					}
					HashSet<int> wordLineIndexes;
					if (!_textWordDictionary.TryGetValue(word, out wordLineIndexes))
					{
						wordLineIndexes = new HashSet<int> {lineIndex};
						_textWordDictionary.Add(word, wordLineIndexes);
					}
					else
					{
						wordLineIndexes.Add(lineIndex);
					}
				}
			}
		}

		private List<HashSet<int>> RunQueries()
		{
			var result = new List<HashSet<int>>();
			for (var i = 0; i < _queries.Length; i++)
			{
				var query = _queries[i];
				result.Add(query.Contains(' ') ? RunMultiwordQuery(query) : RunQuery(query));
			}
			return result;
		}

		private HashSet<int> RunQuery(string query)
		{
			if (string.IsNullOrEmpty(query))
				return EmptyIntHashSet;
			HashSet<int> cachedResult;
			if (_queryCache.TryGetValue(query, out cachedResult))
			{
				return cachedResult;
			}
			HashSet<int> result;
			if (_textWordDictionary.TryGetValue(query, out result))
			{
				_queryCache.Add(query, result);
				return result;
			}

			// missed
			_queryCache.Add(query, EmptyIntHashSet);
			return EmptyIntHashSet;
		}

		private HashSet<int> RunMultiwordQuery(string query)
		{
			var queryWords = query.Split(StringSplits.Space);
			var multiwordQueryResult = new HashSet<int>();
			for (int i = 0; i < queryWords.Length; i++)
			{
				var queryWord = queryWords[i];
				if (string.IsNullOrEmpty(queryWord))
					continue;
				HashSet<int> cachedResult;
				HashSet<int> linesWithWord;
				if (_queryCache.TryGetValue(queryWord, out cachedResult))
				{
					if (cachedResult.Any())
					{
						linesWithWord = cachedResult;
					}
					else
					{
						return EmptyIntHashSet;
					}
				}
				else if (_textWordDictionary.TryGetValue(queryWord, out linesWithWord))
				{
					_queryCache.Add(queryWord, linesWithWord);
				}
				else
				{
					// missed
					_queryCache.Add(queryWord, EmptyIntHashSet);
					return EmptyIntHashSet;
				}

				if (linesWithWord.Count == 0)
				{
					// not in text
					return EmptyIntHashSet;
				}
				if (multiwordQueryResult.Count == 0)
				{
					// first init
					multiwordQueryResult = linesWithWord;
				}
				else
				{
					if (!linesWithWord.Overlaps(multiwordQueryResult))
					{
						return EmptyIntHashSet;
					}
				}
			}

			return multiwordQueryResult;
		}
	}
}
