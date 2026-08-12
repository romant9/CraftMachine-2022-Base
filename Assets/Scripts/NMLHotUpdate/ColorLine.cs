using UnityEngine;

public class ColorLine : Line
{
	public Color color;

	public ColorLine(Vector3 inStart, Vector3 inEnd, Color lineColor)
		: base(inStart, inEnd)
	{
		color = lineColor;
	}
}
