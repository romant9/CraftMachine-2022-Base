using System.Collections.Generic;
using UnityEngine;

public class CurveSegment : PathSegment
{
	private Vector3 startTangent;

	private Vector3 endTangent;

	private Vector3 up;

	private float cachedLength;

	public override float Length
	{
		get
		{
			if (cachedLength < 0f)
			{
				cachedLength = 0f;
				Vector3 a = GetPosition(0f);
				for (int i = 1; i < 11; i++)
				{
					float t = Mathf.Clamp((float)i / 10f, 0f, 1f);
					Vector3 position = GetPosition(t);
					cachedLength += Vector3.Distance(a, position);
					a = position;
				}
			}
			return cachedLength;
		}
	}

	public CurveSegment(Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent, Vector3 up, Color color)
		: base(start, end, color)
	{
		this.startTangent = start + startTangent;
		this.endTangent = end - endTangent;
		this.up = up;
		cachedLength = -1f;
	}

	public CurveSegment(Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent, Vector3 up)
		: this(start, end, startTangent, endTangent, up, Color.white)
	{
	}

	public override Vector3 GetPosition(float t)
	{
		float num = 1f - t;
		float num2 = t * t;
		float num3 = num2 * t;
		float num4 = num * num;
		float num5 = num4 * num;
		return start * num5 + startTangent * 3f * num4 * t + endTangent * 3f * num * num2 + end * num3;
	}

	public override Vector3 GetDirection(float t)
	{
		return GetTangent(t);
	}

	private float GetDerivative(float t, float startValue, float startTangentValue, float endTangentValue, float endValue)
	{
		float num = 1f - t;
		float num2 = t * t;
		float num3 = num * num;
		return -3f * startValue * num3 + startTangentValue * (3f * num3 - 6f * num * t) + endTangentValue * (6f * num * t - 3f * num2) + 3f * endValue * num2;
	}

	public Vector3 GetTangent(float t)
	{
		float num = 1f - t;
		float num2 = t * t;
		float num3 = num * num;
		return (-3f * start * num3 + startTangent * (3f * num3 - 6f * num * t) + endTangent * (6f * num * t - 3f * num2) + 3f * end * num2).normalized;
	}

	public Vector3 GetNormal(float t)
	{
		Vector3 tangent = GetTangent(Mathf.Clamp(t, 0f, 1f));
		Vector3 tangent2 = GetTangent(Mathf.Clamp(t - 0.001f, 0f, 1f));
		Vector3 tangent3 = GetTangent(Mathf.Clamp(t + 0.001f, 0f, 1f));
		Vector3 vector = Vector3.Cross(tangent2, tangent3);
		return Vector3.Cross((Vector3.Dot(tangent2, tangent3) > 0.999f) ? up : vector.normalized, tangent).normalized;
	}

	public Vector3 GetUp(float t)
	{
		Vector3 tangent = GetTangent(Mathf.Clamp(t - 0.001f, 0f, 1f));
		Vector3 tangent2 = GetTangent(Mathf.Clamp(t + 0.001f, 0f, 1f));
		return Vector3.Cross(tangent, tangent2);
	}

	public override void AddPoints(List<Vector3> points, List<Vector3> normals, List<Color> colors, int steps, bool uniquePoints)
	{
		for (int i = 0; i < steps; i++)
		{
			float t = Mathf.Clamp((float)i / (float)(steps - 1), 0f, 1f);
			Vector3 position = GetPosition(t);
			if (uniquePoints || points.Count == 0 || (points[points.Count - 1] - position).sqrMagnitude > 0f)
			{
				Vector3 normal = GetNormal(t);
				points.Add(position);
				normals.Add(normal);
				colors?.Add(color);
			}
		}
	}
}
