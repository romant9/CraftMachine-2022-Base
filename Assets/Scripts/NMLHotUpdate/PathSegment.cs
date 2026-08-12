using System.Collections.Generic;
using UnityEngine;

public abstract class PathSegment
{
	public Vector3 start;

	public Vector3 end;

	public Color color;

	public virtual float Length => (end - start).magnitude;

	public PathSegment(Vector3 inStart, Vector3 inEnd, Color inColor)
	{
		start = inStart;
		end = inEnd;
		color = inColor;
	}

	public abstract Vector3 GetPosition(float t);

	public abstract Vector3 GetDirection(float t);

	public abstract void AddPoints(List<Vector3> points, List<Vector3> normals, List<Color> colors, int steps, bool uniquePoints);
}
