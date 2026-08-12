using UnityEngine;

public class EquipmentBag : MonoBehaviour
{
	public delegate void OnBagOpenedCallback(EquipmentBag step);

	[Tooltip("Zip object.")]
	[SerializeField]
	private GameObject zipObject;

	[Tooltip("Object that will modify the mesh. Should be an invisible object.")]
	[SerializeField]
	private GameObject magnetObject;

	[Tooltip("Final zip dragposition.")]
	[SerializeField]
	private Transform finalZipDragPosition;

	[Tooltip("Zip position wheere he must scroll on its own.")]
	[SerializeField]
	private Transform finalZipPosition;

	private float animationTime;

	private float ANIMATION_DURATION = 1f;

	private Vector3 initalZipDragPosition;

	public event OnBagOpenedCallback OnBagOpened;

	public void Start()
	{
		if (magnetObject != null)
		{
			initalZipDragPosition = magnetObject.transform.position;
		}
		animationTime = -1f;
	}

	private void Update()
	{
		if (!(animationTime > -1f))
		{
			return;
		}
		animationTime += Time.deltaTime;
		animationTime = Mathf.Min(animationTime, ANIMATION_DURATION);
		float num = animationTime / ANIMATION_DURATION;
		if (magnetObject != null)
		{
			magnetObject.transform.position = initalZipDragPosition + (finalZipPosition.position - initalZipDragPosition) * num;
		}
		Vector3 vector = magnetObject.transform.position - initalZipDragPosition;
		Vector3 vector2 = finalZipDragPosition.position - initalZipDragPosition;
		Vector3 normalized = vector2.normalized;
		float magnitude = vector2.magnitude;
		float magnitude2 = vector.magnitude;
		float num2 = Mathf.Min(magnitude, magnitude2);
		zipObject.transform.position = initalZipDragPosition + normalized * num2;
		if (animationTime >= ANIMATION_DURATION)
		{
			if (this.OnBagOpened != null)
			{
				this.OnBagOpened(this);
			}
			base.gameObject.SetActive(value: false);
		}
	}

	public void OnClick(GameObject buttonObject)
	{
		animationTime = 0f;
		buttonObject.SetActive(value: false);
	}
}
