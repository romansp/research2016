using System;
using System.Linq;
using Xunit;

namespace research2016Tests
{
	public class Solution1Tests
	{
		[Fact]
		public void Rational_is_in_System_namespace()
		{
			Assert.True(typeof(Rational).Namespace == typeof(string).Namespace);
		}

		[Fact]
		public void Rational_is_value_type()
		{
			Assert.True(typeof(Rational).IsValueType);
		}

		[Theory]
		[InlineData(0, 0)]
		[InlineData(1, 0)]
		[InlineData(-1, 0)]
		public void Rational_incorrect_arguments_throws_ArgumentException(int num, int denom)
		{
			Assert.Throws<ArgumentException>(() => new Rational(num, denom));
		}

		[Theory]
		[InlineData(2, 3, "2/3")]
		[InlineData(-2, 4, "-2/4")]
		[InlineData(1, -2, "1/-2")]
		[InlineData(-2, -2, "-2/-2")]
		[InlineData(0, 1, "0")]
		[InlineData(42, 1, "42")]
		public void Rational_ToString_formats(int num, int denom, string formatted)
		{
			var rational = new Rational(num, denom);
			Assert.True(Equals(rational.ToString(), formatted));
		}

		[Theory]
		[InlineData(-2, 4, -1, 2)]
		[InlineData(-2, -4, 1, 2)]
		[InlineData(2, -4, -1, 2)]
		[InlineData(2, 4, 1, 2)]
		[InlineData(42, 2, 21, 1)]
		[InlineData(0, 1, 0, 1)]
		[InlineData(int.MinValue, int.MinValue, 1, 1)]
		[InlineData(int.MaxValue, int.MaxValue, 1, 1)]
		[InlineData(int.MinValue, int.MaxValue, int.MinValue, int.MaxValue)]
		[InlineData(int.MaxValue, int.MinValue, int.MaxValue, int.MinValue)]
		public void Rational_Simplify(int num, int denom, int numSimpl, int denomSimpl)
		{
			var rationalSimpl = new Rational(num, denom).Simplify();
			Assert.Equal(numSimpl, rationalSimpl.Numerator);
			Assert.Equal(denomSimpl, rationalSimpl.Denominator);
		}

		[Theory]
		[InlineData(1, 2, 1, 2, "1")]
		public void Rational_Sum(int r1Num, int r1Denom, int r2Num, int r2Denom, string stringResult)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);

			var result = rational1 + rational2;

