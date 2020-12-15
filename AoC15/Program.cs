using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC15
{
	class Program
	{
		static void Main(string[] args)
		{
			var nums = input.Split(",", StringSplitOptions.RemoveEmptyEntries);
			Problem1(nums);
			Problem2(nums);
			Console.ReadLine();
		}

		static void Problem1(string[] nums)
		{
			Console.WriteLine("#1: " + ElvesSequence(nums, 2020));
		}

		static void Problem2(string[] nums)
		{
			Console.WriteLine("#2: " + ElvesSequence(nums, 30000000));
		}

		static int ElvesSequence(string[] nums, int iterations)
		{
			int val = 0;
			int sinceLast = 0;
			var numbersLastTurn = new Dictionary<int, int>(2000);
			for (int i = 0; nums.Length > i; ++i)
			{
				val = int.Parse(nums[i]);
				numbersLastTurn.Add(val, i + 1);
			}
			for (int turn = nums.Length + 1; iterations > turn; ++turn)
			{
				val = sinceLast; // << Turn is here
				sinceLast = numbersLastTurn.TryGetValue(val, out int lastTurn) ?
					(turn - lastTurn) :
					0;
				numbersLastTurn[val] = turn;
			}

			return sinceLast;
		}

		static string input = @"8,0,17,4,1,12";
	}
}
