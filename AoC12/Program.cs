﻿using System;
using System.Linq;

namespace AoC12
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
			var sp = new Point();
			int r = 0;
			foreach (var s in lines)
				MoveShip(s[0], int.Parse(s[1..]), ref sp, ref r);
			Console.WriteLine("#1: " + (Math.Abs(sp.X) + Math.Abs(sp.Y)));
		}

		static void Problem2(string[] lines)
		{
			var sp = new Point();
			var wp = new Point() { X = 10, Y = 1 };
			foreach (var s in lines)
				MoveWaypointMethod(s[0], int.Parse(s[1..]), ref sp, ref wp);
			Console.WriteLine("#2: " + (Math.Abs(sp.X) + Math.Abs(sp.Y)));
		}

		static char[] directions = { 'E', 'N', 'W', 'S' };
		static void MoveShip(char c, int val, ref Point sp, ref int r)
		{
			switch (c)
			{
				// Action N means to move north by the given value.
				case 'N': sp.Y += val; break;
				// Action S means to move south by the given value.
				case 'S': sp.Y -= val; break;
				// Action E means to move east by the given value.
				case 'E': sp.X += val; break;
				// Action W means to move west by the given value.
				case 'W': sp.X -= val; break;
				// Action L means to turn left the given number of degrees.
				case 'L': r = (r + val) % 360; break;
				// Action R means to turn right the given number of degrees. (+ 360 to remove negative degrees)
				case 'R': r = (r + 360 - val) % 360; break;
				// Action F means to move forward by the given value in the direction the ship is currently facing.
				case 'F':
					MoveShip(directions[r / 90], val, ref sp, ref r);
					break;
			}
		}

		static void MoveWaypointMethod(char c, int val, ref Point sp, ref Point wp)
		{
			switch (c)
			{
				// Action N means to move the waypoint north by the given value.
				case 'N': wp.Y += val; break;
				// Action S means to move the waypoint south by the given value.
				case 'S': wp.Y -= val; break;
				// Action E means to move the waypoint east by the given value.
				case 'E': wp.X += val; break;
				// Action W means to move the waypoint west by the given value.
				case 'W': wp.X -= val; break;
				// Action L means to rotate the waypoint around the ship left (counter-clockwise) the given number of degrees.
				case 'L': wp = Rotate(wp, val); break;
				// Action R means to rotate the waypoint around the ship right (clockwise) the given number of degrees.
				case 'R': wp = Rotate(wp, -val); break;
				// Action F means to move forward to the waypoint a number of times equal to the given value.
				case 'F':
					sp.X += (wp.X * val);
					sp.Y += (wp.Y * val);
					break;
			}
		}

		static Point Rotate(Point p, int deg)
		{
			var rad = deg * System.Math.PI / 180.0;
			int cos = (int)Math.Cos(rad);  // Only rotating by 90 deg so can use integer values
			int sin = (int)Math.Sin(rad);
			return new Point() { X = p.X * cos - p.Y * sin, Y = p.X * sin + p.Y * cos };
		}

		public struct Point
		{
			public int X;
			public int Y;
		}

		static string input = @"
