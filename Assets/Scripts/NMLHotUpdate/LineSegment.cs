using System.Collections.Generic;
using UnityEngine;

public class LineSegment : PathSegment
{
	private Vector3 up;

	public LineSegment(Vector3 inStart, Vector3 inEnd, Vector3 inUp, Color color)
		: base(inStart, inEnd, color)
	{
		up = inUp;
	}

	public LineSegment(Vector3 inStart, Vector3 inEnd, Vector3 inUp)
		: this(inStart, inEnd, inUp, Color.white)
	{
	}

	public override Vector3 GetPosition(float t)
	{
		return Vector3.Lerp(start, end, t);
	}

	public override Vector3 GetDirection(float t)
	{
		float t2 = t;
		float t3 = Mathf.Clamp(t + 0.01f, 0f, 1f);
		if (t >= 1f)
		{
			t2 = 0.99f;
		}
		Vector3 position = GetPosition(t2);
		Vector3 vector = GetPosition(t3) - position;
		if (!(vector.sqrMagnitude > 0f))
		{
			return new Vector3(0f, 0f, 1f);
		}
		return vector.normalized;
	}

	public override void AddPoints(List<Vector3> points, List<Vector3> normals, List<Color> colors, int steps, bool uniquePoints)
	{
		Vector3 item = -Vector3.Cross((end - start).normalized, up).normalized;
		if (uniquePoints || points.Count == 0 || (points[points.Count - 1] - start).sqrMagnitude > 0f)
		{
			points.Add(start);
			normals.Add(item);
			colors?.Add(color);
		}
		if (uniquePoints || points.Count == 0 || (points[points.Count - 1] - end).sqrMagnitude > 0f)
		{
			points.Add(end);
			normals.Add(item);
			colors?.Add(color);
		}
	}
}
