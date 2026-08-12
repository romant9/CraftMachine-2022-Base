using UnityEngine;

public class CombatToggleButton : MonoBehaviour
{
	[Tooltip("Toggle highlight object.")]
	[SerializeField]
	private UISprite highlightObject;

	private bool selected;

	public bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
			if (highlightObject != null)
			{
				highlightObject.gameObject.SetActive(selected);
			}
		}
	}

	private void Awake()
	{
		Selected = false;
	}
}
