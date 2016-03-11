using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Solution4
{
	public static class StringSplits
	{
		// to avoid boxing in string.Split()
		public static readonly char[]
			Comma = {','},
			Space = {' '};
	}

	class Program
	{
		static void Main(string[] args)
		{
		}
	}

	public static class Engine
	{
		private static readonly Dictionary<string, IEnumerable<int>> QueryCache =
			new Dictionary<string, IEnumerable<int>>(StringComparer.OrdinalIgnoreCase);

		private static readonly Dictionary<string, List<int>> TextDictionary =
			new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

		public static string QueryText(string[] textLines, string[] queries)
		{
			// prepare text
			for (int lineIndex = 0; lineIndex < textLines.Length; lineIndex++)
			{
				var line = textLines[lineIndex];
				var words = line.Split(StringSplits.Space);
				for (int j = 0; j < words.Length; j++)
				{
					var word = words[j];

					List<int> wordLineIndexes;
					if (!TextDictionary.TryGetValue(word, out wordLineIndexes))
					{
						wordLineIndexes = new List<int> { lineIndex };
						TextDictionary.Add(word, wordLineIndexes);
					}
					else
					{
						if (!wordLineIndexes.Contains(lineIndex))
						{
							wordLineIndexes.Add(lineIndex);
						}
					}
				}
			}
			// run queries
			var queryResult = RunQueries(queries);

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

			// remove single trailing newline
			return result.ToString().Remove(result.Length - Environment.NewLine.Length, Environment.NewLine.Length);
		}

		private static IEnumerable<IEnumerable<int>> RunQueries(string[] queries)
		{
			for (int i = 0; i < queries.Length; i++)
			{
				var query = queries[i];
				if (query.IndexOf(' ') >= 0)
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

		private static IEnumerable<int> RunQuery(string query)
		{
			IEnumerable<int> cachedResult;
			if (QueryCache.TryGetValue(query, out cachedResult))
			{
				return cachedResult;
			}
			List<int> result;
			if (TextDictionary.TryGetValue(query, out result))
			{
				QueryCache.Add(query, result);
				return result;
			}

			// missed
			QueryCache.Add(query, Enumerable.Empty<int>());
			return Enumerable.Empty<int>();
		}

		private static IEnumerable<int> RunMultiwordQuery(string[] queryWords)
		{
			return new List<int> {0, 1};
			//var multiwordQueryResult = new List<int>();
			//for (int i = 0; i < queryWords.Length; i++)
			//{
			//	var query = queryWords[i];
			//	IEnumerable<int> cachedResult;
			//	if (_queryCache.TryGetValue(query, out cachedResult))
			//	{
			//		return cachedResult;
			//	}
			//	List<int> result;
			//	if (_textDictionary.TryGetValue(query, out result))
			//	{
			//		_queryCache.Add(query, result);
			//		return result;
			//	}

			//	// missed
			//	_queryCache.Add(query, Enumerable.Empty<int>());
			//	return Enumerable.Empty<int>();
			//}
		}
	}
}
