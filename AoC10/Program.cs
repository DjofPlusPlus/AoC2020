using System;
using System.Linq;

namespace AoC10
{
	class Program
	{

		static void Main(string[] args)
		{
			int[] nums = input.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(t => int.Parse(t)).ToArray();

			var orderedNums = nums.OrderBy(v => v);
			var wallToDevice = new int[] { 0 }
				.Concat(orderedNums)
				.Concat(new int[] { nums.Max() + 3 }).ToArray();

			Problem1(wallToDevice);
			Problem2(wallToDevice);
			Console.ReadLine();
		}

		static void Problem1(int[] wallToDevice)
		{
			int step1jolt = wallToDevice.Where((v, i) => i > 0 && wallToDevice[i - 1] + 1 == v).Count();
			int step3jolt = wallToDevice.Where((v, i) => i > 0 && wallToDevice[i - 1] + 3 == v).Count();
			Console.WriteLine("#1: " + (step1jolt * step3jolt));
		}

		static void Problem2(int[] wallToDevice)
		{
			//int canBeRemoved = wallToDevice.Where((v, i) => i > 0 && i < wallToDevice.Length && wallToDevice[i + 1] - wallToDevice[i - 1] <= 3).Count();

			// Split input on the 3-gaps
			var gaps3 = wallToDevice
				.Select((v, i) => (v, i))
				.Where(vi => vi.i > 0 && wallToDevice[vi.i - 1] + 3 == vi.v)
				.Select(vi => vi.i);
			var spans = Enumerable.Range(0, 1)
				.Concat(gaps3)
				.Zip(gaps3)
				.Select(g => wallToDevice[g.First..g.Second])
				.Where(s => s.Length > 1);

			Console.WriteLine("#2: " + spans.Select(a => RecursiveFindPermutations(a)).Aggregate((long)1, (x, y) => x * y));
		}

		static int RecursiveFindPermutations(int[] permutation)
		{
			// Find the first item that can be removed
			int removableIndex = Enumerable.Range(1, permutation.Length - 2)
				.FirstOrDefault(i => permutation[i + 1] - permutation[i - 1] <= 3);
			if (1 > removableIndex)
				return 1;

			// Check possible permutations with and without the the item that can be removed
			return
				RecursiveFindPermutations(permutation[removableIndex..]) +
				RecursiveFindPermutations(permutation[..removableIndex].Concat(permutation[(removableIndex + 1)..]).ToArray());
		}

		static string input = @"
104
83
142
123
87
48
102
159
122
69
127
151
147
64
152
90
117
132
63
109
27
47
7
52
59
11
161
12
148
155
129
10
135
17
153
96
3
93
82
55
34
65
89
126
19
72
20
38
103
146
14
105
53
77
120
39
46
24
139
95
140
33
21
84
56
1
32
31
28
4
73
128
49
18
62
81
66
121
54
160
158
138
94
43
2
114
111
110
78
13
99
108
141
40
25
154
26
35
88
76
145";
	}
}
