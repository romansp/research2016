using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution5
{
	internal class ReverseComparer<T> : IComparer<T>
	{
		public int Compare(T x, T y)
		{
			return Comparer<T>.Default.Compare(y, x);
		}

		public static ReverseComparer<T> Instance = new ReverseComparer<T>();
	}

	internal class MaxHeap
	{
		private readonly List<int> _array = new List<int>();
		private long _sum = 0;

		public void Add(int element)
		{
			_sum += element;
			_array.Add(element);
			int c = _array.Count - 1;
			int parent = (c - 1);
			while (c > 0 && _array[c].CompareTo(_array[parent]) > 0)
			{
				int tmp = _array[c];
				_array[c] = _array[parent];
				_array[parent] = tmp;
				c = parent;
				parent = (c - 1);
			}
		}

		public int RemoveMax()
		{
			int ret = _array[0];
			_sum -= ret;
			_array[0] = _array[_array.Count - 1];
			_array.RemoveAt(_array.Count - 1);

			int c = 0;
			while (c < _array.Count)
			{
				int max = c;
				if (2 * c + 1 < _array.Count && _array[2 * c + 1].CompareTo(_array[max]) > 0)
					max = 2 * c + 1;
				if (2 * c + 2 < _array.Count && _array[2 * c + 2].CompareTo(_array[max]) > 0)
					max = 2 * c + 2;

				if (max == c)
					break;
				int tmp = _array[c];
				_array[c] = _array[max];
				_array[max] = tmp;
				c = max;
			}

			return ret;
		}

		public int Peek()
		{
			return _array[0];
		}

		public int Count
		{
			get
			{
				return _array.Count;
			}
		}

		public long Sum
		{
			get
			{
				return _sum;
			}
		}

		public int Search(int value)
		{
			return _array.BinarySearch(value, ReverseComparer<int>.Instance);
		}

		public void RemoveAt(int index)
		{
			int ret = _array[index];
			_sum -= ret;

			_array[index] = _array[_array.Count - 1 - index];
			_array.RemoveAt(_array.Count - 1 - index);

			int c = 0;
			while (c < _array.Count)
			{
				int max = c;
				if (2 * c + 1 < _array.Count && _array[2 * c + 1].CompareTo(_array[max]) > 0)
					max = 2 * c + 1;
				if (2 * c + 2 < _array.Count && _array[2 * c + 2].CompareTo(_array[max]) > 0)
					max = 2 * c + 2;

				if (max == c)
					break;
				int tmp = _array[c];
				_array[c] = _array[max];
				_array[max] = tmp;
				c = max;
			}
		}
	}

	internal class PriorityQueue
	{
		private readonly MaxHeap _maxHeap = new MaxHeap();

		public void Add(int element)
		{
			_maxHeap.Add(element);
		}

		public int RemoveMax()
		{
			return _maxHeap.RemoveMax();
		}

		public int Peek()
		{
			return _maxHeap.Peek();
		}

		public int Count
		{
			get
			{
				return _maxHeap.Count;
			}
		}

		public long Sum
		{
			get
			{
				return _maxHeap.Sum;
			}
		}

		public int Search(int value)
		{
			return _maxHeap.Search(value);
		}

		public void RemoveAt(int index)
		{
			_maxHeap.RemoveAt(index);
		}
	}

	public class Difference
	{
		public int A { get; set; }
		public int B { get; set; }
		public int Substract { get; set; }
	}

	class Program
	{
		static void Main(string[] args)
		{
			const long delay = 100;

			string line;
			bool timeToWorkLine = true;
			bool totalElementsLine = true;

			long timeToWork = long.MaxValue;

			var stopwatch = new Stopwatch();
			var numbers = new List<int>();

			int i = 0;
			
			while ((line = Console.ReadLine()) != null)
			{
				if (timeToWorkLine)
				{
					timeToWorkLine = false;
					timeToWork = ParseLongFast(line) - delay;
					stopwatch.Start();
				}
				else if (totalElementsLine)
				{
					totalElementsLine = false;
					var totalElements = ParseIntFast(line);
					numbers = new List<int>(totalElements);
				}
				else
				{
					numbers.Add(ParseIntFast(line));
					i++;
				}
			}

			var priorityQueue = new PriorityQueue();

			foreach (var number in numbers)
			{
				priorityQueue.Add(number);
			}

			var differences = new List<Difference>();

			while (priorityQueue.Count > 1)
			{
				int a = priorityQueue.RemoveMax();
				int b = priorityQueue.RemoveMax();
				int difference = a - b;

				differences.Add(new Difference {A = a, B = b, Substract = difference});
				long sum = (long)a + b;
				priorityQueue.Add(difference);
			}

			// setup paritions
			var listA = new PriorityQueue();
			var listB = new PriorityQueue();
			long sumA = 0;
			long sumB = 0;

			bool toggle = false;
			for (int j = differences.Count - 1; j >= 0; j--)
			{
				var difference = differences[j];
				var inB = listB.Search(difference.Substract);
				var inA = listA.Search(difference.Substract);
				if (inB >= 0 && inA >= 0)
				{
					toggle = !toggle;
				}
				if (toggle == false)
				{
					if (inB >= 0)
					{
						listB.RemoveAt(inB);
					}
					else if (inA >= 0)
					{
						listA.RemoveAt(inA);
					}
					listA.Add(differences[j].A);
					listB.Add(differences[j].B);
				}
				else {
					if (inA >= 0)
					{
						listA.RemoveAt(inA);
					}
					else if (inB >= 0)
					{
						listB.RemoveAt(inB);
					}
					listA.Add(differences[j].B);
					listB.Add(differences[j].A);
				}
			}
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

		private static long ParseLongFast(string s)
		{
			long y = 0;
			for (var i = 0; i < s.Length; i++)
			{
				y = y * 10 + (s[i] - '0');
			}
			return y;
		}
	}
}
