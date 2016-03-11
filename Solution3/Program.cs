using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Solution3
{
    public static class StringSplits
    {
        // to avoid boxing in string.Split()
        public static readonly char[] Comma = { ',' };
    }

    public static class WaresFileColumns
    {
        public static readonly string ware_id = "ware_id";
        public static readonly string ware_name = "ware_name";
    }

    public static class DispatchesFileColumns
    {
        public static readonly string id = "id";
        public static readonly string ware_id = "ware_id";
        public static readonly string start_time = "start_time";
        public static readonly string end_time = "end_time";
    }

	public class WareSalesStats
	{
		private double _oldM, _newM, _oldS, _newS;
		private long _salesTotal;
		private double _max;

		public int WareId { get; set; }
		public string WareName { get; set; }

		public long SalesTotal
		{
			get { return _salesTotal; }
		}

		public double Max
		{
			get { return _max; }
		}

		public double Mean
		{
			get { return SalesTotal > 0 ? _newM : 0.0; }
		}

		public double Variance
		{
			get { return SalesTotal > 1 ? _newS / (SalesTotal - 1) : 0.0; }
		}

		public double Deviation
		{
			get { return Math.Sqrt(Variance); }
		}

		public void AddSale(double statValue)
		{
			_salesTotal++;
			if (SalesTotal == 1)
			{
				_oldM = _newM = statValue;
				_oldS = 0.0;
				_max = statValue;
			}
			else
			{
				_newM = _oldM + (statValue - _oldM) / SalesTotal;
				_newS = _oldS + (statValue - _oldM) * (statValue - _newM);

				_oldM = _newM;
				_oldS = _newS;

				if (statValue > Max)
				{
					_max = statValue;
				}
			}
		}
	}

	public class Program
    {
        private const string DispatchesFile = "dispatches.txt";
        private const string WaresFile = "wares.txt";
        private const string ReportFile = "report.txt";
        private const string DateFormat = "dd.MM.yyyy HH:mm:ss.fff";
        private const string ReportHeader = "ware_id,ware_name,mean,deviation,max";
        private const string ReportDataFormat = "{0},{1},{2},{3},{4}";
        private const string DoubleFormat = "F3";
        
        public static void Main(string[] args)
        {
            //GenerateWaresCSV();

            var wares = LoadWares();

            //GenerateDispatchesCSV(wares);

            var salesData = BuildSalesData(wares);
            WriteReport(salesData);
        }

        private static void WriteReport(Dictionary<int, WareSalesStats> salesData)
        {
            var reportLines = new List<string> { ReportHeader };
            foreach (var salesInfo in salesData.OrderByDescending(c => c.Value.Mean))
            {
                var value = salesInfo.Value;
                reportLines.Add(string.Format(ReportDataFormat,
                    value.WareId.ToString(),
                    value.WareName.ToString(), 
                    value.Mean.ToString(DoubleFormat, CultureInfo.InvariantCulture),
                    value.Deviation.ToString(DoubleFormat, CultureInfo.InvariantCulture),
                    value.Max.ToString(DoubleFormat, CultureInfo.InvariantCulture)
                ));
            }
            File.WriteAllLines(ReportFile, reportLines);
        }

        private static Dictionary<int, WareSalesStats> BuildSalesData(IDictionary<int, string> wares)
        {
            // todo: producer - consumer pattern
            // producer: bulk read file lines and place strings into ConcurrentBag
            // consumer: read files from ConcurrentBag in while(!bag.IsCompleted) loop
			
            var isHeaderLine = true;
            var wareIdIndex = 1;
            var startTimeIndex = 2;
            var endTimeIndex = 3;

            var salesData = new Dictionary<int, WareSalesStats>();

			//var dispatches = File.ReadLines(DispatchesFile);
			using (var fileStream = new FileStream(DispatchesFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
			using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
			{ 
				string dispatchLine;
				while ((dispatchLine = streamReader.ReadLine()) != null)
				{
					if (isHeaderLine)
					{
						isHeaderLine = false;
						var headerColumns = dispatchLine.Split(StringSplits.Comma);

						if (headerColumns.Length == 4 &&
							string.Equals(headerColumns[1], DispatchesFileColumns.ware_id, StringComparison.OrdinalIgnoreCase) &&
							string.Equals(headerColumns[2], DispatchesFileColumns.start_time,
								StringComparison.OrdinalIgnoreCase))
						{
							// trust template
							continue;
						}
						// columns are not in template order
						for (var i = 0; i < headerColumns.Length; i++)
						{
							if (string.Equals(headerColumns[i], DispatchesFileColumns.ware_id, StringComparison.OrdinalIgnoreCase))
							{
								wareIdIndex = i;
							}
							else if (string.Equals(headerColumns[i], DispatchesFileColumns.start_time,
								StringComparison.OrdinalIgnoreCase))
							{
								startTimeIndex = i;
							}
							else if (string.Equals(headerColumns[i], DispatchesFileColumns.end_time,
								StringComparison.OrdinalIgnoreCase))
							{
								endTimeIndex = i;
							}
						}
					}
					else
					{
						var dispatchValues = dispatchLine.Split(StringSplits.Comma);
						var wareId = ParseIntFast(dispatchValues[wareIdIndex]);

						string wareName;
						if (wares.TryGetValue(wareId, out wareName))
						{
							WareSalesStats wareSales;
							if (!salesData.TryGetValue(wareId, out wareSales))
							{
								wareSales = new WareSalesStats
								{
									WareId = wareId,
									WareName = wareName
								};
								salesData.Add(wareId, wareSales);
							}

							var dispatchStartTime = dispatchValues[startTimeIndex];
							var dispatchEndTime = dispatchValues[endTimeIndex];
							var startTime = ParseDateTimeFast(dispatchStartTime, DateFormat);
							var endTime = ParseDateTimeFast(dispatchEndTime, DateFormat);

							var timeToProcess = (endTime - startTime).TotalMinutes;
							wareSales.AddSale(timeToProcess);
						}
					}
				}
			}
            return salesData;
        }

        private static IDictionary<int, string> LoadWares()
        {
            var waresFileLines = File.ReadAllLines(WaresFile);
            var headerColumns = waresFileLines[0].Split(StringSplits.Comma);
            int wareIdIndex = 0;
            int wareNameIndex = 1;

	        if (headerColumns.Length != 2 || !string.Equals(headerColumns[0], WaresFileColumns.ware_id, StringComparison.OrdinalIgnoreCase))
	        {
		        // columns are not in template order
		        for (int i = 0; i < headerColumns.Length; i++)
		        {
			        if (string.Equals(headerColumns[i], WaresFileColumns.ware_id, StringComparison.OrdinalIgnoreCase))
			        {
				        wareIdIndex = i;
			        }
			        else if (string.Equals(headerColumns[i], WaresFileColumns.ware_name, StringComparison.OrdinalIgnoreCase))
			        {
				        wareNameIndex = i;
			        }
		        }
	        }

	        var wares = new Dictionary<int, string>();
           	// skip header
			for (int i = 1; i < waresFileLines.Length; i++)
	        {
				var wareValues = waresFileLines[i].Split(StringSplits.Comma);
				var wareId = ParseIntFast(wareValues[wareIdIndex]);
				var wareName = wareValues[wareNameIndex];
				wares.Add(wareId, wareName);
			}
            return wares;
        }

        private static void GenerateWaresCSV()
        {
            const string AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz#@$^*()      ";
            Random rng = new Random();
	        var lines =
		        Enumerable.Range(0, 700000)
			        .Select(index => index + "," + RandomStrings(AllowedChars, 1, 16, 1, rng).First())
			        .ToList();
            lines.Shuffle();
            var columns = new[] { WaresFileColumns.ware_id, WaresFileColumns.ware_name };
            WriteCsv(WaresFile, columns, lines);
        }

        private static void GenerateDispatchesCSV(IDictionary<int, string> wares)
        {
            Random rng = new Random();
            var lines = new List<string>();
            var index = 0;
            foreach (var ware in wares)
            {
                lines.AddRange(Enumerable.Range(0, rng.Next(0, 5)).Select(i =>
                {
                    index++;
                    var dateStart = new DateTime(2015, 1, 1).AddMilliseconds(rng.NextDouble() * 30 * 24 * 60 * 60 * 10);
                    var dateEnd = dateStart.AddMilliseconds(rng.NextDouble() * 10 * 30 * 24 * 60 * 60 * 10);
                    return string.Format("{0},{1},{2},{3}", index, ware.Key, dateStart.ToString(DateFormat), dateEnd.ToString(DateFormat));
                }));
            }
            lines.Shuffle();
            var columns = new[] { DispatchesFileColumns.id, DispatchesFileColumns.ware_id, DispatchesFileColumns.start_time, DispatchesFileColumns.end_time };
            WriteCsv(DispatchesFile, columns, lines);
        }

		private static DateTime ParseDateTimeFast(string s, string dateFormat)
		{
			var year = 0;
			var month = 0;
			var day = 0;
			var hour = 0;
			var minute = 0;
			var second = 0;
			var millisecond = 0;

			for (var i = 0; i < dateFormat.Length; i++)
			{
				var c = s[i];
				switch (dateFormat[i])
				{
					case 'y':
						year = year * 10 + (c - '0');
						break;
					case 'M':
						month = month * 10 + (c - '0');
						break;
					case 'd':
						day = day * 10 + (c - '0');
						break;
					case 'H':
						hour = hour * 10 + (c - '0');
						break;
					case 'm':
						minute = minute * 10 + (c - '0');
						break;
					case 's':
						second = second * 10 + (c - '0');
						break;
					case 'f':
						millisecond = millisecond * 10 + (c - '0');
						break;
				}
			}

			return new DateTime(year, month, day, hour, minute, second, millisecond);
		}

		private static int ParseIntFast(string s)
        {
            int y = 0;
            for (int i = 0; i < s.Length; i++)
            {
                y = y * 10 + (s[i] - '0');
            }
            return y;
        }

        private static void WriteCsv(string file, string[] columns, IList<string> lines)
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
    }

    public static class Extensions
    {
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
