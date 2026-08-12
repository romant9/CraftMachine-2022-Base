using UnityEngine;

public class EnemyInfoItem : MonoBehaviour
{
	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel description;

	public void SetVisuals(string iconName, string title, string description)
	{
		classIcon.spriteName = iconName;
		this.title.text = title;
		this.description.text = description;
	}
}
