using System;
using UnityEngine;

public class CoverScrollView : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Center card zoom.")]
	private float centerCardZoom;

	[SerializeField]
	[Tooltip("Side card zoom.")]
	private float sideCardZoom;

	[SerializeField]
	[Tooltip("Side card fading.")]
	private int sideCardFading;

	[SerializeField]
	[Tooltip("Card rotation & zoom damping.")]
	private int damping;

	[SerializeField]
	[Tooltip("How much space is there between the cards. Put 100 and they will be on top of each other. Put 0 and there will be no change.")]
	private int CardWidthPercentageOffset;

	[SerializeField]
	[Tooltip("Z position of the target object that the cards are trying to face.")]
	private float targetObjectZ;

	[SerializeField]
	[Tooltip("Define the width of the center part where the cards doesn't rotate at all. The bigger it is , the longer it will take before the cart starts rotating.")]
	private float centerNoRotationWidth;

	private Vector3 targetPositionOffset;

	private UIScrollView scrollView;

	private void Start()
	{
		scrollView = GetComponent<UIScrollView>();
		targetPositionOffset = Vector3.zero;
		float num = base.transform.GetChild(1).localPosition.x - base.transform.GetChild(0).localPosition.x;
		int i = 1;
		for (int childCount = base.transform.childCount; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			Vector3 localPosition = child.localPosition;
			localPosition.x -= (float)i * num * (float)CardWidthPercentageOffset / 100f;
			child.localPosition = localPosition;
		}
	}

	private void Update()
	{
		targetPositionOffset.z = targetObjectZ;
		Vector3[] worldCorners = scrollView.panel.worldCorners;
		Vector3 vector = (worldCorners[2] + worldCorners[0]) * 0.5f;
		Vector3 vector2 = vector + targetPositionOffset;
		int i = 0;
		for (int childCount = base.transform.childCount; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (child.gameObject.activeInHierarchy)
			{
				Quaternion b = Quaternion.identity;
				Vector3 b2;
				if (Math.Abs(child.position.x - vector.x) < centerNoRotationWidth)
				{
					b2 = Vector3.one * centerCardZoom;
				}
				else
				{
					b2 = Vector3.one * sideCardZoom;
					b = Quaternion.LookRotation(vector2 - child.position);
					b.y = 0f - b.y;
				}
				child.rotation = Quaternion.Slerp(child.rotation, b, Time.deltaTime * (float)damping);
				child.localScale = Vector3.Lerp(child.localScale, b2, Time.deltaTime * (float)damping);
			}
		}
	}
}
