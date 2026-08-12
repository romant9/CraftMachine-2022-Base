using System.Collections.Generic;
using UnityEngine;

public class Edge
{
	public int A { get; set; }

	public int B { get; set; }

	public Edge(int a, int b)
	{
		A = a;
		B = b;
	}

	public Edge GetFlipped()
	{
		return new Edge(B, A);
	}

	public Vector2 GetNormal(List<Vector2> points)
	{
		Vector2 vector = points[B] - points[A];
		return new Vector2(vector.y, 0f - vector.x).normalized;
	}

	public bool PointInFront(List<Vector2> points, Vector2 point)
	{
		double num = Vector2.Dot(point - points[A], GetNormal(points));
		if (num == 0.0)
		{
			double num2 = Vector2.SqrMagnitude(point - points[A]);
			double num3 = Vector2.SqrMagnitude(point - points[B]);
			double num4 = Vector2.SqrMagnitude(points[A] - points[B]);
			if (num2 <= num4)
			{
				return num3 <= num4;
			}
			return false;
		}
		return num > 0.0;
	}

	public bool PointInLine(List<Vector2> points, Vector2 point)
	{
		return (double)Vector2.Dot(point - points[A], GetNormal(points)) == 0.0;
	}

	public double DistanceToPoint(List<Vector2> points, Vector2 point)
	{
		Vector2 vector = points[A];
		Vector2 vector2 = points[B];
		Vector2 lhs = point - vector;
		Vector2 vector3 = point - vector2;
		Vector2 vector4 = vector2 - vector;
		double num = Vector2.Dot(lhs, vector4) / vector4.sqrMagnitude;
		if (num < 0.0)
		{
			return lhs.magnitude;
		}
		if (num > 1.0)
		{
			return vector3.magnitude;
		}
		return (vector + vector4 * (float)num - point).magnitude;
	}

	public bool Contains(int index)
	{
		if (index != A)
		{
			return index == B;
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		Edge edge = obj as Edge;
		return this == edge;
	}

	public override int GetHashCode()
	{
		return A ^ B;
	}

	public static bool operator ==(Edge a, Edge b)
	{
		if ((object)a == null && (object)b == null)
		{
			return true;
		}
		if ((object)a == null || (object)b == null)
		{
			return false;
		}
		if (a.A != b.A || a.B != b.B)
		{
			if (a.A == b.B)
			{
				return a.B == b.A;
			}
			return false;
		}
		return true;
	}

	public static bool operator !=(Edge a, Edge b)
	{
		return !(a == b);
	}
}
