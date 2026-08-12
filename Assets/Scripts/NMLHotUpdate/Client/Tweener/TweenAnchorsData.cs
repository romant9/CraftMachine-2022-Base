using System;
using UnityEngine;

namespace Client.Tweener
{
	[Serializable]
	public class TweenAnchorsData
	{
		public float left;

		public float right;

		public float bottom;

		public float top;

		public float alpha;

		public void GetAsVector4s(out Vector4 positionData, out Vector4 alphaData)
		{
			positionData.x = left;
			positionData.y = right;
			positionData.z = bottom;
			positionData.w = top;
			alphaData.x = alpha;
			alphaData.y = alpha;
			alphaData.z = alpha;
			alphaData.w = alpha;
		}
	}
}