			Assert.Equal(stringResult, result.ToString());
		}

		[Theory]
		[InlineData(int.MaxValue, 2, int.MaxValue, 2)]
		[InlineData(int.MaxValue, 2, 1, 2)]
		[InlineData(1, 2, int.MaxValue, 2)]
		public void Rational_OverflowSum_OverflowException(int r1Num, int r1Denom, int r2Num, int r2Denom)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);
			Assert.Throws<OverflowException>(() => rational1 + rational2);
		}

		[Theory]
		[InlineData(1, 2, 1, 2, "0")]
		[InlineData(1, 2, 2, 3, "-1/6")]
		public void Rational_Substract(int r1Num, int r1Denom, int r2Num, int r2Denom, string stringResult)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);

			var result = rational1 - rational2;

			Assert.Equal(stringResult, result.ToString());
		}

		[Theory]
		[InlineData(int.MinValue, 2, int.MaxValue, 2)]
		[InlineData(int.MinValue, 2, 1, 2)]
		public void Rational_OverflowSubstract_OverflowException(int r1Num, int r1Denom, int r2Num, int r2Denom)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);
			Assert.Throws<OverflowException>(() => rational1 - rational2);
		}

		[Theory]
		[InlineData(1, 2, "-1/2")]
		[InlineData(-3, 2, "3/2")]
		[InlineData(-4, 2, "2")]
		[InlineData(int.MaxValue, 2, "-2147483647/2")]
		public void Rational_InverseSum(int r1Num, int r1Denom, string stringResult)
		{
			var rational1 = new Rational(r1Num, r1Denom);

			var result = -rational1;

			Assert.Equal(stringResult, result.ToString());
		}

		[Theory]
		[InlineData(int.MinValue, 2)]
		public void Rational_OverflowInverseSum_OverflowException(int r1Num, int r1Denom)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			Assert.Throws<OverflowException>(() => -rational1);
		}

		[Theory]
		[InlineData(1, 2, 1, 2, "1/4")]
		[InlineData(1, 2, 2, 3, "1/3")]
		public void Rational_Multiply(int r1Num, int r1Denom, int r2Num, int r2Denom, string stringResult)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);

			var result = rational1 * rational2;

			Assert.Equal(stringResult, result.ToString());
		}

		[Theory]
		[InlineData(int.MaxValue, 2, int.MaxValue, 2)]
		[InlineData(int.MinValue, 2, int.MinValue, 2)]
		[InlineData(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue)]
		[InlineData(int.MinValue, int.MinValue, int.MinValue, int.MinValue)]
		public void Rational_OverflowMultiply_OverflowException(int r1Num, int r1Denom, int r2Num, int r2Denom)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);
			Assert.Throws<OverflowException>(() => rational1 * rational2);
		}

		[Theory]
		[InlineData(1, 2, 1, 2, "1")]
		[InlineData(1, 2, 1, 4, "2")]
		public void Rational_Divide(int r1Num, int r1Denom, int r2Num, int r2Denom, string stringResult)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);

			var result = rational1 / rational2;

			Assert.Equal(stringResult, result.ToString());
		}

		[Theory]
		[InlineData(int.MaxValue, 2, int.MaxValue, 2)]
		[InlineData(int.MinValue, 2, int.MinValue, 2)]
		[InlineData(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue)]
		[InlineData(int.MinValue, int.MinValue, int.MinValue, int.MinValue)]
		public void Rational_OverflowDivide_OverflowException(int r1Num, int r1Denom, int r2Num, int r2Denom)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);
			Assert.Throws<OverflowException>(() => rational1 / rational2);
		}

		[Theory]
		[InlineData(1, 2, 1, 2, true)]
		[InlineData(2, 4, 1, 2, true)]
		[InlineData(4, 2, 2, 1, true)]
		[InlineData(0, 1, 0, 40, true)]
		[InlineData(int.MinValue, int.MinValue, int.MinValue, int.MinValue, true)]
		[InlineData(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, true)]
		[InlineData(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue, true)]
		[InlineData(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue, true)]
		[InlineData(int.MinValue, int.MaxValue, int.MinValue, int.MaxValue, true)]
		[InlineData(int.MaxValue, int.MinValue, int.MaxValue, int.MinValue, true)]
		[InlineData(1, 2, 1, 3, false)]
		[InlineData(-2, 4, 1, 2, false)]
		[InlineData(4, 2, -2, 1, false)]
		[InlineData(int.MaxValue, int.MinValue, int.MinValue, int.MaxValue, false)]
		[InlineData(int.MinValue, int.MaxValue, int.MaxValue, int.MinValue, false)]

		[InlineData(679277, 679279, 679277, 679279, true)]
		[InlineData(679279, 679277, 679277, 679279, false)]
		[InlineData(679279, 679277, 679297, 679277, false)]
		[InlineData(679297, 679277, 679279, 679277, false)]

		[InlineData(1, -2, -2, 1, false)]
		public void Rational_comparison_equal(int r1Num, int r1Denom, int r2Num, int r2Denom, bool expected)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);

			Assert.Equal(expected, rational1 == rational2);
		}

		[Theory]
		[InlineData(1, 2, 1, 2, false)]
		[InlineData(2, 4, 1, 2, false)]
		[InlineData(4, 2, 2, 1, false)]
		[InlineData(0, 1, 0, 40, false)]
		[InlineData(int.MinValue, int.MinValue, int.MinValue, int.MinValue, false)]
		[InlineData(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, false)]
		[InlineData(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue, false)]
		[InlineData(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue, false)]
		[InlineData(int.MinValue, int.MaxValue, int.MinValue, int.MaxValue, false)]
		[InlineData(int.MaxValue, int.MinValue, int.MaxValue, int.MinValue, false)]
		[InlineData(1, 2, 1, 3, true)]
		[InlineData(-2, 4, 1, 2, true)]
		[InlineData(4, 2, -2, 1, true)]
		[InlineData(int.MaxValue, int.MinValue, int.MinValue, int.MaxValue, true)]
		[InlineData(int.MinValue, int.MaxValue, int.MaxValue, int.MinValue, true)]

		[InlineData(679277, 679279, 679277, 679279, false)]
		[InlineData(679279, 679277, 679277, 679279, true)]
		[InlineData(679279, 679277, 679297, 679277, true)]
		[InlineData(679297, 679277, 679279, 679277, true)]

		[InlineData(1, -2, -2, 1, true)]
		public void Rational_comparison_notequal(int r1Num, int r1Denom, int r2Num, int r2Denom, bool expected)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);

			Assert.Equal(expected, rational1 != rational2);
		}

		[Theory]
		[InlineData(1, 2, 1, 2, false)]
		[InlineData(2, 4, 1, 2, false)]
		[InlineData(4, 2, 2, 1, false)]
		[InlineData(0, 1, 0, 40, false)]
		[InlineData(int.MinValue, int.MinValue, int.MinValue, int.MinValue, false)]
		[InlineData(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, false)]
		[InlineData(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue, false)]
		[InlineData(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue, false)]
		[InlineData(int.MinValue, int.MaxValue, int.MinValue, int.MaxValue, false)]
		[InlineData(int.MaxValue, int.MinValue, int.MaxValue, int.MinValue, false)]
		[InlineData(1, 2, 1, 3, false)]
		[InlineData(-2, 4, 1, 2, true)]
		[InlineData(4, 2, -2, 1, false)]
		[InlineData(int.MaxValue, int.MinValue, int.MinValue, int.MaxValue, false)]
		[InlineData(int.MinValue, int.MaxValue, int.MaxValue, int.MinValue, true)]

		[InlineData(679277, 679279, 679277, 679279, false)]
		[InlineData(679279, 679277, 679277, 679279, false)]
		[InlineData(679279, 679277, 679297, 679277, true)]
		[InlineData(679297, 679277, 679279, 679277, false)]

		[InlineData(1, -2, -2, 1, false)]
		public void Rational_comparison_less(int r1Num, int r1Denom, int r2Num, int r2Denom, bool expected)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);

			Assert.Equal(expected, rational1 < rational2);
		}

		[Theory]
		[InlineData(1, 2, 1, 2, true)]
		[InlineData(2, 4, 1, 2, true)]
		[InlineData(4, 2, 2, 1, true)]
		[InlineData(0, 1, 0, 40, true)]
		[InlineData(int.MinValue, int.MinValue, int.MinValue, int.MinValue, true)]
		[InlineData(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, true)]
		[InlineData(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue, true)]
		[InlineData(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue, true)]
		[InlineData(int.MinValue, int.MaxValue, int.MinValue, int.MaxValue, true)]
		[InlineData(int.MaxValue, int.MinValue, int.MaxValue, int.MinValue, true)]
		[InlineData(1, 2, 1, 3, false)]
		[InlineData(-2, 4, 1, 2, true)]
		[InlineData(4, 2, -2, 1, false)]
		[InlineData(int.MaxValue, int.MinValue, int.MinValue, int.MaxValue, false)]
		[InlineData(int.MinValue, int.MaxValue, int.MaxValue, int.MinValue, true)]

		[InlineData(679277, 679279, 679277, 679279, true)]
		[InlineData(679279, 679277, 679277, 679279, false)]
		[InlineData(679279, 679277, 679297, 679277, true)]
		[InlineData(679297, 679277, 679279, 679277, false)]

		[InlineData(1, -2, -2, 1, false)]
		[InlineData(-1, 2, 2, -1, false)]
		public void Rational_comparison_lessOrEqual(int r1Num, int r1Denom, int r2Num, int r2Denom, bool expected)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);

			Assert.Equal(expected, rational1 <= rational2);
		}

		[Theory]
		[InlineData(1, 2, 1, 2, false)]
		[InlineData(2, 4, 1, 2, false)]
		[InlineData(4, 2, 2, 1, false)]
		[InlineData(0, 1, 0, 40, false)]
		[InlineData(int.MinValue, int.MinValue, int.MinValue, int.MinValue, false)]
		[InlineData(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, false)]
		[InlineData(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue, false)]
		[InlineData(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue, false)]
		[InlineData(int.MinValue, int.MaxValue, int.MinValue, int.MaxValue, false)]
		[InlineData(int.MaxValue, int.MinValue, int.MaxValue, int.MinValue, false)]
		[InlineData(1, 2, 1, 3, true)]
		[InlineData(-2, 4, 1, 2, false)]
		[InlineData(4, 2, -2, 1, true)]
		[InlineData(int.MaxValue, int.MinValue, int.MinValue, int.MaxValue, true)]
		[InlineData(int.MinValue, int.MaxValue, int.MaxValue, int.MinValue, false)]

		[InlineData(679277, 679279, 679277, 679279, false)]
		[InlineData(679279, 679277, 679277, 679279, true)]
		[InlineData(679279, 679277, 679297, 679277, false)]
		[InlineData(679297, 679277, 679279, 679277, true)]

		[InlineData(1, -2, -2, 1, true)]
		public void Rational_comparison_greater(int r1Num, int r1Denom, int r2Num, int r2Denom, bool expected)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);

			Assert.Equal(expected, rational1 > rational2);
		}

		[Theory]
		[InlineData(1, 2, 1, 2, true)]
		[InlineData(2, 4, 1, 2, true)]
		[InlineData(4, 2, 2, 1, true)]
		[InlineData(0, 1, 0, 40, true)]
		[InlineData(int.MinValue, int.MinValue, int.MinValue, int.MinValue, true)]
		[InlineData(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, true)]
		[InlineData(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue, true)]
		[InlineData(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue, true)]
		[InlineData(int.MinValue, int.MaxValue, int.MinValue, int.MaxValue, true)]
		[InlineData(int.MaxValue, int.MinValue, int.MaxValue, int.MinValue, true)]
		[InlineData(1, 2, 1, 3, true)]
		[InlineData(-2, 4, 1, 2, false)]
		[InlineData(4, 2, -2, 1, true)]
		[InlineData(int.MaxValue, int.MinValue, int.MinValue, int.MaxValue, true)]
		[InlineData(int.MinValue, int.MaxValue, int.MaxValue, int.MinValue, false)]

		[InlineData(679277, 679279, 679277, 679279, true)]
		[InlineData(679279, 679277, 679277, 679279, true)]
		[InlineData(679279, 679277, 679297, 679277, false)]
		[InlineData(679297, 679277, 679279, 679277, true)]

		[InlineData(1, -2, -2, 1, true)]
		public void Rational_comparison_greaterOrEqual(int r1Num, int r1Denom, int r2Num, int r2Denom, bool expected)
		{
			var rational1 = new Rational(r1Num, r1Denom);
			var rational2 = new Rational(r2Num, r2Denom);

			Assert.Equal(expected, rational1 >= rational2);
		}

		[Theory]
		[InlineData(1, 2, 0.5)]
		[InlineData(1, 3, ((double)1 / 3))]
		[InlineData(-1, 2, -0.5)]
		[InlineData(1, -3, ((double)-1 / 3))]
		[InlineData(0, 40, 0.0)]
		[InlineData(2, 3, ((double)2 / 3))]
		public void Rational_explicit_to_double(int r1Num, int r1Denom, double expected)
		{
			var rational1 = new Rational(r1Num, r1Denom);

			Assert.Equal(expected, (double)rational1);
		}

		[Theory]
		[InlineData(10, 10, 1)]
		[InlineData(-10, -10, 1)]
		[InlineData(0, 0, 1)]
		public void Rational_implicit_from_int(int denominator, int r1NumExpected, int r1DenomExpected)
		{
			Rational rational = denominator;

			Assert.Equal(r1NumExpected, rational.Numerator);
			Assert.Equal(r1DenomExpected, rational.Denominator);
		}

		[Fact]
		public void Rational_sorting()
		{
			var random = new Random();
			var rationals = Enumerable.Range(1, 50).Select(val => new Rational((random.Next(0, 2) * 2 - 1) * val, (random.Next(0, 2) * 2 - 1) * val ^ val + 1)).ToList();

			rationals.Sort();
			var isOrdered =
				rationals.Zip(rationals.Skip(1), (previous, current) => new { current, previous })
					.All(p => (double)p.previous < (double)p.current);

			Assert.True(isOrdered);
		}
	}
}