R90
F58
S2
E4
F28
W3
N2
F25
E1
F24
W2
F91
S3
F83
L90
F24
R90
N3
R90
F5
W5
E1
S1
E5
F47
F83
N1
F64
W2
R90
S1
R180
N2
R180
S3
F35
E2
S3
W2
R270
F57
E5
F100
N3
L90
F70
E5
L90
F65
L90
S2
W5
F73
L90
E3
R90
W2
S2
L180
E4
W3
R90
E2
F18
N4
W5
R90
S1
F55
W2
L90
S1
L180
E4
N1
R90
F31
N5
E4
R90
W4
F18
W4
N2
L90
F6
N3
L180
F89
R90
N4
L180
S4
W1
F87
N1
F80
E4
R90
N4
F38
E5
F74
S4
R180
S1
L90
W1
F88
S1
F42
S1
R90
F2
R90
E2
S1
F57
E1
F53
R180
F97
W2
R90
F70
R180
W1
R180
W5
E5
N1
E5
F63
N3
F24
L90
S5
W5
R90
W4
F27
R90
E2
L180
E4
F16
L90
F47
S3
E1
S2
L90
W5
F80
E4
F10
N5
E3
N4
W2
L90
E1
F83
R180
S4
L90
E1
F94
N2
R90
S5
L180
E1
F40
E5
L180
W5
F57
E1
N5
W5
F62
L180
W3
F12
E4
F93
R270
F8
S3
W3
L90
W1
N1
E4
L90
E2
S2
E1
L270
E5
N5
F81
L90
W5
F82
E5
L90
F24
F3
F14
L180
N5
E1
F11
R90
F22
L90
S1
F82
S4
F38
L90
F31
R180
W4
F88
E3
R90
N5
F9
S3
W4
L90
W5
R90
F5
S4
F91
N4
S4
W4
S4
F78
N2
F67
L180
N3
R90
F65
N2
E1
F46
E2
L180
S4
E2
F98
L90
W2
N1
E3
F7
S4
F90
S2
W3
R90
F30
E4
F2
L90
F17
E4
R90
F94
N3
E5
R180
S4
F76
E2
F94
R90
N5
W2
F89
W1
F83
N5
W5
F62
S1
W4
N5
E2
R90
F14
R90
N5
W4
R180
E2
R90
L90
F67
N4
L90
E4
F93
W5
F85
L180
F45
W2
F78
N3
F90
L90
W1
S2
L90
S5
E5
F82
S4
F36
E5
L90
E1
F13
S2
E3
F65
L90
E3
E4
F1
W5
S3
F14
L90
F47
L90
S3
W4
F18
E1
N3
E5
F58
E5
S1
W2
F48
W5
F65
N5
E1
N3
R90
N2
L90
N4
F21
R90
F58
W1
F7
R90
E4
N5
F47
W4
L90
N4
R90
E1
L180
F8
E3
W4
F41
E3
S5
E5
S3
E3
F14
N4
R90
W3
L90
E5
R180
W4
S1
N2
F93
L90
F41
R180
F37
R90
E1
L180
N5
F96
W3
R90
E1
F88
S2
E2
L90
N2
W4
F34
E4
F69
L90
N3
F18
W3
S4
F44
E2
L90
N2
F55
R90
E1
L90
S5
E2
N3
L90
N1
F55
W5
N2
E2
F20
S2
W5
S1
W5
R180
F100
F44
S2
L90
E3
F98
N3
R90
W4
F14
N2
F87
W5
F12
L180
S2
W1
N1
W1
R90
L180
F64
E2
F41
L90
F20
R270
F91
N4
L180
F28
W4
F40
R90
F40
F36
E3
L270
F76
L90
N2
F15
N5
E4
L180
S1
R90
E1
F81
R90
F96
N1
E5
R90
S5
L90
S1
S4
R90
F30
E1
N5
E2
S3
F97
N3
R90
N1
R90
E3
R180
E3
F8
R90
W2
F27
L90
N5
W3
L90
E5
R180
S1
L90
F51
S4
L180
N1
W4
F71
W2
R90
E4
F29
E4
L90
L90
F68
W2
F57
E3
R90
S2
F94
W4
S2
L180
E3
N2
F52
E3
S1
S5
R90
E1
F35
W3
F53
R270
E3
F81
S2
L90
W4
F86
N5
E4
R90
N5
F99
L180
F65
R90
S2
W3
F33
E3
R270
F34
E1
F56
S2
E5
R180
N2
E3
N3
F30
N2
F22
E5
F10
N2
F16
N1
F31
R90
E2
F78
E4
R180
S3
R90
F80
E5
S1
F49
E1
S2
E4
N4
R90
F9
W1
E4
N3
N5
E4
L90
S5
E2
R90
F74
R180
N2
F98
S2
W4
W4
F73
E2
N4
E1
F25
S2
E5
L90
F96
N2
E3
N3
F16
S5
L90
F43
S2
R90
S1
F46
S3
F82
S5
S4
E5
F58
R90
F51
R90
N4
L90
W3
S4
R90
F15
E1
S4
W3
S2
W1
R90
F76
S1
L180
F5
R180
E1
L180
W3
F47
W4
F68
E5
F75
W3
N1
R90
S1
W5
R90
E2
F78
W2
L90
F24
W5
R90
F75
E1
F35
E5
R90
F57
L180
S1
L90
F90
R180
F63
S3
R180
W3
F79
N5
E4
F81
E3
F6
R90
E4
L90
W5
L180
W2
S4
F26
L90
E5
S3
L90
F3
L90
S2
R90
F55
W5
S1
E3
F63
L180
N2
E2
L90
S4
E3
F18
W4
N2
W5
R90
E5
L90
F96
N3
F7
N1
F87
R90
F53
R90
W5
S3
F94";
	}
}