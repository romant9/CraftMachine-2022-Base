using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class UIShineEffect : MonoBehaviour
{
	[SerializeField]
	private float ShineStartDelay;

	[SerializeField]
	private float ShineWaitTime;

	[SerializeField]
	private float ShineDuration;

	[SerializeField]
	private int ShineRepeats;

	[SerializeField]
	private bool Loop;

	[SerializeField]
	private float ShineWidth;

	[SerializeField]
	private float ShineRotation;

	[SerializeField]
	private AnimationCurve MoveCurve;

	private float shinePosition;

	private Coroutine shineRoutine;

	private int shineLocationParameterID;

	private int shineWidthParameterID;

	private int shineRotationParameterID;

	private UIWidget uiwidget;

	private void Start()
	{
		shineLocationParameterID = Shader.PropertyToID("_ShineLocation");
		shineWidthParameterID = Shader.PropertyToID("_ShineWidth");
		shineRotationParameterID = Shader.PropertyToID("_Angle");
		uiwidget = GetComponent<UIWidget>();
		uiwidget.onRender = OnRenderWidget;
	}

	private void OnEnable()
	{
		StartShine(ShineStartDelay);
	}

	private void OnDisable()
	{
		StopShine();
	}

	public void StartShine(float delay)
	{
		if (shineRoutine != null)
		{
			StopCoroutine(shineRoutine);
		}
		shineRoutine = StartCoroutine(ShineCoroutine(delay));
	}

	public void StopShine()
	{
		if (shineRoutine != null)
		{
			StopCoroutine(shineRoutine);
		}
		shineRoutine = null;
	}

	private IEnumerator ShineCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (ShineDuration <= 0f)
		{
			yield break;
		}
		int count = (Loop ? 1 : ShineRepeats);
		float progress = 0f;
		while (count > 0)
		{
			yield return new WaitForSeconds(ShineWaitTime);
			count = (Loop ? 1 : (count - 1));
			while (progress < ShineDuration)
			{
				progress += Time.deltaTime;
				shinePosition = MoveCurve.Evaluate(progress / ShineDuration);
				yield return new WaitForEndOfFrame();
			}
			progress = 0f;
		}
		yield return null;
	}

	private void OnRenderWidget(Material material)
	{
		material.SetFloat(shineLocationParameterID, shinePosition);
		material.SetFloat(shineWidthParameterID, ShineWidth);
		material.SetFloat(shineRotationParameterID, ShineRotation * (MathF.PI / 180f));
	}
}
