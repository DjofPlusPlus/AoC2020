﻿using System;
using System.Linq;

namespace AoC9
{
	class Program
	{
		const int preamble = 25;

		static void Main(string[] args)
		{
			long[] nums = input.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(t => long.Parse(t)).ToArray();
			Problem1(nums);
			Problem2(nums);
			Console.ReadLine();
		}

		static void Problem1(long[] nums)
		{
			Console.WriteLine("#1: " + FindFirstNumberWithoutPairSum(nums));
		}

		static void Problem2(long[] nums)
		{
			var result = FindFirstSliceWhichSumsTo(nums, FindFirstNumberWithoutPairSum(nums));
			Array.Sort(result);
			Console.WriteLine("#2: " + (result[0] + result[^1]));
		}

		static long FindFirstNumberWithoutPairSum(long[] nums)
		{
			return nums[preamble..]
				.Select((v, i) => (v, nums[i..(preamble + i)]					// Foreach item select the items before it
					.SelectMany(_ => nums[i..(preamble + i)], (a, b) => (a, b))	// Cross-join every item before with themselves
					.Where(x => x.a > x.b && x.a + x.b == v)))					// Only keep previous pairs of not equal and matching sum
				.Where(x => !x.Item2.Any())										// Find item without matchim sum
				.Select(x => x.v)
				.First();
		}

		static long[] FindFirstSliceWhichSumsTo(long[] nums, long searchForSum)
		{
			return Enumerable.Range(2, nums.Length - 2)							// Check subarrays of length 2..end
				.SelectMany(len => Enumerable.Range(0, nums.Length - len + 1)	// Range offset from 0 to end-length, inclusively
					.Select(offset => nums[offset..(offset + len)]))			// Slice
				.Where(s => s.Sum() == searchForSum)
				.First();
		}

