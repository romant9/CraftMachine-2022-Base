using UnityEngine;

public class ActorInfoPopup : HUDElementFollowTarget
{
	public UISprite IconSprite;

	public UILabel NameLabel;

	public UILabel LevelLabel;

	public UILabel AlertnessLabel;

	public UILabel DescriptionLabel;

	public UILabel HealthLabel;

	public UISprite contentContainer;

	public UISprite triangleSprite;

	private Vector3 originalTrianglePos;

	private Vector3 originalContentPos;

	public void Awake()
	{
		originalTrianglePos = triangleSprite.transform.localPosition;
		originalContentPos = contentContainer.transform.localPosition;
	}

	public void SetText(string classIconName, string name, string level, string alertness, string description, int currentHealth, int maxHealth)
	{
		IconSprite.spriteName = classIconName;
		NameLabel.text = name;
		LevelLabel.text = level;
		AlertnessLabel.text = alertness;
		DescriptionLabel.text = description;
		HealthLabel.text = currentHealth + "/" + maxHealth;
	}

	public void OnEnable()
	{
		if (UICamera.currentCamera != null)
		{
			Vector3 localPosition = triangleSprite.transform.localPosition;
			Vector3 localPosition2 = contentContainer.transform.localPosition;
			Vector3 vector = UICamera.currentCamera.WorldToScreenPoint(base.transform.position) + originalTrianglePos;
			float num = (float)(triangleSprite.height + contentContainer.height) - 1f;
			if (vector.y + num > (float)Screen.height)
			{
				bool flag = vector.x > (float)Screen.width / 2f;
				float num2 = (float)IconSprite.width / 2f + (float)triangleSprite.width;
				triangleSprite.flip = UIBasicSprite.Flip.Vertically;
				localPosition.y = 0f;
				localPosition2.y = 0f - num;
				localPosition2.x = (flag ? (0f - num2) : num2);
			}
			else
			{
				triangleSprite.flip = UIBasicSprite.Flip.Nothing;
				localPosition = originalTrianglePos;
				localPosition2 = originalContentPos;
			}
			triangleSprite.transform.localPosition = localPosition;
			contentContainer.transform.localPosition = localPosition2;
		}
	}
}
