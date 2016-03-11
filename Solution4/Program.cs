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

		private static readonly Regex Validator = new Regex(@"^([a-zA-Zа-яА-Я0-9_])+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static string ParseWord(string str)
		{
			var wordMatch = Validator.Match(str);
			return wordMatch.Success ? wordMatch.Groups[0].Value : string.Empty;
		}

		public IEnumerable<HashSet<int>> Execute()
		{
			// prepare text
			for (var lineIndex = 0; lineIndex < _textFileLines.Length; lineIndex++)
			{
				var line = _textFileLines[lineIndex];
				var words = line.Split(StringSplits.Space);
				for (var j = 0; j < words.Length; j++)
				{
					var word = ParseWord(words[j]);
					if (string.IsNullOrEmpty(word))
					{
						continue;
					}
					HashSet<int> wordLineIndexes;
					if (!_textWordDictionary.TryGetValue(word, out wordLineIndexes))
					{
						wordLineIndexes = new HashSet<int> { lineIndex };
						_textWordDictionary.Add(word, wordLineIndexes);
					}
					else
					{
						wordLineIndexes.Add(lineIndex);
					}
				}
			}

			// run queries
			return RunQueries();
		}

	
		private IEnumerable<HashSet<int>> RunQueries()
		{
			for (var i = 0; i < _queries.Length; i++)
			{
				var query = _queries[i];
				if (query.Contains(' '))
				{
					var words = query.Split(StringSplits.Space);
					yield return RunMultiwordQuery(words);
				}
				else
				{
					yield return RunQuery(query);
				}
			}
		}

		private HashSet<int> RunQuery(string query)
		{
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

		private HashSet<int> RunMultiwordQuery(string[] queryWords)
		{
			var multiwordQueryResult = new HashSet<int>();
			for (int i = 0; i < queryWords.Length; i++)
			{
				var query = queryWords[i];
				HashSet<int> cachedResult;
				HashSet<int> linesWithWord;
				if (_queryCache.TryGetValue(query, out cachedResult))
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
				else if (_textWordDictionary.TryGetValue(query, out linesWithWord))
				{
					_queryCache.Add(query, linesWithWord);
				}
				else
				{
					// missed
					_queryCache.Add(query, EmptyIntHashSet);
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
