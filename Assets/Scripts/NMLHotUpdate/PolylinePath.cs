using System.Collections.Generic;
using UnityEngine;

public class PolylinePath
{
	private List<PathSegment> segments;

	private bool lengthDirty;

	private float length;

	public List<PathSegment> Segments => segments;

	public PolylinePathIterator Iterator => new PolylinePathIterator(this);

	public float Length
	{
		get
		{
			if (lengthDirty)
			{
				length = 0f;
				foreach (PathSegment segment in segments)
				{
					length += segment.Length;
				}
				lengthDirty = false;
			}
			return length;
		}
	}

	public bool EndsAtCurve
	{
		get
		{
			if (segments.Count > 0)
			{
				return segments[segments.Count - 1] is CurveSegment;
			}
			return false;
		}
	}

	public PolylinePath()
	{
		segments = new List<PathSegment>();
		lengthDirty = true;
		length = 0f;
	}

	public void AddSegment(PathSegment segment)
	{
		segments.Add(segment);
		lengthDirty = true;
	}

	public void RemoveSegment(PathSegment segment)
	{
		segments.Remove(segment);
		lengthDirty = true;
	}

	public List<Vector3> GetPathPoints(List<Vector3> points, List<Vector3> normals, List<Color> colors, int stepsForCurves, bool uniquePoints = true)
	{
		foreach (PathSegment segment in segments)
		{
			segment.AddPoints(points, normals, colors, stepsForCurves, uniquePoints);
		}
		return points;
	}

	public List<Vector3> GetPathPoints(List<Vector3> points, List<Vector3> normals, int stepsForCurves)
	{
		foreach (PathSegment segment in segments)
		{
			segment.AddPoints(points, normals, null, stepsForCurves, uniquePoints: false);
		}
		return points;
	}

	public override string ToString()
	{
		return "PolylinePath { Segments: " + segments.Count + " Length: " + Length + " }";
	}
}