		static string input = @"
18
15
39
28
25
48
41
1
32
33
8
49
43
36
40
5
20
26
22
38
4
14
3
35
29
30
6
37
7
76
9
12
10
11
15
13
18
74
8
45
17
32
28
16
41
19
20
14
34
22
23
21
27
35
24
25
26
29
30
51
37
31
33
63
49
36
38
39
40
61
41
42
50
83
77
44
45
52
53
55
54
56
59
71
64
67
69
72
143
97
106
90
79
99
86
95
87
98
89
122
96
100
146
107
109
110
123
255
168
131
157
141
177
185
179
362
165
166
173
175
176
189
246
212
253
230
207
216
345
219
309
354
272
351
535
438
306
385
339
331
338
340
526
348
364
365
646
419
423
559
923
426
435
491
525
578
603
767
1202
766
688
671
669
712
678
686
774
790
713
842
784
845
914
1275
861
1265
917
1401
1016
1103
1181
1688
1340
1347
1357
1349
1497
2095
1877
2963
1460
2131
2130
1555
1626
2185
2126
4460
1778
1933
2020
2098
3243
3953
2563
2844
2800
3903
2696
4226
2809
2957
3015
3086
4869
3181
4641
6910
3333
3404
3711
7084
3798
3876
4031
6755
4661
5363
10658
5505
5496
6213
5653
5711
9228
10859
5972
6348
6267
11924
6514
6737
7044
9074
8692
22783
9848
7674
11149
10298
14345
10874
10868
11216
17972
11844
11364
11625
11683
12320
12239
16646
13311
12781
13251
13558
15429
18408
16366
19841
33651
18542
22513
22493
21166
32622
22084
22551
22580
22989
23469
32161
23308
23922
44481
26339
26032
26092
26809
38917
28987
31795
34774
46415
42464
39708
40626
43250
43659
66719
45569
45131
45540
45888
46297
46777
47230
49340
49954
113134
83090
52124
52901
55796
81381
60782
66569
77238
80334
120960
82958
89199
86909
88790
204050
165258
91019
91428
92185
107920
94007
96570
99294
174341
119470
105025
134282
195864
164147
127351
160076
143807
168257
163292
300620
204490
265360
225301
182447
183204
185026
183613
193301
271158
240377
328833
201595
253752
307099
224495
232376
291498
387787
287427
361558
428985
679285
437365
422824
365651
620978
582585
366060
554859
500400
368639
376914
1234144
441972
561209
426090
493093
456871
511922
593134
953894
578925
648985
653487
727209
992224
959194
731711
734290
858744
980949
1133784
1370666
745553
1887631
803004
818886
868062
882961
1005015
1035796
1504146
968793
1383275
1172059
1477264
1546095
1302472
1550597
1613615
2173670
1617251
2984281
1479843
1548557
1564439
1685965
2339459
2177074
1621890
1787679
1686948
1751023
2055020
2207855
3234522
3885554
3605617
3090879
2474531
5431649
3025938
2866911
3044282
3028400
3308838
3798964
3112996
3101733
3170447
6911492
3372913
4878558
5278807
3409569
3437971
6827364
5236255
4262875
5233793
6037358
5341442
5500469
5912502
5502931
5968644
5892849
5895311
6072682
6130133
9070377
6214729
6272180
11131566
8606706
10175377
6847540
12006002
7672444
11129104
9475329
10736724
9604317
10158186
11788160
12041326
10841911
11573151
12167491
12486909
11861493
11967993
12025444
12344862
12977673
21771808
19650706
13944624
14519984
15454246
16322869
17276761
33599630
21443322
20446228
19762503
22883237
15353384
22503048
24009319
27422239
22415062
23886937
29244754
24370306
23829486
26545428
27698246
25322535
26922297
42533943
44132809
39267159
49209472
30807630
49805534
41163698
39240321
35115887
35799612
37768446
37856432
63221851
49425345
48199792
46244548
60271494
50751783
49152021
112647196
51867963
52244832
53020781
81774264
80232975
65923517
66607242
105847563
68576076
81559413
75039933
70915499
154047355
72884333
73568058
97351813
84100980
94444340
95396569
122720079
143799832
99903804
101019984
126907896
212551000
168012398
155272908
223848984
138807850
135183318
132530759
205415092
139491575
230579887
156599346
144483557
155016479
217367890
146452391
213059633
226975099
178545320
279565304
227927328
200923788
226811700
227927880
457391587
406473200
267714077
271338609
289130105
272022334
273991168
301468870
277014316
283975132
285943966
384527226
303051737
290935948
692417166
324997711
670471192
485081967
544728393
495641405
568695409
427735488
1261112575
695449565
551689209
947485508
626466581
545329777
543360943
546013502
628049448
551005484
562958282
560989448
569919098
576879914
593987685
615933659
718671436
752733199
997654586
912817455
1040369798
923376893
1174062950
1102694693
973065265
1122893416
1088690720
1089374445
1122209691
1220454266
1091343279
1094366427
1108971784
1154977133
1111994932
1192813573
1137869362
1163906783
1170867599
1209921344
2347790706
2087029031
1665550654
2092021013
1836194348
1896442158
2249864294
2061755985
2062439710
2064408544
2197662504
2178065165
2886004920
2185709706
2284156852
2200315063
2203338211
2220966716
3246998146
3089255731
2356720356
5089343131
2334774382
3727990364
3301942357
4520484088
3501745002
3886517370
3732636506
5425063311
3958198143
4239821150
4124195695
4363774871
4264723607
4378380228
4381403376
4484471915
5935974717
4403653274
8010713065
7388262372
4555741098
4691494738
5424030113
6599497989
5636716739
7665717228
6803687359
7034578863
9806466687
7856832201
7619153876
7997360113
12150189143
8321973014
11512458067
8388919302
8934121326
15742441404
8759783604
13024979111
9095148012
8959394372
9247235836
9979771211
10328211477
10115524851
12227717472
17636155138
14456330190
12440404098
13838266222
14422841235
14653732739
15616513989
16378937480
15941126890
16319333127
24436101401
16710892316
17148702906
18054542384
31079521495
24900521262
17719177976
30494946482
18206630208
18939165583
33955488265
20095296062
22555928949
22343242323
34147757098
30594859629
26278670320
26863245333
28261107457
29076573974
30270246728
34880292473
52934834857
33089829796
33030225443
33859595222
47418949634
34867880882
49286151703
54539777777
35925808184
36658343559
37145795791
38301926270
39034461645
50690155691
49419174282
57423809831
48621912643
64129841950
61158962793
83344757818
55124352790
73182218743
59346820702
72064687088
85558036573
89407658659
66120055239
66889820665
84549750913
70793689066
91685573568
72584151743
73071603975
73804139350
74960269829
88453635927
88992081961
111618613388
98041086925
122014173455
161037787670
244382545488
157622723661
116283315583
114471173492
157387907627
133150960052
125466875941
146875743325
133009875904
161576233704
136913744305
143865293041
143377840809
148031873804
145655755718
146388291093
204736951510
148764409179
223507962866
392414419292
235380373054
265392014264
294907617129
230754489075
239938049433
241750191524
247481049396
247622133544
268844716750
405758461055
262380620246
470128965774
269923620209
283302035398
280291585114
280779037346
386326340526
289766131902
292044046811
294420164897
481688240957
353501360689
372272372045
672706004406
466134862129
472504680599
487560182977
470692538508
478235538471
520717086779
920328137950
741349397349
510002753790
531225336996
532304240455
572823084157
633792945803
550215205323
561070622460
570057717016
634280398035
1020606738561
584186296799
586464211708
647921525586
725773732734
819636222818
838407234174
1039306160931
1057156750216
988238292261
1031763160968
1080060470806
1136679417031
1564067401423
1041228090786
1092295959456
2216739887837
1063529577451
1105127324612
1156521928724
1111285827783
1360054130769
1467557748404
1154244013815
1545409955552
1564180966908
1234385737294
2246539973271
1373695258320
2650537280164
2530217187044
1870170395142
2020001453229
2093365616873
2174815405234
2121288561592
2104757668237
2133524050242
2146355415398
3257641243181
2203581787239
2168656902063
2733749389089
2216413152395
3415580350694
2981456222925
2621801762219
2388629751109
2527939272135
2608080995614
4296947404112
3884923017458
4251113083635
3243865653462
6430471454354
3890171848371
3963536012015
10315394471812
4198123285110
4308339455476
13296850694737
4557286653172
4279879465640
4315012317461
4372238689302
4744352424530
4902406291152
4605042903504
5197869375320
4916569023244
8489965920962
5010431513328
4996710746723
6498252843985
11400659135137
8161659297125
7988218077992
7801152306634
7134037501833
8088295133481
8170051314011
8478002750750
8506462740586
12688089206072
12545504731164
9116591113832
11878389926363
12775094217515
19201811441771
13111505644090
12406195210138
9507449194656
9521611926748
21051967471750
15612040252583
18624040308488
12984928824715
11494963590708
24951699941302
26712335441969
15122255579825
24183052796780
14935189808467
15640500242419
16258346447492
16648054064761
16984465491336
36958147014295
18638203040580
19029061121404
20611554704540
24284585136501
21016575517456
21002412785364
21913644404794
28107184404540
32106518295248
25133652179331
27135463833127
40745692431914
28143017655469
24479892415423
26430153399175
30057445388292
41365343207642
31380602027317
30575690050886
44162713300735
40542931583993
32906400512253
58536671694423
43118095456003
37667264161984
41628130221996
45482305200787
58040052691584
99079603278416
62147156577407
47432566184539
50020828809334
55191097567623
55709342230217
50910045814598
51615356248550
52622910070892
54537337803715
55055582466309
100537887667096
70600376972285
63482090563139
95080269387708
85113027854601
70573664674237";
	}
}
