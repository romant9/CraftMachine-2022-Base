using System;
using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class LargeWheelSyncController : MonoBehaviour
{
	[Serializable]
	public class SyncRotateObject
	{
		public Transform targetTransform;

		public float speedMultiplier = 1f;

		public float direction = 1f;

		public bool useLocalRotation = true;

		public Vector3 rotateAxis = Vector3.forward;

		[Header("延迟效果")]
		public float rotationDelay;

		public AnimationCurve delayCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	}

	[Serializable]
	public class SpinAnimationSettings
	{
		[Header("基础设置")]
		public float spinDuration = 5f;

		[Header("圈数控制")]
		public int minSpins = 3;

		public int maxSpins = 6;

		public bool useFixedSpins;

		public int fixedSpins = 4;

		[Header("速度控制")]
		public bool useEaseCurve = true;

		public AnimationCurve speedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[Header("完成延迟")]
		public float completeDelay;

		[Header("过冲效果")]
		public float overshootAngle = 15f;

		public float overshootDuration = 0.3f;

		public float returnDuration = 0.5f;

		public AnimationCurve overshootCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		public AnimationCurve returnCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
	}

	public delegate void WheelCompleteCallback(int prizeId);

	[Header("转盘设置")]
	public Transform wheelTransform;

	public Transform pointerTransform;

	[Header("动画设置")]
	public SpinAnimationSettings animationSettings;

	[Header("同步旋转物体")]
	public List<SyncRotateObject> syncObjects = new List<SyncRotateObject>();

	public GameObject Effect;

	private bool isSpinning;

	private float[] prizeAngles;

	private WheelCompleteCallback currentCallback;

	private Vector3 originalPointerPosition;

	private Dictionary<Transform, Quaternion> originalSyncRotations = new Dictionary<Transform, Quaternion>();

	private List<RouletteDefinition> rouletteDefinitions;

	private string _unlockSound = "camp/roulette_unlock";

	public void InitializeWheel(List<RouletteDefinition> definitions)
	{
		rouletteDefinitions = definitions;
		if (pointerTransform != null)
		{
			originalPointerPosition = pointerTransform.localPosition;
		}
		foreach (SyncRotateObject syncObject in syncObjects)
		{
			if (syncObject.targetTransform != null)
			{
				originalSyncRotations[syncObject.targetTransform] = (syncObject.useLocalRotation ? syncObject.targetTransform.localRotation : syncObject.targetTransform.rotation);
			}
		}
		prizeAngles = new float[rouletteDefinitions.Count];
		float num = 360f / (float)rouletteDefinitions.Count;
		for (int i = 0; i < rouletteDefinitions.Count; i++)
		{
			prizeAngles[i] = (float)i * num;
		}
	}

	public void StartSpin(int slot, WheelCompleteCallback callback = null, bool isQuick = false)
	{
		int spinCount = (animationSettings.useFixedSpins ? animationSettings.fixedSpins : UnityEngine.Random.Range(animationSettings.minSpins, animationSettings.maxSpins + 1));
		StartSpin(slot, spinCount, callback, isQuick);
	}

	public void StartSpin(int slot, int spinCount, WheelCompleteCallback callback = null, bool isQuick = false)
	{
		if (!isSpinning && slot >= 0 && slot < rouletteDefinitions.Count)
		{
			if (spinCount <= 0)
			{
				spinCount = animationSettings.minSpins;
			}
			currentCallback = callback;
			isSpinning = true;
			StartCoroutine(SpinToPrizeWithSync(slot, spinCount, isQuick));
		}
	}

	private IEnumerator SpinToPrizeWithSync(int slot, int spinCount, bool isQuick = false)
	{
		float targetAngle = CalculateTargetAngle(slot, spinCount);
		if (isQuick)
		{
			pointerTransform.localEulerAngles = new Vector3(0f, 0f, targetAngle);
			OnSpinComplete(slot);
			yield break;
		}
		yield return StartCoroutine(RotateWithEaseCurve(targetAngle));
		if (animationSettings.overshootAngle > 0f)
		{
			yield return StartCoroutine(OvershootEffect(targetAngle));
		}
		if (animationSettings.completeDelay > 0f && !OfflineManager.IsNoEffects)
		{
			if (Effect != null)
			{
				yield return new WaitForSeconds(0.2f);
				Helpers.GameObjectSetActive(Effect, value: true);
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(_unlockSound);
			yield return new WaitForSeconds(animationSettings.completeDelay);
			Helpers.GameObjectSetActive(Effect, value: false);
		}
		OnSpinComplete(slot);
	}

	private IEnumerator RotateWithEaseCurve(float targetAngle)
	{
		float startAngle = ((pointerTransform != null) ? pointerTransform.localEulerAngles.z : 0f);
		float totalRotation = targetAngle - startAngle;
		float startTime = Time.time;
		float elapsedTime = 0f;
		Dictionary<Transform, float> syncStartRotations = new Dictionary<Transform, float>();
		foreach (SyncRotateObject syncObject in syncObjects)
		{
			if (syncObject.targetTransform != null)
			{
				if (syncObject.useLocalRotation)
				{
					syncStartRotations[syncObject.targetTransform] = syncObject.targetTransform.localEulerAngles.z;
				}
				else
				{
					syncStartRotations[syncObject.targetTransform] = syncObject.targetTransform.eulerAngles.z;
				}
			}
		}
		while (elapsedTime < animationSettings.spinDuration)
		{
			elapsedTime = Time.time - startTime;
			float num = elapsedTime / animationSettings.spinDuration;
			float value = num;
			if (animationSettings.useEaseCurve)
			{
				value = animationSettings.speedCurve.Evaluate(num);
			}
			value = Mathf.Clamp01(value);
			float z = startAngle + totalRotation * value;
			if (pointerTransform != null)
			{
				pointerTransform.localEulerAngles = new Vector3(0f, 0f, z);
			}
			foreach (SyncRotateObject syncObject2 in syncObjects)
			{
				if (syncObject2.targetTransform != null && syncStartRotations.ContainsKey(syncObject2.targetTransform))
				{
					float time = Mathf.Max(0f, num - syncObject2.rotationDelay);
					float value2 = syncObject2.delayCurve.Evaluate(time);
					value2 = Mathf.Clamp01(value2);
					if (animationSettings.useEaseCurve)
					{
						value2 = animationSettings.speedCurve.Evaluate(value2);
					}
					float num2 = totalRotation * value2 * syncObject2.speedMultiplier * syncObject2.direction;
					float z2 = syncStartRotations[syncObject2.targetTransform] + num2;
					if (syncObject2.useLocalRotation)
					{
						Vector3 localEulerAngles = syncObject2.targetTransform.localEulerAngles;
						syncObject2.targetTransform.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, z2);
					}
					else
					{
						Vector3 eulerAngles = syncObject2.targetTransform.eulerAngles;
						syncObject2.targetTransform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, z2);
					}
				}
			}
			yield return null;
		}
		if (animationSettings.overshootAngle <= 0f && pointerTransform != null)
		{
			pointerTransform.localEulerAngles = new Vector3(0f, 0f, targetAngle);
		}
		if (!(animationSettings.overshootAngle <= 0f))
		{
			yield break;
		}
		foreach (SyncRotateObject syncObject3 in syncObjects)
		{
			if (syncObject3.targetTransform != null && syncStartRotations.ContainsKey(syncObject3.targetTransform))
			{
				float z3 = syncStartRotations[syncObject3.targetTransform] + totalRotation * syncObject3.speedMultiplier * syncObject3.direction;
				if (syncObject3.useLocalRotation)
				{
					Vector3 localEulerAngles2 = syncObject3.targetTransform.localEulerAngles;
					syncObject3.targetTransform.localEulerAngles = new Vector3(localEulerAngles2.x, localEulerAngles2.y, z3);
				}
				else
				{
					Vector3 eulerAngles2 = syncObject3.targetTransform.eulerAngles;
					syncObject3.targetTransform.eulerAngles = new Vector3(eulerAngles2.x, eulerAngles2.y, z3);
				}
			}
		}
	}

	private IEnumerator OvershootEffect(float targetAngle)
	{
		if (pointerTransform == null)
		{
			yield break;
		}
		float currentAngle = pointerTransform.localEulerAngles.z;
		float normalizedTargetAngle = NormalizeAngle(targetAngle);
		float overshootTargetAngle = ((!(Mathf.Abs(normalizedTargetAngle) < 0.1f) && !(Mathf.Abs(normalizedTargetAngle - 360f) < 0.1f)) ? (currentAngle - animationSettings.overshootAngle) : (currentAngle - animationSettings.overshootAngle));
		overshootTargetAngle = NormalizeAngle(overshootTargetAngle);
		Dictionary<Transform, float> syncStartAngles = new Dictionary<Transform, float>();
		foreach (SyncRotateObject syncObject in syncObjects)
		{
			if (syncObject.targetTransform != null)
			{
				float angle = (syncObject.useLocalRotation ? syncObject.targetTransform.localEulerAngles.z : syncObject.targetTransform.eulerAngles.z);
				syncStartAngles[syncObject.targetTransform] = NormalizeAngle(angle);
			}
		}
		float elapsedTime = 0f;
		while (elapsedTime < animationSettings.overshootDuration)
		{
			elapsedTime += Time.deltaTime;
			float time = Mathf.Clamp01(elapsedTime / animationSettings.overshootDuration);
			float t = animationSettings.overshootCurve.Evaluate(time);
			float num = Mathf.Lerp(currentAngle, overshootTargetAngle, t);
			pointerTransform.localEulerAngles = new Vector3(0f, 0f, num);
			float num2 = num - currentAngle;
			foreach (SyncRotateObject syncObject2 in syncObjects)
			{
				if (syncObject2.targetTransform != null && syncStartAngles.ContainsKey(syncObject2.targetTransform))
				{
					float num3 = num2 * syncObject2.speedMultiplier * syncObject2.direction;
					float angle2 = syncStartAngles[syncObject2.targetTransform] + num3;
					angle2 = NormalizeAngle(angle2);
					if (syncObject2.useLocalRotation)
					{
						Vector3 localEulerAngles = syncObject2.targetTransform.localEulerAngles;
						syncObject2.targetTransform.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, angle2);
					}
					else
					{
						Vector3 eulerAngles = syncObject2.targetTransform.eulerAngles;
						syncObject2.targetTransform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, angle2);
					}
				}
			}
			yield return null;
		}
		pointerTransform.localEulerAngles = new Vector3(0f, 0f, overshootTargetAngle);
		Dictionary<Transform, float> syncOvershootAngles = new Dictionary<Transform, float>();
		foreach (SyncRotateObject syncObject3 in syncObjects)
		{
			if (syncObject3.targetTransform != null)
			{
				float angle3 = (syncObject3.useLocalRotation ? syncObject3.targetTransform.localEulerAngles.z : syncObject3.targetTransform.eulerAngles.z);
				syncOvershootAngles[syncObject3.targetTransform] = NormalizeAngle(angle3);
			}
		}
		elapsedTime = 0f;
		while (elapsedTime < animationSettings.returnDuration)
		{
			elapsedTime += Time.deltaTime;
			float time2 = Mathf.Clamp01(elapsedTime / animationSettings.returnDuration);
			float t2 = animationSettings.returnCurve.Evaluate(time2);
			float num4 = Mathf.Lerp(overshootTargetAngle, normalizedTargetAngle, t2);
			pointerTransform.localEulerAngles = new Vector3(0f, 0f, num4);
			float num5 = num4 - overshootTargetAngle;
			foreach (SyncRotateObject syncObject4 in syncObjects)
			{
				if (syncObject4.targetTransform != null && syncOvershootAngles.ContainsKey(syncObject4.targetTransform) && syncStartAngles.ContainsKey(syncObject4.targetTransform))
				{
					float num6 = num5 * syncObject4.speedMultiplier * syncObject4.direction;
					float angle4 = syncOvershootAngles[syncObject4.targetTransform] + num6;
					angle4 = NormalizeAngle(angle4);
					if (syncObject4.useLocalRotation)
					{
						Vector3 localEulerAngles2 = syncObject4.targetTransform.localEulerAngles;
						syncObject4.targetTransform.localEulerAngles = new Vector3(localEulerAngles2.x, localEulerAngles2.y, angle4);
					}
					else
					{
						Vector3 eulerAngles2 = syncObject4.targetTransform.eulerAngles;
						syncObject4.targetTransform.eulerAngles = new Vector3(eulerAngles2.x, eulerAngles2.y, angle4);
					}
				}
			}
			yield return null;
		}
		pointerTransform.localEulerAngles = new Vector3(0f, 0f, normalizedTargetAngle);
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

	private float CalculateTargetAngle(int prizeId, int spinCount)
	{
		float num = prizeAngles[prizeId];
		if (Mathf.Abs(num) < 0.001f)
		{
			num = 0.1f;
		}
		return 0f - (360f * (float)spinCount + num);
	}

	private void OnSpinComplete(int slot)
	{
		isSpinning = false;
		if (currentCallback != null)
		{
			currentCallback(slot);
			currentCallback = null;
		}
	}

	public void ResetWheel(bool forced = false)
	{
		if (OfflineManager.IsLoadDataManager && !forced) return;
		if (isSpinning)
		{
			return;
		}
		if (pointerTransform != null)
		{
			pointerTransform.localEulerAngles = Vector3.zero;
			pointerTransform.localPosition = originalPointerPosition;
		}
		foreach (SyncRotateObject syncObject in syncObjects)
		{
			if (syncObject.targetTransform != null && originalSyncRotations.ContainsKey(syncObject.targetTransform))
			{
				if (syncObject.useLocalRotation)
				{
					syncObject.targetTransform.localRotation = originalSyncRotations[syncObject.targetTransform];
				}
				else
				{
					syncObject.targetTransform.rotation = originalSyncRotations[syncObject.targetTransform];
				}
			}
		}
		currentCallback = null;
	}

	public int GetCurrentPrizeIndex()
	{
		if (rouletteDefinitions == null || rouletteDefinitions.Count == 0)
		{
			return -1;
		}
		if (pointerTransform == null)
		{
			return -1;
		}
		float z = pointerTransform.localEulerAngles.z;
		z = NormalizeAngle(z);
		int count = rouletteDefinitions.Count;
		float num = 360f / (float)count;
		float angle = 360f - z;
		angle = NormalizeAngle(angle);
		if (angle < num / 2f || angle >= 360f - num / 2f)
		{
			return 0;
		}
		return Mathf.Clamp(Mathf.FloorToInt((angle + num / 2f) / num), 0, count - 1);
	}
}
