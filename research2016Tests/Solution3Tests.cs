using System;
using System.Linq;
using Solution3;
using Xunit;

namespace research2016Tests
{
	public class Solution3Tests
	{
		[Fact]
		public void WareSalesStats_mean_max_variance_correct()
		{
			Random rng = new Random();
			var stats = new WareSalesStats();
			var items = Enumerable.Range(0, 150).Select(i =>
			{
				var dateStart = new DateTime(2015, 1, 1).AddMilliseconds(rng.NextDouble() * 30 * 24 * 60 * 60 * 10);
				var dateEnd = dateStart.AddMilliseconds(rng.NextDouble() * 10 * 30 * 24 * 60 * 60 * 10);
				return (dateEnd - dateStart).TotalMinutes;
			}).ToList();

			foreach (var item in items)
			{
				stats.AddSale(item);
			}

			double mean = items.Sum() / items.Count;
			var variance = items.Sum(x => Math.Pow(x - mean, 2))/(items.Count - 1);
			var deviation = Math.Sqrt(variance);

			Assert.Equal(Math.Round(items.Average(), 3), Math.Round(stats.Mean, 3));
			Assert.Equal(Math.Round(items.Max(), 3), Math.Round(stats.Max, 3));

			Assert.Equal(Math.Round(variance, 3), Math.Round(stats.Variance, 3));
			Assert.Equal(Math.Round(deviation, 3), Math.Round(stats.Deviation, 3));
		}
	}
}