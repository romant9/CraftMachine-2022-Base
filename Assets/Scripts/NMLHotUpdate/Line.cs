using UnityEngine;

public class Line
{
	public Vector3 start;

	public Vector3 end;

	public Vector3 center => (start + end) * 0.5f;

	public Vector3 direction => (end - start).normalized;

	public Line(Vector3 inStart, Vector3 inEnd)
	{
		start = inStart;
		end = inEnd;
	}

	public Vector3 GetNormal(Vector3 up)
	{
		return Vector3.Cross(direction, up).normalized;
	}

	public bool Connected(Line other, bool checkEndOnly)
	{
		float num = 0.0001f;
		bool num2 = Vector3.SqrMagnitude(end - other.start) < num;
		bool flag = Vector3.SqrMagnitude(start - other.end) < num;
		if (!num2)
		{
			return !checkEndOnly && flag;
		}
		return true;
	}

	public void FlipToConnect(Line other)
	{
		float num = 0.0001f;
		if (Vector3.SqrMagnitude(end - other.end) < num || Vector3.SqrMagnitude(start - other.start) < num)
		{
			Flip();
		}
	}

	public void Flip()
	{
		Vector3 vector = end;
		end = start;
		start = vector;
	}
}
