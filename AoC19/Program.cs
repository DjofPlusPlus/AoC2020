using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC19
{
	class Program
	{
		static void Main(string[] args)
		{
			var lines = test3.Split("\r\n");
			Problem1(lines);
			Problem2(lines);
			Console.ReadLine();
		}

		static void Problem1(string[] lines)
		{
			// Parse rule strings into parts
			var stringRules = lines
				.TakeWhile(l => l != "")
				.Select(l => l.Split(": "))
				.Select(s => (int.Parse(s[0]), s[1].Split(' ')))
				.ToArray();

			// Start off the rules from the leaf rules
			var rules = new Dictionary<int, string>();
			foreach (var sr in stringRules.Where(x => x.Item2.Length == 1 && x.Item2[0].StartsWith("\"")))
				rules.Add(sr.Item1, sr.Item2[0].Trim('"'));
			// Iteratively find rules for which all sub-rules are known.
			while (rules.Count < stringRules.Length)
			{
				foreach (var sr in stringRules.Where(x => !rules.ContainsKey(x.Item1)))
				{
					if (sr.Item2.All(i => i == "|" || rules.ContainsKey(int.Parse(i))))
					{
						string pattern = string.Concat(new string[] { "((" }
							.Concat(sr
								.Item2
								.Select(i => i == "|" ? ")|(" : rules[int.Parse(i)]))
							.Concat(new string[] { "))" }));
						rules.Add(sr.Item1, pattern);
					}
				}
			}

			// Count matching rule 0
			var rule0Rex = new Regex("^" + rules[0] + "$", RegexOptions.Compiled);
			var countRule0 = lines
				.SkipWhile(l => l != "")
				.Skip(1) // Empty line
				.Where(s => rule0Rex.IsMatch(s))
				.Count();

			Console.WriteLine("#1: " + countRule0);
		}

		static void Problem2(string[] lines)
		{
			// Parse rule strings into parts
			var stringPattern = "(?(DEFINE)" + string.Concat(lines
				.TakeWhile(l => l != "")
				.Select(l => l.StartsWith("8: ") ? "8: 42 | 42 8" : (l.StartsWith("11: ") ? "11: 42 31 | 42 11 31" : l))
				.Select(l => l.Split(":"))
				.Select(s => $"\n(?<g{s[0]}>(" + string.Concat(s[1]
					.Split(' ', StringSplitOptions.RemoveEmptyEntries)
					.Select(i =>
						i[0] == '|' ? ")|(" :
						i[0] == '"' ? i.Trim('"') :
						//s[0] != "0" && i == "8" ? "(?&g8)" :
						//s[0] != "0" && i == "11" ? "(?&g11)" :
						/* else    */ "(?P>g" + i + ")")) + "))")
				) + ")\n^(?P>g0)$";

			Console.WriteLine("#2: PCRE regex\n" + stringPattern);
		}

		static string test3 = @"42: 9 14 | 10 1
9: 14 27 | 1 26
10: 23 14 | 28 1
1: ""a""
11: 42 31
5: 1 14 | 15 1
19: 14 1 | 14 14
12: 24 14 | 19 1
16: 15 1 | 14 14
31: 14 17 | 1 13
6: 14 14 | 1 14
2: 1 24 | 14 4
0: 8 11
13: 14 3 | 1 12
15: 1 | 14
17: 14 2 | 1 7
23: 25 1 | 22 14
28: 16 1
4: 1 1
20: 14 14 | 1 15
3: 5 14 | 16 1
27: 1 6 | 14 18
14: ""b""
21: 14 1 | 1 14
25: 1 1 | 1 14
22: 14 14
8: 42
26: 14 22 | 1 20
18: 15 15
7: 14 5 | 1 21
24: 14 1

abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa
bbabbbbaabaabba
babbbbaabbbbbabbbbbbaabaaabaaa
aaabbbbbbaaaabaababaabababbabaaabbababababaaa
bbbbbbbaaaabbbbaaabbabaaa
bbbababbbbaaaaaaaabbababaaababaabab
ababaaaaaabaaab
ababaaaaabbbaba
baabbaaaabbaaaababbaababb
abbbbabbbbaaaababbbbbbaaaababb
aaaaabbaabaaaaababaa
aaaabbaaaabbaaa
aaaabbaabbaaaaaaabbbabbbaaabbaabaaa
babaaabbbaaabaababbaabababaaab
aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba";
		static string input = @"44: 82 117 | 26 54
4: 94 117 | 94 54
86: 54 54
110: 117 66 | 54 17
7: 4 54 | 22 117
16: 117 27 | 54 15
36: 117 38 | 54 110
40: 117 117 | 54 56
37: 97 117
84: 120 54 | 55 117
96: 17 54 | 66 117
129: 16 54 | 116 117
10: 108 54 | 41 117
63: 54 97 | 117 40
23: 86 54 | 130 117
45: 54 130 | 117 53
32: 54 37 | 117 114
62: 119 54 | 60 117
71: 107 54 | 134 117
76: 27 117 | 70 54
42: 115 54 | 25 117
99: 54 91 | 117 12
15: 54 94 | 117 94
64: 56 54 | 117 117
114: 54 17 | 117 86
75: 87 117 | 51 54
0: 8 11
91: 117 54 | 117 117
8: 42
38: 40 56
1: 54 73 | 117 85
132: 117 104 | 54 17
134: 33 54 | 127 117
28: 12 54
123: 66 117 | 53 54
107: 117 123 | 54 111
68: 22 54 | 79 117
87: 58 54 | 100 117
31: 54 75 | 117 1
78: 117 12 | 54 118
12: 54 117 | 54 54
92: 54 40 | 117 64
97: 54 117
34: 54 12 | 117 64
20: 91 56
2: 54 13 | 117 99
17: 117 54 | 54 54
51: 117 71 | 54 3
131: 104 54 | 86 117
94: 117 54
5: 56 56
3: 89 54 | 109 117
41: 103 54 | 49 117
25: 117 80 | 54 121
81: 122 117 | 28 54
85: 54 47 | 117 65
39: 97 117 | 12 54
77: 97 54 | 66 117
22: 64 54 | 104 117
119: 97 54 | 17 117
33: 86 117 | 86 54
100: 61 117 | 36 54
35: 50 117 | 119 54
90: 54 94 | 117 86
57: 54 95 | 117 96
112: 12 117 | 66 54
115: 54 44 | 117 52
26: 126 54 | 76 117
105: 54 20 | 117 98
120: 130 117 | 64 54
83: 54 92 | 117 112
66: 56 54 | 54 117
79: 91 117 | 64 54
128: 117 48 | 54 127
11: 42 31
70: 117 97 | 54 118
6: 62 54 | 7 117
116: 117 101 | 54 133
27: 12 54 | 104 117
118: 117 117
46: 64 54 | 66 117
52: 30 54 | 124 117
80: 117 129 | 54 9
13: 117 104 | 54 40
89: 106 117 | 34 54
127: 91 54 | 64 117
88: 56 66
130: 117 117 | 54 117
9: 69 117 | 81 54
106: 97 117 | 66 54
124: 117 32 | 54 83
109: 54 90 | 117 125
121: 117 6 | 54 10
133: 66 56
113: 130 117 | 118 54
30: 18 54 | 102 117
48: 56 17
122: 94 117 | 40 54
49: 17 117 | 104 54
60: 54 94 | 117 66
55: 54 40 | 117 118
95: 97 54
29: 133 117 | 48 54
101: 40 117 | 12 54
98: 5 117 | 91 54
21: 54 118 | 117 40
111: 118 54 | 12 117
102: 54 111 | 117 43
135: 105 117 | 35 54
58: 54 93 | 117 84
65: 24 54 | 57 117
19: 54 17 | 117 94
126: 131 117 | 63 54
59: 117 13 | 54 77
82: 54 2 | 117 128
125: 117 91 | 54 12
74: 97 54 | 86 117
18: 54 39 | 117 46
24: 54 113 | 117 21
104: 56 117 | 117 54
117: ""b""
108: 117 88 | 54 122
47: 67 117 | 59 54
69: 117 78 | 54 33
67: 54 19 | 117 132
61: 117 74 | 54 23
14: 54 29 | 117 68
54: ""a""
43: 104 54 | 53 117
56: 54 | 117
103: 54 130 | 117 118
93: 117 72 | 54 45
73: 117 14 | 54 135
72: 117 118 | 54 118
53: 117 117 | 54 54
50: 117 97 | 54 94

bbabbaabbbaabaaaabbbbaabaabbaabbabbbbabb
bbbaaababbabbbaababbaaaaaaabaaabbbabbbab
bbaaabbbababbbaaabaabaabaaaaabaa
baabbbbaababbbbbaaaaaaabaaaabbbbaabaabaa
bbbabbabbbaababbabbbbbbbababbabaaaababbbbaababaaaaaaabab
bbbababbaababaaaabaaabbaabbbababbbbbaababaababbbaabbaaaababbbbab
bbaabaaaabbaaabbbababbabbbbaaaabbaababbababaaabbbabbbbaa
aabbbaabaabbabbbbbaaabaa
babbabbaabbbaabbbabbbbab
baababababbaababbbaaaaaabaabbaaabbbbbbbbabbbbabbaababbabaaabaaababbaabaa
aabbaaabaabbabaabbbabbaa
babaabbbaabaabaabababaababbabbba
aabbabbbbbbbaaaaaabaaabb
aaaabbbabbbbaabaabbbabaa
aabbaaabaababaaaabbbbaaa
babaaaaaabbabbbaaaabbbaa
bbaabbbbbbbaaabbbbabbbaa
abbbaaababaaababaaabaabaaabaababababababaaaababaabaaaabbaabbbabb
baaaababaabbabaabbbbaabbaabbaaabbbbabaaaaabababbbbaaabaa
bbbababaaabbabaaaaaabaabababaaaaabbaabbbababbbabbababaab
bbbbbbaabaaaabbbaababaabbbaaabba
baabaabaaaabbaaaabbbbbaabbaabaaaabaaabaaaaaaabbb
aaababbbbbbbbbaabaababaa
ababaaababbbababaaaabbbbababbbaaaaabaabb
aabababaaaabbbbaaaaaabab
abbabbbbaaaabbbaaaabbabbbbbbaaaaaabbaaaa
ababbbbbabbabaabbbbbabababaabaaaaabbbbba
abbaababaaaababbaaabaaba
aaaabbaaaabaababbaababaa
abaaabbaaaabbabbaababababbabbbbb
abbaaabaabbabababbbbbbbbababbabababaaaaa
aabaabababbbabbbbbababbbaaaaaabbabaaaaba
aabbbabbbbbbbbaabbabbaaabbababba
bbaabbbabbaaaaaabbaaaaaabaabaabbabaabbaa
aababbaabbaaaabbbbaabbaaaaabbbababaaaabb
babaabbaababbbaaaabbbbba
baabaababbaabbbbabbaabbbaabbaababbbaabbbabbabbba
babbababbbbabaabbbbbabaabababbbabaaabbab
bbbbbbaaabbabbbbbbbaabaabaaababb
aaaabaabbbbaaabbbbbaaaaaaababbbaabaaabababaaabaabbababab
baaaabbaababbbbaaaaaabbabbbaaabaabbbabbbbbaaaaaa
aabbbaababbaaababbaabaabaababaabaaabaabb
aaababbaabababbaaaaababbabbbbaab
aaaabbbbaaaaaabaaabababa
abbaaabbbaabbbaaaaabaaba
aabbabbbabbababaaabaabababbbbbbaabbbbabb
ababaaabbbbabbbbabbababaabbaababbabababbbababaab
bbbaabaabbbabaabbbbbaabaaaabaabbabaabbaa
abbaaabbbababbbabababbaa
bababbbabaaababaaabaaaaaabbbaabaaabbbaabbaababbabbababbabbbaababbabbbbba
abaaabbbbaaaaabbabbababaababbbbbaaabaabb
bbaaaabbbabbaabbaaabaaab
abbaababbaaabbaabbaabbbbbbbbabba
baaaaaabbabaabbaaaaaabab
babbaababbbaaabaaabbabbbbbbababaaaabaabbbbbaabba
bbababaaaaaaaaabbbaaabbbabaaabbbabbaaabbaaabbababbbbaabbbbbbabba
abaabaaabaababaaabbbaabaaaaaabaabbababbbbbbaaaabbbabaaabbbabaabaaabaabaaaaabbbaa
bbaabaabaaaabbaabbbbaabaabbaaabaaabaaaba
aaababbbabbabbabababaaaaabaaabbbbabababbbaababbaabbaabbaaabababaabaabaababaabaab
baabbbaabbbbaababbbbabababaabaabbaaaaaaaaabaabbbbbbbaaabbabbbbbaaabaabaaabaabbab
abababbaabaabaaabbbababaababbababbbabaabbbaababa
abbbaababbbabaaaabababaababbaabaaababbbababaabbb
bbbabbbbbbbaaaababaabaab
abbaabbbbabaaababbbbabbb
babbabbbabaaaaaabbaababa
aaaabbbaabbbbbbbabbbaababaaabaaabbbbabba
abbabababaaaabbabbaaabba
babababaababbbbbbbbaaaaaaaaaabab
baaaaabaaaabaaaababaaaaa
bbaababbbbbabbabababbabaabbbababaaaabbab
bbbaaaaabbaabbbbaaaabaabbbabaabbbabaaaab
abababbabaabbbbabbabaabbabaaaabb
abaabaaaaaaabbbaaaabbabbbbbababbabbabbaa
bbaabbbbabbbbaabbbbbbaba
baabbabaaaaabaababaabaaabaaabbab
bbbbbbbbbbbaaabaaaabbbbbbababbabbbbaaabbbabbabbaaaaaaaaababaaaabaaabbbaabababbbabbbbbaab
baabbbbaaababbbabbbabbabaababbbbbaaabbabaabbbaaaaabbbbba
babbaababaabbaaabaaabbbb
bbbaaaaaaaababbaaaaabbaaaaabababaababaabbaabbbababbbabaabbabbbaa
baaabbbaaaabbbbaabaaaaab
abbaaababbbbbabababaabaaabbbbaaabababaaabbbbaaba
bbbbaaaaaabbaababbbaabab
aaabaaaaabbbabbbaababaaaaaaabaababaababbabbbbaab
bbbbbbaabaaabbbaaabbbabbbaabbbaaaabbabbaabaabbbb
babbabbbaaabbabbbaaababb
baabbbaababaaababaababbb
abbababbabbabaabbbaaaaba
abaaabbbbbaaabbbaabaabba
bbaababababaabbbabababbbbbbaabaababbabab
bbbaabbbaabbbbaaabaabbba
abbabbabbbaabbbbbbaabbbbbbabbaba
bbbaaaaaabbabbbbabaabbbb
bababababbbababababaaabb
abaabaabbbababbbbbabbaba
aabbaaabaabbaabaaaaaaabb
bbbaaababbbaaaaaaaabbbbb
aababaabaaaabbbaabaabaabababbbbabbbbbabaaabbbaaa
bbababaaababbbaaaaaaabbb
aababbbabbbabaabaaabaaba
abaaaaaabbaababbaaabbbbaaaabbabbbababbaaaaaaabbb
aaabbabbabaaabbaaaaabaabbbabaaab
bbabbbbbbaaabbbbabaaaaabbbabbbaaaabbaaaabaabbabababbaabbabaababa
aababaabaabbabaaaabbbabaaabbababbababbabaabaabbbababaaaa
bababaaabbabbbaabbbabbbaaabbbaaa
bbbabbabbabbabaaabbbbbbaabaabbbaababbabb
bbabaaaaabbabbbabbbbaabbaabaabbabaaaabaa
bbaaaaaaabbaaabbbaaaabbabbbbaaab
abbabaabaaabababbbbbabaa
bbabbaabbababbabaabababb
baabaaababbbababbaabaabaababbaaa
abbabababbabbaabbbbbabba
aaabbabaaaaabaabbbbababaabababaa
bbbbabaaaaaabbaaaabbbaaa
bababbbaaaaabbbbaaabaaaabaabaabaaabbbaabbbbbabbbabbbbaaa
bbaabbabababbbababbbaabbabaaaaab
aaaababbbabbaababbbabbababbbbbbbbbbbabaabbaabbabbabbaaabbaaaabaa
ababbbabbaaabaababbaaaaaababaaaaaabbabbaaaabbbbaabbabababbbababa
aababaaaaaabbaaabbabbbaabbbbaabbaababaab
bbbababaaababbbabaaaaabbbbaaabbbbbaabaaabbbabaabaaaaaabb
bbabbaaabbbbaabaabbbaabb
aaababbbbbbaabbbbabbbaba
aaabbbabbbbbaaabababbaaa
ababbbaaabbbbbbbabaabaaaaaabbbab
babbaabbbbaaaaaabbabbababbabbbaabbaababa
ababbbbbaabbaaaaabaabbaabbbaaabbabbabaab
bbaabaabbbbaaabbabbabbaa
aababbaaaaababbbbabbbbaa
abbaaabbbbbabaaaaababbab
abbabaaaaaaabbaabbababaabbbbbbba
abaaabbaabbabaaaabaabaababbbbbbaaabababbabbbbaaaabbbbabb
bbababaaababaabaaaabbbab
aaaabbaaaabbabbababaabbb
babaabababaaaabbbbabbaba
baaaababaaabbaaaabbbabaabbaabbbbabbaaaaaabaabaaaabbbbababbbbbaabbbabbaabaabbbbbb
baabbababbaaabbbbbbbabaaaababbaaaaaababbbabbabababbbbbab
aabbbabbaaaabbbbbaaaabaa
bbabbaababbabaabaaaaabbababaabaaabbbabaa
babbabbbaaaababbbbaaabab
ababbbbaababbbbaababbbaabbabaabababbabba
baaaabbbbaaaaabbabaabbba
baaaaababbbabaabbbaaaaaabaabaaaabaabbbaaaabbbbba
ababaaaaabbbbbbbbabbbabb
abbaaabbbaaaaaaabbaababaabbbabba
abaabaaaabaabaabbbaaabaa
bbbabbabbaaaabbabaaaabbbababbaab
abbbbabbbbaababaaabaabababbbaababbaabbbabbabaababbabaabb
bbababaabbbabaaaaaabbaababaabbab
aabbabbbbbabbaababaababb
aaaaabbabbbbaaaabbaaaabbbbabbabb
aaaaabbabaabaaaaabababaabbbbbabb
babaaababbaaaabbaaababbaabababaaabbbbbaaaaaaaaaa
babbaabbabbaaababbaaabba
bbbbaabaababababaabbbbab
baaabbaaaaababbbbaaababa
abbbabababbaababababaaaabbbabbabbbaabaabaabaabaababbbbbb
babababaaaabababaaababaaaaaabbabbbababab
bbbbababbababababbaaabaa
aabaabababbabaaabaabbbab
bbbabbababaabaababababbaabaabaabbbbbbaab
bbbabababaaaaabaabbababbabbaaabaaaabbbababbbbaababbbbaab
aababaaaababbbaaaaaababbbbaabaaabaabbbaaabaaaabbabbaabbabbabbbbbabbbabaa
abbbbababbabbaaababbabbbaaabbbbababaababbaaabaab
bbaabaabaabbbaabaaaaabbaabaaaaaaabbbbaababbbbaaa
bbaaabbbbbbabbabbabbbbba
aabbbaabbbbabbabbbbbbabb
baaaaaaaabbbababbabaaabb
abaabaabaabbbabbbbbababbbbaabaababaabaababaaaabababaaaaa
abbaababbbbbbbbbbbbbaaab
baaaaababbababaaabaaaaba
abaaabbbbababbbabbbabaabbabaabbbaabbbababbbbbbababbaaabb
babbababaabbabaaaabaabbbbaabaaaaaaabaaba
abbababaaaaabbbaabaabaaabbabbbaa
bbbbabaabaaaaaabaabaababaabbbababbbbaaabbbaaaaba
aababababababbbbbbbababbbbbabbabbababbbb
ababbabbaabbbaaabbbbabbb
aaabbbbabbaaabbbbbbbaabb
baabaababbbbbbabaaaaaaaa
bbaaabaaabbaabababbbaaaabaabaaaa
baaaaaabbbbaaaaabbbaabaaabbbbaabbabbaaaa
baabbbaabbbababababbaaab
baaaabbbaaababbaaaababbabbbabaabaaaabbab
bbbbabababaaabbbbabaabab
aaabababbbabaabbababaaaabbbbabaaabaaabba
bbbaaabaaabbaaabbaaabbaabbababba
bbabbaaabaaaaabaababbbbababbaabbbbbbaaaaaababbabbabbbaba
aaaaaabaaabbabababababaaaaabaaba
abababbabbababbbbbababbbbababaaa
babaabbabbababaaabbbbabb
aabbabaaabababaaabbbbbaaaaabaaba
bbaaaaaaaaaabbbbbbabaaaa
aaaaaaabbaabbaaabbababba
aaababbbbabbaabbbbbaaabaaababbaaabbababbabaaaaab
aaababababaaabbbbbbaabba
abbbababaababbbbaabbaabb
abaababbabaaabbbbbbaaababaaabbabbabbbbaa
aabbaababbababaabbabbaaaabbabaaabbbabbbbbabbbaaaabbbbbbaabaabbaabbbbbaaa
aabbbabbabaabaabbbaaabbbabbbabbbbaaaabaababababbaababbabbaabbabb
aabbbaabaaabbaabaaaaabbb
abaaabbbbabaaababbaababa
baabaaabbbababaabaaababb
abbbaababbaaaabbbbaaaaab
aababbbaabbaaabbaaabbbbababbaabaabbbbbab
abbababaaaabaaaaababbabb
abbaabababbaaabbababbbab
bbbaabbbbbababaabababbababaababbbababaaaabbbbaaabbbbaabbbbbabbbabbabbbba
aabaaaabbaaaabbaaababbbbbaaabaaaabaaaaba
baaaababaaaabbbaaaabbbbabbbbabbaabbbbaabbabbabbabaabbbab
aaabbaabbbbbabaaaaaaaabb
bbabbaabbbbabaaaabbbabba
bbbbaabaabbabbabbabbaabbabaaaabb
aaaababbaaabaaaababaaababbbabaabbbbaabaa
abbbababababababbaaabbab
aaabbbaabbaababbababbabaaaabbabbaabbaaabaaabbbbbaaaaaaabaaaaabba
aabbabbaaaababababababbababaaaaa
aabbabbbabbbaaaaabbbbbaaabbbaaaaaabbbaabbbabaababbabbbbaababbaaaaaaabaaa
abbbabbbaaabbababbabbaabbaaababb
bababbbbbababbabbaabaababbbbabab
babbabbbaababbbbaaabaaba
bbbabaabaababaababbaabaa
aababaababbabababaaabaaa
bbbabbbbbabbaabbaabbbbaabbababaaabaaababaabababb
babaaabaaaababbaabaaaaaabaaaaabaabbbbaab
bbaabaababbbbbbbbbaabaaaaaabaabb
baabbaaaaaabbaabbbbbaababbaaabbbbaaaaaaabbbaabaabaaabbbb
aabbabaabaaaabbaabbabababaabaabb
bbbaabaabbbaaaaaaaabbaabaaaaaabb
baabbbaaabbaaabbaaaabbbbbbaaaabaabbaaaaaabaabbbaabbbabaababaabab
abbbbbaaabbabaaaaaabbaaaaabbbbaababaabababbbaaabaaaaabbb
aababbbaaaaababbabaababb
aabbbbaaabbabaaaabaaaaaabaabaaabbabbbbaa
aaaabbaabbaabbbbaabaaaaaaababbababbbbbbaaaabbbabaaaabbba
aaababbbabbaabababaaabbababbababbbabbbbabbbbbaab
abbaababbaaaabbbbaaabaab
aaaababbaababaabbbababbbaababbbaaababbaa
babbbabbaabbaabbbabbabaaababaaabbabbbbab
aabaaaabaaabbabaabbabbaa
bbbababbbbababaababaaaaa
aaaabbabaaaaaabbaaaabaaa
bababbbaabbbaabaabbabbbbbbbabaaaaabaaaabbaababbb
bbaaaaaaaaababbaaabaaaaa
abbbababbabaaabababaabaabaaaabaabbabbbab
bababbbbabbabaaabbbabaabbaabaababaababbaabbaabbabbabababaaababaababbbaba
abaaabbbbbbbbbaabbaabbaabbaabbababbaabba
aaaaabbabaaaaabbababbaaaaabbabaaabbaabbbbbaabaabaaabbbaabbbbbaba
baaaabababbaabaaabaaaaaaaabaabbbbaababaa
bbaaabbbbabbaabbaabaabba
aaaabbaaababbbbbaaaaabbb
abbababaaabbaabbaaaaabaabaaababbabbbaaababaaaababbbaabbaaaaababb
aabbaababbababbbbbbabaaabbbaaabaabababbababbbaaababaabaabbabbbbabbaaaaba
aaaaabbaaaabbbbabbbbabaabbbbbbaaaaaabbbabbbaaabbbbabaaaaaaabbbbbbababaababbbabaa
babababbbabbabbaaabbbbbaaaaaabaa
abbbbbabbbbbaaabbbabbababaabbbababaaaaba
aaabbaaabaaaaabbbabaabab
aaabbbbabababbbabaaaaabbabaababb
ababbbbbbbbabbabbabababb
ababaaababbbababbaabbbbb
babbaabaaabbabbaaaaabbbbbbaabaabbbbabaaabbabbbab
abbbaabaaababbbaaababaabaaababbbbbabbbba
aababaaaababbbbbbbbabaabbbbabbaa
aaabbabababbbbaabbabbaba
baabaabababbabaaaababbbbabaabbbb
ababaabaababbabaaabababaaaaaaaaaabbbbbab
bbbbabaabbbaabaabbbaabbbabbabaabbbbaabbaaaaaabbbabaaaaab
aabbabbabbbbbaaabbabbbabbbaabababbbaabbaaabbaabababababaabbaaaba
bbabbabaabaabaaabbaabbababaaaababaababbbbaaababbabaababb
babbabababbaaaabaaaaaaabaabaaabbaaaabbaabbbaaaabbaaaaabbaaaaaababababbba
abababababbabbabaaaabbba
aabbbabbbaaabbaabaaabbbbbaaaabaaabbbbbba
aabbabaaaaabaaaabbbbaaaabbbabbabaabababaabbaaaaa
bbaabbbbaaabbabbbbbabbaa
aaababbbbaaabbaaabbabbabbabaaabb
aabaabbbaaaabaabbbaaaaab
abaaaaaaaababbbaaaabbbbaaaababaa
bbaabaababababababbbbaaa
abbaaabbabbabbbbbaababbb
aaabbaabbabbaababaaabbaabbaabaaaabbbbbbabbabbbabaabbbbab
bbabbaababaaabbbaabaaaab
ababababaabaaaabbaaaaaaabbaabbbbbababbabaabaaaba
aaabbabbaababbaabbbabbba
bbbaaaaaababbbbabbbbbbaababbaaab
abababaaabbabbabbabbbaba
bbbaabaaabababaaaaabaaab
abbbabbbababbbbaaaabaaaababaaaab
bababbbabababababbbbbabb
bbaaabbbbbbbbbabbbbaaabababbbbba
bbabbaabbabaaabaaaabbbbb
ababbbaabbaabaabaabaaaabaaabababaabaaabbaabbbbba
aaaaaaabbaaabbbabbbbaaaababaabaabbbaabab
aaabbabaabbbabbbababaabb
baabbbbabbbaaabaabbabaabbbaabbbaaabbaaabbbbbababaabaabaabbbbbabaaabaaabb
bbbabaaabbabbaabbabbbbab
aabbababbaaaaabaababbbbabbbabaaababaaaabbbbabbbaaaaaaaaa
baabababaabbaaabaabbabbbbaaaababbbabaaabbbbbaabb
bbbaaaaabaabababaaaaaababbbaaaaaaababbbbbaababbb
baaaaabaabbabababbaababbaabaabba
abababbaaaaababbabaabbba
baaaaabbbaabbabaaaaaaaaabbabbbab
abbabababaaaabbbabbbbbaaaaaaaaabbbabaaba
aabaaaabbbabbbbbbabaaabbbabbaaaaabbbbbaa
abaaaaaabbbbbbabababbabaaabaaaababaaabab
abbabbbaabaababbabababbb
abbbbbbbbbaabaabbaaaabaa
ababaabaaababaaabaaaaabbaaaaabaa
aaabaaaaaaabbabbbbaaabba
bbbabaaabaabaababbbbaaaaaababbbaababaaaababbbbaa
abbabbbabbabaabbaaabbbbbabbababa
aabbababababbbaaaabbbabbbbaaabaa
abaabaaaababaaaaaaabaaaabaaabbabaabaaaba
baabaabaabbaaabbbbbabababbaaabaa
abbabaabbbbbabaaaabbbbbb
aaaabbaaabbbbbaababbbaba
aaaabbbabbbbaababbaabbbbabaaaabbbaababba
abbabaabaaabbbbaaaabbbbb
aabaaabbabbbbabbabbbaabb
aabbbaababbabbababaabbbb
aaaabbaaababbaabaaaaabbb
abbabbbbaaaaaaabbbbbbbabbaabbbbb
ababaaaabbaabaabaababaaaaabbbaab
baaabbaaabbabbbbbabbababaaabbbbb
bbaababbbabaabbaababbbab
abababbaabbbabababbbaabababaabaa
bbabbaabbbbabbbbbbbbababbaaabbbb
aaabaaaaababbabaaabbbabbbbabbaaaabaababb
baabaaaaaaabbaababbbabbbaabbaaab
aabababaabbbabbbbbbabaabbbbbbbaaababaaabaaaabaaabbabaaba
abbaaabaabbaabbbbbaaabab
baaaababbaaaababbbababba
aabaaaababbbbbbbbabbbabb
bbbbbbaabbbbbbbbbaabaabaababababbaabababaabaaabbababbaab";
	}
}
