﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC16
{
	class Program
	{
		static void Main(string[] args)
		{
			var lines = input.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			Problem1(lines);
			Problem2(lines);
			Console.ReadLine();
		}

		static void Problem1(string[] lines)
		{
			var rules = ParseRules(lines).ToArray();
			var invalidValues = lines
				.SkipWhile(l => !l.StartsWith("nearby tickets"))
				.Skip(1) // One more to get to values
				.SelectMany(l => l.Split(",", StringSplitOptions.RemoveEmptyEntries))
				.Select(valStr => int.Parse(valStr))
				.Where(val => rules.All(r => !val.IsWithin(r.R1.L, r.R1.H) && !val.IsWithin(r.R2.L, r.R2.H)));
			Console.WriteLine("#1: " + invalidValues.Sum());
		}

		static void Problem2(string[] lines)
		{
			var rules = ParseRules(lines).ToArray();
			var validTickets = lines
				.SkipWhile(l => !l.StartsWith("nearby tickets"))
				.Skip(1) // One more to get to values
				.Select(l => l.Split(",", StringSplitOptions.RemoveEmptyEntries)
					.Select(valStr => int.Parse(valStr))
					.ToArray())
				.Where(ticket => ticket
					.All(val => rules.Any(r => val.IsWithin(r.R1.L, r.R1.H) || val.IsWithin(r.R2.L, r.R2.H))))
				.ToArray();

			var allPossibleRulesByColumnIndex = Enumerable.Range(0, validTickets[0].Length)
				.Select(i => rules
					.Where(r => validTickets
						.Select(ticket => ticket[i])
						.All(val => val.IsWithin(r.R1.L, r.R1.H) || val.IsWithin(r.R2.L, r.R2.H)))
					.Select(r => r.Name) // Since this code already deals with column index, using names for rules instead of rule-index makes it easier
					.ToArray())
				.ToArray();

			// Progressively remove rules that can only apply to one column until there's a unique pairing for all columns
			var uniquePairs = new Dictionary<string, int>();
			while (uniquePairs.Count < validTickets[0].Length)
			{
				var onePossiblePair = allPossibleRulesByColumnIndex
					.Select((l, i) => (i, l.Where(x => !uniquePairs.ContainsKey(x))))
					.Where(li => li.Item2.Count() == 1)
					.Select(li => (li.i, li.Item2.First()))
					.First();
				uniquePairs.Add(onePossiblePair.Item2, onePossiblePair.i);
			}

			var ownTicket = lines
				.SkipWhile(l => !l.StartsWith("your ticket"))
				.Skip(1) // One more to get to values
				.Select(l => l.Split(",", StringSplitOptions.RemoveEmptyEntries)
					.Select(valStr => int.Parse(valStr)))
				.First()
				.ToArray();

			// Use unique pairing to find indexes in our ticket to select
			var productOfDeparting = uniquePairs
				.Where(kv => kv.Key.StartsWith("departure"))
				.Select(kv => ownTicket[kv.Value])
				.Aggregate((long)1, (x, y) => x * y); // Muliply all values
			Console.WriteLine("#2: " + productOfDeparting);
		}

		static IEnumerable<(string Name, (int L, int H) R1, (int L, int H) R2)> ParseRules(string[] lines)
		{
			return lines
				.TakeWhile(l => !l.StartsWith("your ticket"))
				.Select(l => ruleRex.Match(l))
				.Select(m => (m.Groups["name"].Value,
					(int.Parse(m.Groups["r1l"].Value), int.Parse(m.Groups["r1h"].Value)),
					(int.Parse(m.Groups["r2l"].Value), int.Parse(m.Groups["r2h"].Value))));
		}

		static Regex ruleRex = new Regex(@"^(?<name>[\w\ ]+)\:\ (?<r1l>\d+)\-(?<r1h>\d+)\ or\ (?<r2l>\d+)\-(?<r2h>\d+)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

		static string test = @"
class: 0-1 or 4-19
row: 0-5 or 8-19
seat: 0-13 or 16-19

your ticket:
11,12,13

nearby tickets:
3,9,18
15,1,5
5,14,9";
		static string input = @"
departure location: 35-796 or 811-953
departure station: 25-224 or 248-952
departure platform: 47-867 or 885-959
departure track: 44-121 or 127-949
departure date: 49-154 or 180-960
departure time: 35-532 or 546-971
arrival location: 41-700 or 706-953
arrival station: 25-562 or 568-968
arrival platform: 31-672 or 680-969
arrival track: 43-836 or 852-961
class: 38-291 or 304-968
duration: 31-746 or 755-956
price: 46-711 or 719-971
route: 35-584 or 608-955
row: 39-618 or 640-950
seat: 25-308 or 334-954
train: 26-901 or 913-957
type: 33-130 or 142-965
wagon: 34-395 or 405-962
zone: 46-358 or 377-969

your ticket:
97,103,89,191,73,79,83,101,151,71,149,53,181,59,61,67,113,109,107,127

nearby tickets:
895,527,676,768,695,821,473,414,835,426,741,650,886,709,938,355,113,358,106,888
559,796,709,661,116,680,773,857,118,304,704,578,720,339,584,914,270,196,661,861
390,557,348,432,734,441,74,761,272,266,531,704,52,78,200,478,455,664,663,339
400,386,926,211,100,481,358,429,450,336,943,549,933,78,274,722,571,483,144,442
579,509,478,975,218,855,93,759,92,406,339,648,144,128,478,948,489,482,547,926
512,946,469,183,24,694,889,198,551,947,275,857,408,943,734,382,308,80,448,119
305,830,449,54,518,193,663,825,95,946,484,672,248,701,257,395,827,783,218,189
128,52,773,150,561,436,483,913,526,819,903,700,530,941,757,509,386,885,554,788
779,947,111,357,206,252,661,481,124,450,773,554,779,827,116,466,259,434,901,898
651,143,274,523,89,53,116,71,513,108,753,858,209,282,410,436,357,57,517,743
87,691,793,306,426,127,152,836,192,497,276,418,66,771,147,910,824,917,767,510
180,657,454,609,60,190,705,337,941,947,835,554,546,84,546,772,389,193,531,890
128,387,865,735,462,548,652,89,902,728,490,490,278,931,81,265,449,523,948,112
588,892,450,651,154,583,667,835,356,497,81,405,671,480,53,259,514,896,352,734
928,686,618,946,933,205,485,392,445,119,69,248,265,398,522,725,438,781,821,555
73,213,183,89,210,650,349,356,722,550,341,816,986,824,250,145,890,446,690,698
488,462,456,569,746,471,460,995,916,777,526,116,520,71,762,569,455,472,890,647
549,351,421,477,661,335,148,530,759,207,599,283,304,208,271,609,919,574,528,708
610,833,569,613,672,664,924,99,94,389,819,664,727,739,249,768,836,911,252,825
923,429,814,577,109,818,571,522,714,646,815,706,898,439,216,494,495,931,898,458
96,852,211,822,181,71,697,709,542,745,273,813,493,499,474,196,469,79,276,792
608,219,527,665,474,755,419,700,913,644,822,393,666,412,335,402,281,618,460,771
267,438,112,433,853,103,676,766,187,831,490,569,927,826,771,334,773,377,342,209
258,98,457,900,83,511,612,897,220,414,119,24,734,572,147,425,72,509,118,287
81,386,854,74,363,118,738,448,53,772,526,112,198,948,926,915,186,526,408,501
853,553,490,194,282,756,261,939,754,448,386,522,516,83,154,65,433,780,415,451
153,702,580,200,948,198,146,379,121,109,759,114,153,659,794,735,852,568,347,84
856,742,770,91,270,611,693,148,450,265,506,889,680,657,130,729,75,457,455,8
188,553,865,61,184,208,643,405,996,212,856,574,943,470,745,532,185,811,613,735
887,381,24,68,426,640,89,85,693,757,460,109,913,689,763,358,121,655,498,424
772,771,418,58,922,152,272,378,689,254,290,568,453,24,73,768,254,457,503,896
695,649,924,79,209,661,775,859,575,221,858,86,269,857,836,305,274,23,659,795
61,553,740,52,522,270,388,124,736,660,457,897,262,665,833,104,442,530,270,479
392,893,306,252,278,336,259,560,421,103,150,409,923,460,190,711,261,503,12,924
863,768,397,857,384,260,70,916,96,470,390,466,472,666,442,50,457,819,532,69
861,441,900,519,383,467,809,191,72,426,720,711,380,468,505,485,546,266,743,652
189,82,440,738,685,640,507,194,699,694,449,449,404,437,553,113,947,433,925,64
151,768,503,66,918,129,913,474,827,823,358,115,750,833,791,783,576,790,920,276
432,813,10,925,689,694,558,462,813,194,515,739,476,210,490,182,726,609,480,421
148,211,56,811,945,213,550,354,465,529,463,795,58,655,677,919,282,266,812,357
462,455,76,737,383,471,663,617,811,84,97,796,263,546,930,442,871,409,827,467
250,724,682,652,197,512,501,861,373,220,276,899,734,777,258,467,888,287,697,441
89,918,763,793,208,823,694,467,212,264,755,889,104,643,530,766,702,767,462,154
146,347,79,341,791,195,858,622,648,833,796,390,189,262,192,249,719,392,67,95
367,144,697,887,430,506,343,739,220,283,455,469,507,471,684,85,547,489,856,832
498,65,97,187,445,427,615,583,451,198,734,513,142,454,756,713,513,612,471,789
501,483,212,690,715,739,735,410,900,561,584,336,855,727,651,70,526,613,776,527
654,762,519,185,350,438,584,940,831,687,378,782,278,436,716,816,502,63,217,835
825,265,471,920,584,286,118,109,616,271,772,511,192,726,633,103,407,416,419,418
223,263,121,56,187,654,514,737,876,942,856,286,427,768,831,740,455,281,85,854
486,642,271,617,576,672,459,151,763,890,665,697,708,96,180,678,98,579,854,354
78,219,92,736,143,865,268,862,658,120,356,732,704,854,147,411,214,685,117,926
491,777,270,230,62,552,477,427,618,337,213,508,790,571,345,512,410,69,72,552
697,149,409,661,576,615,696,142,942,353,281,736,473,452,739,452,418,367,866,583
354,738,737,108,637,112,211,825,356,926,520,766,345,355,700,948,279,212,764,90
203,425,771,222,279,428,583,507,277,689,687,215,188,864,467,72,674,641,381,759
479,763,261,511,655,62,568,781,656,223,530,75,692,987,104,935,266,733,467,262
415,111,785,308,489,111,932,648,455,512,218,55,910,285,389,355,497,520,547,724
650,76,531,598,767,216,406,947,438,212,920,256,611,572,660,395,865,789,251,856
498,740,14,822,82,335,522,724,932,414,284,775,722,191,699,493,421,740,475,184
151,337,451,581,663,419,279,400,646,272,64,744,181,684,512,727,393,142,739,469
251,649,640,621,76,858,305,440,923,264,407,642,520,687,258,827,108,947,78,932
832,546,893,919,218,96,196,478,418,769,722,864,576,382,597,787,392,949,655,691
83,741,429,56,289,615,546,734,127,450,638,192,343,251,498,255,852,308,671,923
447,818,826,743,145,695,285,485,414,432,864,358,95,481,104,931,10,734,561,931
784,502,263,204,449,94,766,941,684,464,124,776,425,86,608,380,272,467,485,282
261,449,457,646,349,721,199,466,731,783,198,192,61,455,901,366,512,449,725,70
449,145,579,783,501,426,111,736,277,257,433,864,455,217,289,600,128,642,573,51
21,788,352,920,184,794,783,938,785,405,614,885,580,102,515,129,57,502,93,938
578,664,726,192,224,484,761,452,105,436,556,240,765,671,273,249,920,663,896,858
217,436,644,118,743,764,671,273,746,922,891,721,390,856,897,561,910,826,788,117
507,618,859,708,149,687,922,181,119,350,671,391,457,980,727,554,786,788,386,441
617,478,483,217,640,929,183,935,274,771,245,258,664,385,834,286,79,434,448,145
428,946,524,413,402,51,496,115,777,682,64,378,681,493,520,609,147,77,900,692
261,642,350,949,640,561,626,63,394,254,829,525,886,584,51,409,380,812,515,550
282,66,827,524,274,514,830,356,703,818,721,862,532,447,405,614,901,949,334,561
101,546,574,365,439,390,922,96,581,72,896,782,80,513,439,280,78,149,520,291
183,854,86,307,796,500,150,103,358,725,368,937,191,211,900,644,732,742,198,142
616,558,63,650,349,127,349,554,484,80,103,261,678,407,832,212,265,187,767,693
441,943,943,730,689,980,471,183,348,289,948,820,641,469,569,666,914,438,575,609
860,482,775,386,283,487,216,717,930,513,53,255,335,756,502,392,378,199,720,790
781,656,555,584,206,284,519,490,890,463,679,272,289,776,559,408,343,206,572,486
665,795,189,287,569,889,387,363,939,207,523,501,819,856,259,516,146,210,816,855
857,78,94,277,821,336,715,663,760,184,934,460,424,528,511,478,780,892,214,738
568,457,280,108,243,892,831,917,127,928,351,99,282,103,889,645,767,416,380,349
933,760,991,736,661,127,853,682,756,180,291,216,83,825,935,651,146,113,498,813
84,408,381,551,50,274,696,471,15,514,612,616,197,780,916,198,763,102,523,503
584,416,85,180,931,485,900,761,674,358,915,502,103,938,946,614,728,290,471,892
695,51,391,102,251,560,391,334,23,578,80,731,729,482,811,61,919,769,87,272
827,143,548,613,933,986,813,513,664,283,796,860,268,933,493,568,444,391,383,855
780,504,571,489,910,927,254,71,889,430,405,59,472,886,482,486,826,887,700,781
60,341,248,187,998,265,128,338,506,65,698,664,741,186,284,711,277,867,550,559
406,304,376,249,742,790,181,345,62,668,498,645,684,822,180,478,812,74,432,506
270,381,831,187,680,283,241,608,735,496,130,724,530,892,792,81,419,772,655,351
482,106,781,656,336,736,64,737,192,357,264,663,185,709,988,383,191,443,217,97
267,944,254,179,82,486,290,794,86,496,617,662,762,308,450,385,180,683,479,215
926,406,626,860,422,897,710,735,693,82,520,561,180,268,252,651,922,250,554,251
468,475,923,562,8,289,924,914,944,392,216,51,485,60,196,927,187,815,78,290
61,855,766,451,864,361,271,105,153,927,338,939,475,788,60,939,104,212,546,900
704,512,570,707,831,427,571,487,429,945,217,186,344,474,50,571,493,405,708,441
388,867,405,398,740,196,147,451,470,546,460,128,98,77,405,776,285,510,824,153
279,438,550,654,392,979,461,852,692,560,56,931,920,69,614,642,427,930,512,116
116,78,490,214,275,494,488,550,397,435,206,144,461,898,947,901,688,557,482,51
579,519,554,903,552,731,59,211,729,654,823,761,154,514,65,888,494,697,348,142
68,129,412,83,870,932,154,643,730,786,659,352,835,681,524,433,658,436,261,347
488,814,279,774,821,105,710,483,16,105,460,520,477,786,546,206,75,554,70,699
652,515,670,211,289,611,348,896,926,199,691,282,718,349,291,104,209,201,823,692
355,87,455,472,934,568,832,827,340,776,261,818,1,143,933,722,482,287,698,785
449,557,357,270,94,463,647,885,853,701,61,59,761,923,578,611,424,729,920,81
619,826,258,520,738,779,577,90,719,757,513,658,338,683,836,918,288,463,691,340
646,128,424,440,87,709,788,391,305,308,381,188,746,78,794,386,636,813,500,270
191,778,659,767,346,257,95,154,393,423,609,380,107,617,857,347,122,142,223,431
516,71,889,922,407,522,870,736,777,147,394,118,818,689,358,344,470,207,526,761
98,97,682,814,62,20,108,828,482,711,524,764,201,771,834,186,495,409,418,434
287,305,54,611,337,489,771,537,816,683,517,465,304,220,515,770,467,572,148,666
215,390,287,269,284,121,727,925,305,815,347,87,808,193,121,97,577,524,820,186
266,213,87,854,580,489,764,57,275,250,538,338,547,307,269,121,143,106,115,183
555,893,866,888,522,195,404,557,455,647,774,494,818,109,784,571,66,822,671,440
658,555,105,338,505,661,573,740,127,508,56,667,642,449,658,111,120,398,255,60
277,65,550,280,640,817,900,262,490,419,791,201,266,548,881,219,350,287,260,821
494,478,573,415,928,70,821,231,394,121,383,562,666,440,664,77,474,348,380,820
666,260,412,534,918,655,449,283,743,463,90,727,558,76,113,711,612,824,745,941
898,815,783,816,94,289,69,222,480,201,108,640,580,522,205,117,468,715,127,781
942,472,648,744,925,112,903,781,410,106,897,274,306,818,781,575,896,664,450,647
407,390,945,269,818,584,616,96,642,185,813,127,900,830,708,475,221,888,366,650
249,407,939,915,95,265,439,436,164,71,766,943,197,110,115,75,890,63,84,268
277,82,617,513,128,334,114,549,621,284,653,709,818,104,836,478,892,181,113,205
443,473,198,416,918,642,646,795,900,488,1,920,728,443,824,188,695,578,522,78
80,443,451,573,181,518,592,735,688,110,919,143,387,64,142,411,193,252,643,919
392,492,486,95,528,252,448,934,527,579,796,902,822,305,477,270,468,923,859,889
643,356,190,830,863,153,76,91,932,457,691,880,344,775,108,512,512,286,61,769
146,490,417,429,687,395,525,477,930,521,61,268,444,184,64,77,235,466,110,727
461,582,343,666,825,188,84,941,886,107,382,129,487,660,261,173,192,72,53,913
128,935,546,419,143,787,83,779,577,431,859,658,496,475,403,52,380,357,863,548
472,900,916,942,777,776,433,659,422,357,53,826,204,182,19,686,575,796,509,532
908,201,268,129,858,495,916,711,697,854,224,757,308,406,484,477,426,503,687,89
794,628,392,180,822,508,555,787,57,202,568,387,507,937,185,916,575,514,103,91
519,348,79,82,745,708,515,99,111,416,564,411,385,652,76,409,818,758,469,473
489,121,342,573,488,439,504,435,519,919,62,265,764,690,927,622,788,348,823,556
660,501,588,654,478,660,725,721,653,352,581,343,88,196,286,616,787,532,830,644
766,525,582,471,947,198,501,89,104,900,745,685,362,455,249,211,389,812,506,113
425,506,457,255,289,492,498,468,707,766,180,386,609,742,57,564,828,782,769,515
275,363,148,699,351,525,812,767,67,932,87,551,308,866,112,188,304,440,431,392
424,414,510,822,223,709,419,55,73,546,203,404,759,860,279,273,927,474,347,861
505,439,783,384,765,719,58,415,353,778,418,674,147,433,794,93,945,823,411,70
918,250,934,436,812,385,207,675,481,383,405,345,208,618,681,84,87,785,616,215
264,94,350,861,116,502,155,946,578,938,391,187,96,666,491,927,653,260,220,429
391,895,497,648,495,81,256,887,348,181,216,354,213,657,501,561,743,483,824,998
938,854,791,777,740,100,767,862,648,432,942,104,562,93,670,216,650,6,220,526
255,110,477,344,347,887,124,552,82,895,147,58,260,77,424,813,615,444,223,928
925,691,345,895,708,388,860,249,563,119,503,928,865,97,498,492,202,426,661,117
472,96,280,424,480,440,348,896,73,503,718,862,730,899,260,941,469,337,186,259
708,646,6,220,482,700,202,918,889,337,831,130,656,890,921,766,530,210,66,381
524,508,528,346,649,77,433,939,935,793,423,304,318,97,671,932,562,736,726,253
145,215,651,721,675,819,206,696,723,111,216,512,385,285,928,726,709,495,437,666
118,288,95,866,435,102,353,196,335,935,145,105,125,508,306,438,818,261,854,736
656,529,742,807,189,416,856,290,414,933,88,572,348,473,925,104,831,186,281,665
249,198,417,714,279,936,813,814,823,357,351,89,304,60,936,503,103,377,381,505
787,933,280,527,492,704,405,252,468,785,305,457,431,193,378,521,896,283,422,741
814,860,216,405,129,790,490,906,465,929,181,478,667,93,497,888,423,641,866,502
834,521,429,529,388,792,477,216,440,683,739,251,831,360,770,650,51,337,727,205
204,401,110,794,394,682,569,95,190,287,720,515,70,524,440,191,501,442,853,644
186,259,509,476,664,687,865,420,186,220,480,651,229,933,153,765,497,546,471,580
405,916,150,514,702,387,741,939,385,896,576,824,487,504,866,867,356,734,889,581
734,358,829,735,548,142,437,388,331,773,455,182,152,336,459,489,483,79,581,470
67,656,820,264,406,739,698,51,516,547,822,128,580,191,71,0,562,857,550,103
513,490,822,982,934,763,555,194,145,789,99,917,380,253,504,388,344,420,75,180
101,662,492,414,147,343,818,668,490,488,69,715,406,731,552,81,357,477,945,833
495,931,719,384,274,145,366,98,501,356,654,437,507,898,248,406,387,556,757,381
410,423,821,395,984,521,862,260,190,611,654,709,213,145,264,84,935,496,248,385
494,94,549,59,784,488,435,685,191,769,181,143,213,441,918,7,90,446,514,734
886,505,744,566,855,288,441,655,487,507,417,744,901,786,278,431,211,769,778,570
85,812,930,667,610,117,735,881,706,414,65,857,289,287,758,109,556,891,103,84
79,476,445,211,515,707,725,557,859,80,393,792,600,470,427,478,120,390,504,930
552,896,73,618,449,447,692,818,662,887,338,820,690,185,406,553,586,643,335,75
745,668,406,877,551,76,756,501,785,98,459,304,519,720,57,469,416,739,668,793
276,834,581,258,764,557,184,760,186,859,857,175,939,924,578,759,866,349,681,287
578,153,766,489,349,406,276,380,709,55,852,304,222,859,900,878,492,347,437,925
579,665,790,901,416,929,75,914,940,205,814,354,196,681,909,432,652,265,651,194
58,505,344,79,528,418,731,121,604,142,270,150,780,257,88,355,789,935,83,337
455,419,391,189,187,497,687,531,130,129,400,856,206,276,260,273,727,696,571,769
886,920,698,784,582,611,698,856,440,248,475,392,876,463,190,443,914,70,697,222
91,205,353,920,477,475,153,672,948,573,485,270,665,480,63,222,784,473,457,993
738,745,212,346,653,828,50,860,379,118,921,694,540,819,214,147,657,94,55,497
793,17,194,925,150,350,755,554,508,252,527,289,55,253,694,509,769,102,59,118
461,250,272,394,480,248,758,890,667,128,575,217,842,700,649,792,212,693,897,96
926,928,558,452,588,515,289,459,221,481,913,60,465,61,143,580,415,744,442,685
267,152,772,891,153,257,448,895,115,612,520,538,710,698,885,463,195,746,86,466
776,89,497,277,344,684,573,414,194,455,667,626,127,523,271,283,340,446,471,784
334,641,853,473,278,452,94,763,644,690,831,738,136,256,733,922,935,462,768,776
249,142,256,97,75,562,458,828,865,680,444,361,644,464,930,423,789,84,688,419
706,52,714,347,510,782,286,428,708,113,505,89,901,834,770,120,511,427,204,63
86,62,501,468,64,885,105,641,214,825,453,560,388,764,79,123,690,413,380,487
96,885,505,689,938,511,483,189,202,97,355,792,500,689,392,336,713,740,853,788
531,89,925,122,261,479,923,864,816,106,568,453,692,518,556,929,709,78,928,790
456,513,395,212,757,730,729,653,724,975,429,756,457,418,489,725,929,528,437,184
947,854,154,255,95,70,724,682,226,94,423,187,641,616,252,105,885,558,658,695
663,386,584,556,209,277,542,732,552,106,818,819,690,415,415,478,216,219,815,218
272,743,99,339,74,647,72,488,577,353,89,426,91,745,823,135,782,757,773,57
81,826,492,113,562,66,430,287,830,370,284,434,547,142,899,338,765,378,732,644
644,350,490,929,901,816,942,69,493,257,118,695,858,713,79,222,500,343,116,608
487,412,702,771,418,722,857,475,437,450,812,830,250,490,561,553,658,709,90,462
550,60,90,145,429,203,991,509,94,283,500,608,468,892,460,617,916,277,857,474
524,695,889,437,582,895,901,422,761,691,107,117,476,54,818,807,269,517,928,894
507,715,776,831,120,385,338,415,377,95,579,445,696,785,91,696,274,568,307,415
85,90,129,78,940,774,521,468,459,101,431,348,255,205,416,24,66,434,287,755
897,937,896,507,895,657,932,858,819,890,471,430,994,681,410,420,925,498,280,940
50,941,470,745,458,72,271,740,288,546,549,125,504,467,248,254,184,767,91,51
575,461,649,149,54,762,925,613,741,584,408,922,738,305,377,703,433,410,755,81
507,269,148,886,180,659,817,71,466,476,111,336,498,782,361,707,915,576,650,478
764,475,57,763,925,88,515,555,803,641,351,414,271,548,121,657,719,467,860,473
484,764,910,487,191,204,721,682,113,187,926,569,441,337,761,787,142,202,353,106
714,437,468,562,811,198,443,420,118,610,743,923,666,643,513,743,934,354,433,357
513,578,744,574,721,739,551,488,207,574,865,145,901,103,701,508,218,818,264,777
684,572,186,679,944,776,78,105,947,212,937,424,857,412,929,787,532,186,260,812
423,259,558,577,648,425,306,672,307,714,550,573,348,612,249,744,253,521,427,193
270,273,474,746,531,836,269,443,471,433,100,732,625,510,654,557,894,420,688,248
825,522,945,833,262,507,82,601,680,945,446,946,921,89,477,513,411,276,70,151
472,664,80,343,277,669,268,224,59,791,347,342,128,934,645,741,375,379,687,185
668,894,212,392,95,491,791,264,573,812,894,698,728,73,767,448,794,634,116,213
885,52,940,305,788,558,447,705,744,526,557,571,549,777,690,886,393,392,377,518
107,344,897,693,767,468,249,560,570,289,121,341,608,435,412,860,399,778,646,470
429,351,515,87,726,888,523,650,418,464,803,200,666,56,813,829,670,816,192,657
279,187,280,500,382,942,52,687,853,62,491,306,934,506,699,321,796,381,934,55
69,772,786,783,722,222,759,358,265,853,710,445,567,920,770,645,936,224,80,733
377,795,794,438,256,813,829,852,511,919,872,662,200,188,457,889,942,467,519,503
437,96,211,829,931,335,347,811,409,700,736,20,119,525,395,338,930,474,734,486
502,199,287,285,471,551,488,252,895,466,104,415,156,945,512,51,358,830,340,859
129,340,461,772,670,686,812,386,438,744,662,590,532,811,577,143,641,888,308,934
85,815,948,571,117,949,516,980,822,58,258,737,856,573,212,819,923,531,914,949
945,531,934,344,917,692,343,281,291,288,164,811,697,899,897,187,928,266,285,852
377,420,283,655,127,421,221,740,550,611,50,306,417,80,610,392,429,705,477,852
116,211,852,422,484,933,215,215,760,202,349,789,440,685,615,536,149,616,729,568
497,393,743,867,60,863,150,13,939,149,56,527,783,111,782,493,671,280,260,923
742,284,811,708,148,710,925,265,734,413,384,731,992,689,467,784,103,886,259,73
484,349,479,705,188,188,933,465,221,722,72,571,274,735,456,866,192,818,76,393
942,998,248,914,272,380,380,924,461,61,291,392,154,709,420,574,813,493,786,511
55,50,353,792,84,721,865,223,205,833,997,930,579,761,583,98,727,744,647,780
766,82,280,201,943,695,516,898,68,143,682,205,387,780,433,230,744,200,690,936
205,558,687,475,90,337,920,665,529,196,456,445,283,55,120,749,60,304,553,389";
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
