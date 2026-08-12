using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InvBaseItem;

public class SmallWheelSyncController : MonoBehaviour
{
	[Serializable]
	public class WheelSettings
	{
		[Header("旋转目标")]
		public Transform wheelTransform;

		public Transform pointerTransform;

		[Header("旋转动画")]
		public float spinDuration = 5f;

		public int minSpins = 3;

		public int maxSpins = 6;

		public bool randomizeSpins = true;

		public int fixedSpins = 4;

		[Header("速度曲线")]
		public AnimationCurve speedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[Header("过冲效果")]
		public float overshootAngle = 15f;

		public float overshootDuration = 0.3f;

		public float returnDuration = 0.5f;

		public AnimationCurve overshootCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		public AnimationCurve returnCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[Header("完成延迟")]
		public float completeDelay;
	}

	public delegate void SpinCompleteCallback(int prizeIndex);

	[Header("转盘设置")]
	public WheelSettings wheelSettings;

	public GameObject effect;

	private bool isSpinning;

	private Quaternion originalWheelRotation;

	private Vector3 originalPointerPosition;

	private List<float> prizeCenterAngles = new List<float>();

	private List<float> prizeStartAngles = new List<float>();

	private List<float> prizeEndAngles = new List<float>();

	private SpinCompleteCallback currentCallback;

	private void Start()
	{
		InitializeWheel();
	}

	public void InitializeWheel()
	{
		if (wheelSettings.wheelTransform != null)
		{
			originalWheelRotation = wheelSettings.wheelTransform.localRotation;
		}
		if (wheelSettings.pointerTransform != null)
		{
			originalPointerPosition = wheelSettings.pointerTransform.localPosition;
			wheelSettings.pointerTransform.localEulerAngles = Vector3.zero;
		}
		CalculatePrizeAnglesFor6Prizes();
	}

	private void CalculatePrizeAnglesFor6Prizes()
	{
		prizeCenterAngles.Clear();
		prizeStartAngles.Clear();
		prizeEndAngles.Clear();
		List<float> list = new List<float> { 90f, 30f, 90f, 30f, 90f, 30f };
		float[] array = new float[6] { 0f, 300f, 240f, 180f, 120f, 60f };
		for (int i = 0; i < list.Count; i++)
		{
			float num = list[i];
			float num2 = array[i];
			float item = NormalizeAngle(num2 - num / 2f);
			float item2 = NormalizeAngle(num2 + num / 2f);
			prizeStartAngles.Add(item);
			prizeEndAngles.Add(item2);
			prizeCenterAngles.Add(num2);
		}
	}

	public void SpinToPrize(int prizeIndex, SpinCompleteCallback callback = null, bool isQuick = false)
	{
		if (!isSpinning && prizeIndex >= 0 && prizeIndex < prizeCenterAngles.Count)
		{
			int spinCount = (wheelSettings.randomizeSpins ? UnityEngine.Random.Range(wheelSettings.minSpins, wheelSettings.maxSpins + 1) : wheelSettings.fixedSpins);
			currentCallback = callback;
			isSpinning = true;
			StartCoroutine(SpinCoroutine(prizeIndex, spinCount, isQuick));
		}
	}

	private IEnumerator SpinCoroutine(int prizeIndex, int spinCount, bool isQuick = false)
	{
		float targetAngle = CalculateTargetAngle(prizeIndex, spinCount);
		if (isQuick)
		{
			float normalizedTargetAngle = NormalizeAngle(targetAngle);
			wheelSettings.wheelTransform.localEulerAngles = new Vector3(0f, 0f, normalizedTargetAngle);
			OnSpinComplete(prizeIndex);
			yield break;
		}
		yield return StartCoroutine(RotateWheel(targetAngle));
		if (wheelSettings.overshootAngle > 0f)
		{
			yield return StartCoroutine(OvershootEffect(targetAngle));
		}
		if (wheelSettings.completeDelay > 0f && !OfflineManager.IsNoEffects)
		{
			if (effect != null)
			{
				yield return new WaitForSeconds(0.2f);
				Helpers.GameObjectSetActive(effect, value: true);
			}
			yield return new WaitForSeconds(wheelSettings.completeDelay);
			Helpers.GameObjectSetActive(effect, value: false);
		}
		OnSpinComplete(prizeIndex);
	}

	private float CalculateTargetAngle(int prizeIndex, int spinCount)
	{
		float num = prizeCenterAngles[prizeIndex];
		float num2 = (float)spinCount * 360f;
		num2 += 360f - num;
		return wheelSettings.wheelTransform.localEulerAngles.z + num2;
	}

