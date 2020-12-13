using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AoC13
{
	class Program
	{
		static void Main(string[] args)
		{
			var lines = input.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			Problem1(lines);
			//Problem2TheDumbWay(lines);
			//Problem2WithThisOneCheekyTrick(lines);
			Problem2WithCheekyTrickAndManyThreads(lines);
			Console.ReadLine();
		}

		static void Problem1(string[] lines)
		{
			int timestamp = int.Parse(lines[0]);
			var earliestBus = lines[1].Split(',')
				.Where(b => b != "x")
				.Select(b => int.Parse(b))
				.Select(b => new { Bus = b, NextDepart = ((timestamp / b) + 1) * b })
				.OrderBy(p => p.NextDepart)
				.First();
			Console.WriteLine("#1: " + (earliestBus.NextDepart - timestamp) * earliestBus.Bus);
		}

		static void Problem2WithCheekyTrickAndManyThreads(string[] lines)
		{
			var busOffsets = lines[1].Split(',')
				.Select((b, i) => new Tuple<int, int>(("x" == b) ? -1 : int.Parse(b), i))
				.Where(p => p.Item1 >= 0)
				.ToArray();

			// Still using cheeky trick that one bus has an offset == first bus ID
			var firstBus = busOffsets[0];
			var cheekyBus = busOffsets
				.Where(b => b.Item2 == busOffsets[0].Item1)
				.First();
			// LCM these two
			var lcm = LCM(firstBus.Item1, cheekyBus.Item1);

			// Similar to Problem2WithThisOneCheekyTrick, but within a value range
			long Problem2ThreadWorker(Tuple<int, int>[] busOffsets, long lcm, long rangeIndex)
			{
				long rangeLen = 100000000000000;
				long startAt = rangeLen * rangeIndex;
				long endAt = startAt + rangeLen;
				long lcmMultiple = (startAt / lcm) * lcm;
				long candidateTime = 0;
				do
				{
					if (lcmMultiple > endAt)
						return -1;		// Not found
					candidateTime = lcmMultiple - busOffsets[0].Item1;
					lcmMultiple += lcm;
				} while (!busOffsets.All(b => ((candidateTime + b.Item2) % b.Item1) == 0));

				return candidateTime;	// Found!
			}

			// Start as many thread as there are logical processors
			long candidateTime = 0;
			int rangeIndex = 0;
			var tasks = new List<Task<long>>();
			var procs = Environment.ProcessorCount;
			while (0 >= candidateTime)
			{
				if (tasks.Count < procs)
					tasks.Add(Task.Run(() => Problem2ThreadWorker(busOffsets, lcm, rangeIndex++)));
				else
				{
					int indexDone = Task.WaitAny(tasks.ToArray());
					candidateTime = tasks[indexDone].Result;
					tasks.RemoveAt(indexDone);
				}
			}

			Console.WriteLine("#2: (" + DateTime.Now + ") " + candidateTime);
		}

		// *** Turns out this is way faster than the dumb way, but not fast enough... There's probably a lot of smart math I don't see
		static void Problem2WithThisOneCheekyTrick(string[] lines)
		{
			var busOffsets = lines[1].Split(',')
				.Select((b, i) => new
				{
					Bus = ("x" == b) ? -1 : int.Parse(b),
					Offset = i
				})
				.Where(p => p.Bus >= 0)
				.ToArray();

			// This is a pretty cheeky way to solve this problem, I don't know if everyone will have a Bus with Offset == first bus' ID
			// But this lets use use a better LCM algorithm to find possible time matches faster
			var firstBus = busOffsets[0];
			var cheekyBus = busOffsets
				.Where(b => b.Offset == busOffsets[0].Bus)
				.First();
			// LCM these two
			var lcm = LCM(firstBus.Bus, cheekyBus.Bus);

			// With an lcm that works for two buses, find a multiple that works for all buses
			long lcmMultiple = lcm;
			long candidateTime = 0;
			do
			{
				candidateTime = lcmMultiple - firstBus.Bus;
				lcmMultiple += lcm;
			} while (!busOffsets.All(b => ((candidateTime + b.Offset) % b.Bus) == 0));

			Console.WriteLine("#2: (" + DateTime.Now + ") " + candidateTime);
		}

		// *** This doesn't scale well to large numbers
		static void Problem2TheDumbWay(string[] lines)
		{
			var busOffsets = lines[1].Split(',')
				.Select((b, i) => new {
					Bus = ("x" == b) ? -1 : int.Parse(b),
					Offset = i})
				.Where(p => p.Bus >= 0)
				.ToArray();

			// Similar to lowest common multiple problem. Step the lowest value.
			long matchingTime = 0;
			var multiples = Enumerable.Repeat(1, busOffsets.Length).ToArray();
			var results = busOffsets.Select((b, i) => (long)b.Bus - b.Offset);
			for (; results.Distinct().Count() != 1; matchingTime = results.First())
			{
				// Not equal, increase step on lowest
				var lowestIndex = results
					.Select((r, i) => (r, i))
					.OrderBy(ri => ri.r)
					.First()
					.i;
				multiples[lowestIndex]++;
				// Check next
				results = busOffsets.Select((b, i) => ((long)b.Bus * multiples[i]) - b.Offset);
			}
			Console.WriteLine("#2: " + matchingTime);
		}

		static long LCM(long a, long b)
		{
			return Math.Abs(a * b) / GCD(a, b);
		}

		static long GCD(long a, long b)
		{
			return b == 0 ? a : GCD(b, a % b);
		}

		static string test = @"939
7,13,x,x,59,x,31,19";
		static string input = @"1008141
17,x,x,x,x,x,x,41,x,x,x,x,x,x,x,x,x,523,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,13,19,x,x,x,23,x,x,x,x,x,x,x,787,x,x,x,x,x,37,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,29";
	}
}
