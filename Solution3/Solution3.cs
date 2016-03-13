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

		public string WareId { get; set; }
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
        private const string DoubleFormat = "0.##0";
        
        public static void Main(string[] args)
        {
            //Generators.GenerateWaresCSV(WaresFile);

            var wares = LoadWares();

			//Generators.GenerateDispatchesCSV(wares, DispatchesFile, DateFormat);

			var salesData = BuildSalesData(wares);
            WriteReport(salesData);
        }

        private static void WriteReport(IDictionary<int, WareSalesStats> salesData)
        {
	        var reportLines = new StringBuilder();
	        reportLines.AppendLine(ReportHeader);
	        var format = ReportDataFormat + Environment.NewLine;
            foreach (var salesInfo in salesData.OrderByDescending(c => c.Value.Mean))
            {
                var value = salesInfo.Value;
                reportLines.AppendFormat(format,
                    value.WareId,
                    value.WareName, 
                    value.Mean.ToString(DoubleFormat, CultureInfo.InvariantCulture),
                    value.Deviation.ToString(DoubleFormat, CultureInfo.InvariantCulture),
                    value.Max.ToString(DoubleFormat, CultureInfo.InvariantCulture)
                );
            }
            File.WriteAllText(ReportFile, reportLines.ToString());
        }

        private static IDictionary<int, WareSalesStats> BuildSalesData(IDictionary<int, string> wares)
        {
            var isHeaderLine = true;
            var wareIdIndex = 1;
            var startTimeIndex = 2;
            var endTimeIndex = 3;

			var salesData = new Dictionary<int, WareSalesStats>();

			foreach (var line in File.ReadLines(DispatchesFile))
			{
				if (isHeaderLine)
				{
					isHeaderLine = false;
					var headerColumns = line.Split(StringSplits.Comma);

					if (headerColumns.Length == 4 &&
						string.Equals(headerColumns[1], DispatchesFileColumns.ware_id, StringComparison.OrdinalIgnoreCase) &&
						string.Equals(headerColumns[2], DispatchesFileColumns.start_time, StringComparison.OrdinalIgnoreCase))
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
						else if (string.Equals(headerColumns[i], DispatchesFileColumns.start_time, StringComparison.OrdinalIgnoreCase))
						{
							startTimeIndex = i;
						}
						else if (string.Equals(headerColumns[i], DispatchesFileColumns.end_time, StringComparison.OrdinalIgnoreCase))
						{
							endTimeIndex = i;
						}
					}
				}
				else
				{
					var dispatchValues = line.Split(StringSplits.Comma);
					var wareIdString = dispatchValues[wareIdIndex];
					var wareId = ParseIntFast(wareIdString);

					string wareName;
					if (!wares.TryGetValue(wareId, out wareName))
						continue;
					WareSalesStats wareSalesStats;
					if (!salesData.TryGetValue(wareId, out wareSalesStats))
					{
						wareSalesStats = new WareSalesStats
						{
							WareId = wareIdString,
							WareName = wareName
						};
						salesData.Add(wareId, wareSalesStats);
					}

					var dispatchStartTime = dispatchValues[startTimeIndex];
					var dispatchEndTime = dispatchValues[endTimeIndex];
					var startTime = ParseDateTimeFast(dispatchStartTime, DateFormat);
					var endTime = ParseDateTimeFast(dispatchEndTime, DateFormat);

					var timeToProcess = (endTime - startTime).TotalMinutes;
					wareSalesStats.AddSale(timeToProcess);
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
            for (var i = 0; i < s.Length; i++)
            {
                y = y * 10 + (s[i] - '0');
            }
            return y;
        }
    }
}
