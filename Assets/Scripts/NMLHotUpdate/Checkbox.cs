using UnityEngine;

public class Checkbox : MonoBehaviour
{
	[SerializeField]
	private GameObject checkboxObject;

	[SerializeField]
	private UIButton button;

	public bool IsOn => checkboxObject.activeSelf;

	private void Start()
	{
		button.onClick.Add(new EventDelegate(delegate
		{
			checkboxObject.SetActive(!checkboxObject.activeSelf);
		}));
	}
}
