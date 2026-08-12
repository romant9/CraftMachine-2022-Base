using UnityEngine;

public class InteractiveObjectInfoPopup : HUDElementFollowTarget
{
	public UISprite IconSprite;

	public UILabel NameLabel;

	public UILabel DescriptionLabel;

	public UISprite contentContainer;

	public UISprite triangleSprite;

	private Vector3 originalTrianglePos;

	public void Awake()
	{
		originalTrianglePos = triangleSprite.transform.localPosition;
	}

	public void SetText(string iconName, string name, string description)
	{
		IconSprite.spriteName = iconName;
		NameLabel.text = name;
		DescriptionLabel.text = description;
	}

	public void OnEnable()
	{
		if (UICamera.currentCamera != null)
		{
			Vector3 localPosition = triangleSprite.transform.localPosition;
			Vector3 vector = UICamera.currentCamera.WorldToScreenPoint(base.transform.position) + originalTrianglePos;
			float num = (float)(triangleSprite.height + contentContainer.height) - 1f;
			Vector3 zero = Vector3.zero;
			if (vector.y + num > (float)Screen.height)
			{
				bool flag = vector.x > (float)Screen.width / 2f;
				float num2 = (float)IconSprite.width / 2f + (float)triangleSprite.width;
				triangleSprite.flip = UIBasicSprite.Flip.Vertically;
				localPosition.y = 0f;
				zero.y = 0f - num;
				zero.x = (flag ? (0f - num2) : num2);
			}
			else
			{
				triangleSprite.flip = UIBasicSprite.Flip.Nothing;
				localPosition = originalTrianglePos;
			}
			triangleSprite.transform.localPosition = localPosition;
			contentContainer.transform.localPosition = zero;
		}
	}
}
