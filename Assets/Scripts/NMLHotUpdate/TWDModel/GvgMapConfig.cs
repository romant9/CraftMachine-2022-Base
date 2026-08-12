using System;

namespace TWDModel
{
	[Serializable]
	public class GvgMapConfig
	{
		public FixedPoint GridcCellSizeX;

		public FixedPoint GridcCellSizeY;

		public FixedPoint CameraStartZ;

		public float CameraStartX;

		public float CameraStartY;

		public float CameraStartZoom;

		public float MinZoom;

		public float MaxZoom;

		public float CameraMoveSpeed;

		public float PinchZoomSpeed;
	}
}
