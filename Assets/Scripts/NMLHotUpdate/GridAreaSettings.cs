using System;

[Serializable]
public class GridAreaSettings
{
	public enum FillType
	{
		None = 0,
		Inside = 1,
		Outside = 2
	}

	public FillType Fill = FillType.Inside;

	public float Curvature;

	public float Thickness;

	public float Smoothing;

	public float TextureScale = 1f;

	public float AreaBorderWidth = 0.51f;

	public float EdgeOffset;

	public float EdgeWidth = 0.4f;

	public float EdgeRandom = 0.4f;
}
