using UnityEngine;

public class ScaleTrainingGrounds : MonoBehaviour
{
	public UISprite Survivor_Container_Bg;
	public UIGrid grid;
	public UIWidget Left_View;
	public float scaleMult;

	private Transform gridTransform => grid.transform;
	private int currentColumns = 3;

	public void ZoomSurvivorPanel(UIScrollBar bar)
	{
		var value1 = MyTools.InterpolateRange(-228, -293, 0, 1, bar.value);
		Survivor_Container_Bg.leftAnchor.absolute = (int)value1;
		var value2 = MyTools.InterpolateRange(472, 540, 0, 1, bar.value);
		Survivor_Container_Bg.rightAnchor.absolute = (int)value2;

		float value3;
		if (bar.value < .1f)
		{
			currentColumns = 3;
			value3 = MyTools.InterpolateRange(1f, .92f, 0, 1, bar.value);
		}
		else
		{
			currentColumns = 4;
			value3 = MyTools.InterpolateRange(scaleMult, .92f, 0, 1, bar.value);
		}

		if (grid.maxPerLine != currentColumns)
		{
			grid.maxPerLine = currentColumns;
			grid.repositionNow = true;
		}

		gridTransform.localScale = value3 * Vector3.one;

		var value4 = MyTools.InterpolateRange(30, -36, 0, 1, bar.value);
		Left_View.leftAnchor.absolute = (int)value4;
		var value5 = MyTools.InterpolateRange(-240, -306, 0, 1, bar.value);
		Left_View.rightAnchor.absolute = (int)value5;
	}
}
