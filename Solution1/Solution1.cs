using System;

namespace System
{
	using Algorithms;

	public struct Rational : IEquatable<Rational>, IComparable<Rational>, IComparable
	{
		private readonly int _numerator;
		private readonly int _denominator;
		private readonly int _gcd;
		private readonly int _sign;

		public Rational(int numerator, int denominator)
		{
			if (denominator == 0)
				throw new ArgumentException("Denominator can't be equal to zero.", "denominator");

			_numerator = numerator;
			_denominator = denominator;
			_gcd = Algorithms.Gcd(_numerator, _denominator);
			_sign = numerator < 0 && denominator < 0 || numerator > 0 && denominator > 0 ? 1 : -1;
		}

		public int Numerator { get { return _numerator; } }
		public int Denominator { get { return _denominator; } }

		public Rational Simplify()
		{
			return new Rational(_numerator / _gcd, _denominator / _gcd);
		}

		public static Rational operator +(Rational r1, Rational r2)
		{
			checked
			{
				return (r1._denominator == r2._denominator
					? new Rational(r1._numerator + r2._numerator, r1._denominator)
					: new Rational(r1._numerator * r2._denominator + r2._numerator * r1._denominator, r1._denominator * r2._denominator)
				).Simplify();
			}
		}

		public static Rational operator -(Rational r1, Rational r2)
		{
			checked
			{
				return (r1._denominator == r2._denominator
					? new Rational(r1._numerator - r2._numerator, r1._denominator)
					: new Rational(r1._numerator * r2._denominator - r2._numerator * r1._denominator, r1._denominator * r2._denominator)
				).Simplify();
			}
		}

		public static Rational operator -(Rational r1)
		{
			checked
			{
				return new Rational(-r1._numerator, r1._denominator).Simplify();
			}
		}

		public static Rational operator *(Rational r1, Rational r2)
		{
			checked
			{
				return new Rational(r1._numerator * r2._numerator, r1._denominator * r2._denominator).Simplify();
			}
		}

		public static Rational operator /(Rational r1, Rational r2)
		{
			checked
			{
				return new Rational(r1._numerator * r2._denominator, r1._denominator / r2._numerator).Simplify();
			}
		}

		public static bool operator ==(Rational r1, Rational r2)
		{
			return r1.Equals(r2);
		}

		public static bool operator !=(Rational r1, Rational r2)
		{
			return !r1.Equals(r2);
		}

		public static bool operator >(Rational r1, Rational r2)
		{
			return r1.CompareTo(r2) > 0;
		}

		public static bool operator <(Rational r1, Rational r2)
		{
			return r1.CompareTo(r2) < 0;
		}

		public static bool operator >=(Rational r1, Rational r2)
		{
			return r1.CompareTo(r2) >= 0;
		}

		public static bool operator <=(Rational r1, Rational r2)
		{
			return r1.CompareTo(r2) <= 0;
		}


		public static explicit operator double(Rational r1)
		{
			return (double)r1._numerator / r1._denominator;
		}

		public static implicit operator Rational(int numerator)
		{
			return new Rational(numerator, 1);
		}

		public int CompareTo(Rational other)
		{
			if (Equals(other))
				return 0;
			checked
			{
				if (_sign != other._sign)
					return _sign;
				
				// store in long type to avoid overflow
				var left = (long)_numerator * other._denominator;
				var right = (long)other._numerator * _denominator;

				//if (_sign < 0 && other._sign < 0)
				//	return right.CompareTo(left); 

				return left.CompareTo(right);
			}
		}

		public int CompareTo(object obj)
		{
			if (ReferenceEquals(obj, null))
				return 1;
			if (!(obj is Rational)) throw new ArgumentException("Object is not a Rational", "obj");
			return CompareTo((Rational)obj);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Rational && Equals((Rational)obj);
		}

		public bool Equals(Rational other)
		{
			if (_numerator == 0)
			{
				return other._numerator == 0;
			}
			// compare simplified rationals to avoid overflow
			return _numerator / _gcd == other._numerator / other._gcd && _denominator / _gcd == other._denominator / other._gcd;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_numerator / _gcd * 397) ^ (_denominator / _gcd);
			}
		}

		public override string ToString()
		{
			if (_numerator == 0) return "0";
			if (_denominator == 1) return _numerator.ToString();
			return _numerator.ToString() + "/" + _denominator.ToString();
		}
	}
}

namespace Algorithms
{
	public static class Algorithms
	{
		/// <summary>
		/// Finds the greatest common divisor of two int values.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int Gcd(int a, int b)
		{
			while (b != 0)
			{
				var temp = b;
				b = a % b;
				a = temp;
			}
			return a;
		}

		/// <summary>
		/// Finds the least common multiple of two int values.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int Lcm(int a, int b)
		{
			return a * b / Gcd(a, b);
		}
	}

}

namespace research2016
{
	class Program
	{
		static void Main(string[] args)
		{
		}
	}
}
