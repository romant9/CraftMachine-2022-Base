using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ShaderProgressBar : MonoBehaviour
{
	[SerializeField]
	private float startDelay;

	[SerializeField]
	private float duration;

	[SerializeField]
	private AnimationCurve fillCurve;

	[SerializeField]
	private bool startAutomatically;

	private UIWidget uiWidget;

	private Coroutine fillRoutine;

	private int fillProgressParameterID;

	private float currentFillPercentage;

	private float currentProgress;

	private void Awake()
	{
		fillProgressParameterID = Shader.PropertyToID("_Fillpercentage");
		uiWidget = GetComponent<UIWidget>();
		uiWidget.onRender = OnRenderWidget;
	}

	private void OnDisable()
	{
		StopFill();
	}

	public void StartFill(float to)
	{
		if (fillRoutine != null)
		{
			StopCoroutine(fillRoutine);
		}
		fillRoutine = StartCoroutine(FillCoroutine(to));
	}

	public void StopFill()
	{
		if (fillRoutine != null)
		{
			StopCoroutine(fillRoutine);
		}
		fillRoutine = null;
		currentFillPercentage = 0f;
		currentProgress = 0f;
	}

	private IEnumerator FillCoroutine(float to)
	{
		yield return new WaitForSeconds(startDelay);
		if (!(duration <= 0f))
		{
			float overAllDuration = duration * to;
			while (currentProgress < overAllDuration)
			{
				currentProgress += Time.deltaTime;
				currentFillPercentage = fillCurve.Evaluate(currentProgress / overAllDuration * to);
				yield return new WaitForEndOfFrame();
			}
			yield return null;
		}
	}

	private void OnRenderWidget(Material material)
	{
		material.SetFloat(fillProgressParameterID, currentFillPercentage);
	}
}