	private IEnumerator RotateWheel(float targetAngle)
	{
		if (!(wheelSettings.wheelTransform == null))
		{
			float startAngle = wheelSettings.wheelTransform.localEulerAngles.z;
			float normalizedTargetAngle = NormalizeAngle(targetAngle);
			float totalRotation = targetAngle - startAngle;
			if (totalRotation < 0f)
			{
				totalRotation += 360f * Mathf.Ceil(Mathf.Abs(totalRotation) / 360f);
			}
			float elapsedTime = 0f;
			while (elapsedTime < wheelSettings.spinDuration)
			{
				elapsedTime += Time.deltaTime;
				float time = Mathf.Clamp01(elapsedTime / wheelSettings.spinDuration);
				float num = wheelSettings.speedCurve.Evaluate(time);
				float z = startAngle + totalRotation * num;
				wheelSettings.wheelTransform.localEulerAngles = new Vector3(0f, 0f, z);
				yield return null;
			}
			wheelSettings.wheelTransform.localEulerAngles = new Vector3(0f, 0f, normalizedTargetAngle);
		}
	}

	private IEnumerator OvershootEffect(float targetAngle)
	{
		if (!(wheelSettings.wheelTransform == null))
		{
			float currentAngle = wheelSettings.wheelTransform.localEulerAngles.z;
			float normalizedTargetAngle = NormalizeAngle(targetAngle);
			float overshootTargetAngle = NormalizeAngle(currentAngle + wheelSettings.overshootAngle);
			float elapsedTime = 0f;
			while (elapsedTime < wheelSettings.overshootDuration)
			{
				elapsedTime += Time.deltaTime;
				float time = Mathf.Clamp01(elapsedTime / wheelSettings.overshootDuration);
				float t = wheelSettings.overshootCurve.Evaluate(time);
				float z = Mathf.Lerp(currentAngle, overshootTargetAngle, t);
				wheelSettings.wheelTransform.localEulerAngles = new Vector3(0f, 0f, z);
				yield return null;
			}
			wheelSettings.wheelTransform.localEulerAngles = new Vector3(0f, 0f, overshootTargetAngle);
			float overshootStartAngle = overshootTargetAngle;
			elapsedTime = 0f;
			while (elapsedTime < wheelSettings.returnDuration)
			{
				elapsedTime += Time.deltaTime;
				float time2 = Mathf.Clamp01(elapsedTime / wheelSettings.returnDuration);
				float t2 = wheelSettings.returnCurve.Evaluate(time2);
				float z2 = Mathf.Lerp(overshootStartAngle, normalizedTargetAngle, t2);
				wheelSettings.wheelTransform.localEulerAngles = new Vector3(0f, 0f, z2);
				yield return null;
			}
			wheelSettings.wheelTransform.localEulerAngles = new Vector3(0f, 0f, normalizedTargetAngle);
		}
	}

	private void OnSpinComplete(int prizeIndex)
	{
		isSpinning = false;
		if (currentCallback != null)
		{
			currentCallback(prizeIndex);
			currentCallback = null;
		}
	}

	private float NormalizeAngle(float angle)
	{
		while (angle < 0f)
		{
			angle += 360f;
		}
		while (angle >= 360f)
		{
			angle -= 360f;
		}
		return angle;
	}

	public void ResetWheel(bool forced = false)
	{
		if (OfflineManager.IsLoadDataManager && !forced) return;
		if (!isSpinning)
		{
			if (wheelSettings.wheelTransform != null)
			{
				wheelSettings.wheelTransform.localRotation = originalWheelRotation;
			}
			if (wheelSettings.pointerTransform != null)
			{
				wheelSettings.pointerTransform.localPosition = originalPointerPosition;
				wheelSettings.pointerTransform.localEulerAngles = Vector3.zero;
			}
		}
	}

	public bool IsSpinning()
	{
		return isSpinning;
	}

	public int GetCurrentPrizeIndex()
	{
		if (wheelSettings.wheelTransform == null)
		{
			return -1;
		}
		float z = wheelSettings.wheelTransform.localEulerAngles.z;
		return GetPrizeIndexByWheelAngle(z);
	}

	public int GetPrizeIndexByWheelAngle(float wheelAngle)
	{
		float num = NormalizeAngle(wheelAngle);
		float num2 = NormalizeAngle(360f - num);
		for (int i = 0; i < prizeStartAngles.Count; i++)
		{
			float num3 = prizeStartAngles[i];
			float num4 = prizeEndAngles[i];
			if ((!(num3 > num4)) ? (num2 >= num3 && num2 <= num4) : (num2 >= num3 || num2 <= num4))
			{
				return i;
			}
		}
		return -1;
	}
}
