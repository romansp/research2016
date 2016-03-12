using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Solution3
{
	public static class Generators
	{
		public static void GenerateWaresCSV(string waresFile)
		{
			const string AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz#@$^*()      ";
			Random rng = new Random();
			var lines =
				Enumerable.Range(0, 700000)
					.Select(index => index + "," + RandomStrings(AllowedChars, 1, 16, 1, rng).First())
					.ToList();
			lines.Shuffle();
			var columns = new[] { WaresFileColumns.ware_id, WaresFileColumns.ware_name };
			WriteCsv(waresFile, columns, lines);
		}

		public static void GenerateDispatchesCSV(IDictionary<int, string> wares, string dispatchesFile, string dateFormat)
		{
			Random rng = new Random();
			var lines = new List<string>();
			var index = 0;
			foreach (var ware in wares)
			{
				lines.AddRange(Enumerable.Range(0, rng.Next(1, 5)).Select(i =>
				{
					index++;
					var dateStart = new DateTime(2015, 1, 1).AddMilliseconds(rng.NextDouble() * 30 * 24 * 60 * 60 * 10);
					var dateEnd = dateStart.AddMilliseconds(rng.NextDouble() * 10 * 30 * 24 * 60 * 60 * 10);
					return string.Format("{0},{1},{2},{3}", index, ware.Key, dateStart.ToString(dateFormat), dateEnd.ToString(dateFormat));
				}));
			}
			lines.Shuffle();
			var columns = new[] { DispatchesFileColumns.id, DispatchesFileColumns.ware_id, DispatchesFileColumns.start_time, DispatchesFileColumns.end_time };
			WriteCsv(dispatchesFile, columns, lines);
		}

		public static void WriteCsv(string file, string[] columns, IList<string> lines)
		{
			lines.Insert(0, string.Join(new string(StringSplits.Comma), columns));
			File.WriteAllLines(file, lines, Encoding.UTF8);
		}

		private static IEnumerable<string> RandomStrings(
			string allowedChars,
			int minLength,
			int maxLength,
			int count,
			Random rng)
		{
			char[] chars = new char[maxLength];
			int setLength = allowedChars.Length;

			while (count-- > 0)
			{
				int length = rng.Next(minLength, maxLength + 1);

				for (int i = 0; i < length; ++i)
				{
					chars[i] = allowedChars[rng.Next(setLength)];
				}

				yield return new string(chars, 0, length);
			}
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			Random rng = new Random();
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}
