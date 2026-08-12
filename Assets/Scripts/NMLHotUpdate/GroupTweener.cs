using System.Collections;
using UnityEngine;

public class GroupTweener : MonoBehaviour
{
	[SerializeField]
	private int animationFowardDelay;

	[SerializeField]
	private int animationReverseDelay;

	[SerializeField]
	private GameObject[] animatedObjects;

	private int currentAnimationIndex;

	private void OnEnable()
	{
		currentAnimationIndex = 0;
		GameObject[] array = animatedObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		StartCoroutine(AnimateCurrentObject());
	}

	private IEnumerator AnimateCurrentObject()
	{
		while (base.gameObject.activeSelf)
		{
			if (currentAnimationIndex > animatedObjects.Length - 1)
			{
				currentAnimationIndex = 0;
			}
			GameObject child = animatedObjects[currentAnimationIndex];
			child.gameObject.SetActive(value: true);
			UITweener[] tweens = child.GetComponents<UITweener>();
			UITweener[] array = tweens;
			foreach (UITweener uITweener in array)
			{
				if (uITweener.style == UITweener.Style.Once)
				{
					uITweener.PlayForward();
				}
			}
			yield return new WaitForSeconds(animationFowardDelay);
			array = tweens;
			foreach (UITweener uITweener2 in array)
			{
				if (uITweener2.style == UITweener.Style.Once)
				{
					uITweener2.PlayReverse();
				}
			}
			yield return new WaitForSeconds(animationReverseDelay);
			currentAnimationIndex++;
			child.gameObject.SetActive(value: false);
		}
		yield return null;
	}
}
