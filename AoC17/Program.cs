using System; 
using System.Collections.Generic;
using System.Diagnostics;

namespace AoC17
{
	class Program
	{
		static void Main(string[] args)
		{
			string[] lines = input.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			Problem1(lines);
			Problem2(lines);
			Console.ReadLine();
		}

		static void Problem1(string[] lines)
		{
			bool[,,] trid = new bool[24, 24, 24];
			int offsetXY = (trid.GetLength(0) / 2) - (lines.GetLength(0) / 2) - 1;
			int offsetZ = trid.GetLength(0) / 2;
			for (int y = 0; lines.Length > y; ++y)
			for (int x = 0; lines[0].Length > x; ++x)
				trid[offsetZ + 0, offsetXY + y, offsetXY + x] = '#' == lines[y][x];

			bool[,,] next = (bool[,,])trid.Clone();
			for (int cycles = 0; 6 > cycles; ++cycles)
			{
				for (int z = 0; trid.GetLength(0) > z; ++z)
				for (int y = 0; trid.GetLength(1) > y; ++y)
				for (int x = 0; trid.GetLength(2) > x; ++x)
				{
					int neighbors = CountNeighbors3D(trid, z, y, x);
					next[z, y, x] = trid[z, y, x] ?
						neighbors.IsWithin(2, 3) :
						3 == neighbors;
				}

				var temp = trid;
				trid = next;
				next = temp;
			}

			
			int found = 0;
			for (var z = 0; trid.GetLength(0) > z; ++z)
			for (var y = 0; trid.GetLength(1) > y; ++y)
			for (var x = 0; trid.GetLength(2) > x; ++x)
			{
				if (trid[z, y, x])
					found++;
			}

			Console.WriteLine("#1: " + found);
		}

		static void Problem2(string[] lines)
		{
			bool[,,,] quad = new bool[24, 24, 24, 24];
			int offsetXY = (quad.GetLength(0) / 2) - (lines.GetLength(0) / 2);
			int offsetZW = quad.GetLength(0) / 2;
			for (int y = 0; lines.Length > y; ++y)
			for (int x = 0; lines[0].Length > x; ++x)
				quad[offsetZW, offsetZW, offsetXY + y, offsetXY + x] = '#' == lines[y][x];
			
			bool[,,,] next = (bool[,,,])quad.Clone();
			for (int cycles = 0; 6 > cycles; ++cycles)
			{
				for (var w = 0; quad.GetLength(0) > w; ++w)
				for (var z = 0; quad.GetLength(1) > z; ++z)
				for (var y = 0; quad.GetLength(2) > y; ++y)
				for (var x = 0; quad.GetLength(3) > x; ++x)
				{
					int neighbors = CountNeighbors4D(quad, w, z, y, x);
					next[w, z, y, x] = quad[w, z, y, x] ?
						neighbors.IsWithin(2, 3) :
						3 == neighbors;
				}

				var temp = quad;
				quad = next;
				next = temp;
			}

			
			int found = 0;
			for (var w = 0; quad.GetLength(0) > w; ++w)
			for (var z = 0; quad.GetLength(1) > z; ++z)
			for (var y = 0; quad.GetLength(2) > y; ++y)
			for (var x = 0; quad.GetLength(3) > x; ++x)
			{
				if (quad[w, z, y, x])
					found++;
			}
			Console.WriteLine("#2: " + found);
		}

		static int CountNeighbors3D(bool[,,] trid, int z, int y, int x)
		{
			int found = 0;
			for (var k = Math.Max(0, z - 1); Math.Min(z + 1, trid.GetUpperBound(0)) >= k; ++k)
			for (var j = Math.Max(0, y - 1); Math.Min(y + 1, trid.GetUpperBound(1)) >= j; ++j)
			for (var i = Math.Max(0, x - 1); Math.Min(x + 1, trid.GetUpperBound(2)) >= i; ++i)
			{
				if (z == k && y == j && x == i)
					continue;
				if (trid[k, j, i])
					found++;
			}

			return found;
		}

		static int CountNeighbors4D(bool[,,,] quad, int w, int z, int y, int x)
		{
			int found = 0;
			int neighb = 0;
			for (var l = Math.Max(0, w - 1); Math.Min(w + 1, quad.GetUpperBound(0)) >= l; ++l)
			for (var k = Math.Max(0, z - 1); Math.Min(z + 1, quad.GetUpperBound(1)) >= k; ++k)
			for (var j = Math.Max(0, y - 1); Math.Min(y + 1, quad.GetUpperBound(2)) >= j; ++j)
			for (var i = Math.Max(0, x - 1); Math.Min(x + 1, quad.GetUpperBound(3)) >= i; ++i)
			{
				if (w == l && z == k && y == j && x == i)
					continue;
				neighb++;
				if (quad[l, k, j, i])
					found++;
			}

			return found;
		}

		static void DumpZPlane3D(bool[,,] trid, int plane)
		{
			for (var j = 0; trid.GetUpperBound(0) >= j; ++j)
			{
				for (var i = 0; trid.GetUpperBound(1) >= i; ++i)
					Debug.Write(trid[plane, j, i] ? '#' : '.');
				Debug.Write("\n");
			}
		}

		static void DumpZPlane4D(bool[,,,] quad, int plane, int w)
		{
			for (var j = 0; quad.GetUpperBound(0) >= j; ++j)
			{
				for (var i = 0; quad.GetUpperBound(1) >= i; ++i)
					Debug.Write(quad[w, plane, j, i] ? '#' : '.');
				Debug.Write("\n");
			}
		}

		static string test = @".#.
..#
###";
		static string input = @"#...#.#.
..#.#.##
..#..#..
.....###
...#.#.#
#.#.##..
#####...
.#.#.##.";
	}

	public static class ValueExtension
	{

		/// <summary>
		/// Check if a value is within two inclusive values.
		/// </summary>
		public static bool IsWithin<T>(this T value, T inclusiveMin, T inclusiveMax)
			where T : struct, IComparable
		{
			Comparer<T> comparer = Comparer<T>.Default;
			return
				0 <= comparer.Compare(value, inclusiveMin) &&
				0 >= comparer.Compare(value, inclusiveMax);
		}
	}
}
