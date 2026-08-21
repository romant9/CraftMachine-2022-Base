using System;
using TWDModel;
using UnityEngine;

public class WalkerAnimationController : CharacterAnimationController
{
	private static int MeleeParamNameHash = Animator.StringToHash("Melee");

	private static int MeleeStrengthParamNameHash = Animator.StringToHash("MeleeStrength");

	private static int MeleeVariationParamNameHash = Animator.StringToHash("MeleeVariation");

	private static int DormantParamNameHash = Animator.StringToHash("Dormant");

	private static int CrawlParamNameHash = Animator.StringToHash("Crawl");

	private static int AlertnessParamNameHash = Animator.StringToHash("Alertness");

	private static int IdleVariationParamNameHash = Animator.StringToHash("IdleVariation");

	private static int TransformParamNameHash = Animator.StringToHash("Transform");

	private static int EnterStunParamNameHash = Animator.StringToHash("Base Layer.EnterStun");

	private static int ExitStunParamNameHash = Animator.StringToHash("Base Layer.ExitStun");

	private static int EnterEatingParamNameHash = Animator.StringToHash("Base Layer.EnterEating");

	private static int ExitEatingParamNameHash = Animator.StringToHash("Base Layer.ExitEating");

	[Tooltip("The override animation controller that will enable the animations of this specific walker type.")]
	public RuntimeAnimatorController AnimationOverrides;

	public Alertness InitialAlertness;

	public float AlertnessInterpSpeed = 3f;

	private bool stunned;

	private bool eatingLure;

	private bool staggered;

	private float idleSpeed;

	public Alertness Alertness { get; set; }

	public event Action OnTransformEffectHandler;

	public event Action OnTransformHandler;

	public override void UseWeapon(bool criticalDamage, bool useFenceAttack, bool useChargeAttack)
	{
		base.UseWeapon(criticalDamage, useFenceAttack: false, useChargeAttack: false);
		float value = (int)(UnityEngine.Random.value * 1000f) % 4;
		float value2 = (criticalDamage ? 1f : 0f);
		base.Animator.SetFloat(MeleeVariationParamNameHash, value);
		base.Animator.SetTrigger(MeleeParamNameHash);
		base.Animator.SetFloat(MeleeStrengthParamNameHash, value2);
	}

	private void SetAlertness(string targetState)
	{
		Alertness alertness = (Alertness)Enum.Parse(typeof(Alertness), targetState);
		Alertness = alertness;
		base.Animator.SetFloat(AlertnessParamNameHash, GetAlertness(Alertness));
	}

	public void SetDormant(DormantType dormantType)
	{
		if (dormantType != DormantType.DormantStand && dormantType == DormantType.DormantProne)
		{
			base.Animator.SetBool(DormantParamNameHash, value: true);
		}
	}

	public void StandUp()
	{
		base.Animator.SetBool(DormantParamNameHash, value: false);
		base.Animator.SetBool(CrawlParamNameHash, value: false);
	}

	public void StartCrawl()
	{
		base.Animator.SetBool(CrawlParamNameHash, value: true);
	}

	private float GetAlertness(Alertness alertness)
	{
		if (stunned)
		{
			return 3f;
		}
		if (eatingLure)
		{
			return 4f;
		}
		return alertness switch
		{
			Alertness.Idle => 0f,
			Alertness.Alert => 1f,
			Alertness.Aggressive => 2f,
			_ => 0f,
		};
	}

	public void SetEatingLure(bool eating)
	{
		ClearEndPosition();
		eatingLure = eating;
		if (eatingLure)
		{
			base.Animator.ResetTrigger(EnterEatingParamNameHash);
			base.Animator.SetTrigger(EnterEatingParamNameHash);
		}
		else
		{
			base.Animator.ResetTrigger(ExitEatingParamNameHash);
			base.Animator.SetTrigger(ExitEatingParamNameHash);
		}
	}

	public void Transform()
	{
		base.Animator.SetTrigger(TransformParamNameHash);
	}

	public void SetStunned(bool astunned)
	{
		stunned = astunned;
		if (stunned)
		{
			base.Animator.ResetTrigger(EnterStunParamNameHash);
			base.Animator.SetTrigger(EnterStunParamNameHash);
		}
		else
		{
			base.Animator.ResetTrigger(ExitStunParamNameHash);
			base.Animator.SetTrigger(ExitStunParamNameHash);
		}
	}

	public void SetStaggered(bool isTargetStaggered)
	{
		staggered = isTargetStaggered;
		if (staggered)
		{
			base.Animator.ResetTrigger(EnterStunParamNameHash);
			base.Animator.SetTrigger(EnterStunParamNameHash);
		}
		else
		{
			base.Animator.ResetTrigger(ExitStunParamNameHash);
			base.Animator.SetTrigger(ExitStunParamNameHash);
		}
	}

	public void SetABTestA2ed(bool active)
	{
		if (active)
		{
			base.Animator.ResetTrigger(EnterStunParamNameHash);
			base.Animator.SetTrigger(EnterStunParamNameHash);
		}
		else
		{
			base.Animator.ResetTrigger(ExitStunParamNameHash);
			base.Animator.SetTrigger(ExitStunParamNameHash);
		}
	}

	private void UpdateAlertness()
	{
		float num = base.Animator.GetFloat(AlertnessParamNameHash);
		float alertness = GetAlertness(Alertness);
		if (alertness < num)
		{
			num -= AlertnessInterpSpeed * Time.deltaTime;
			if (num < alertness)
			{
				num = alertness;
			}
		}
		else if (alertness > num)
		{
			num += AlertnessInterpSpeed * Time.deltaTime;
			if (num > alertness)
			{
				num = alertness;
			}
		}
		base.Animator.SetFloat(AlertnessParamNameHash, Mathf.Clamp(num, 0f, 4f));
	}

	protected override void Update()
	{
		base.Update();
		UpdateAlertness();
		if (IsIdle)
		{
			if (idleSpeed == 0f)
			{
				idleSpeed = UnityEngine.Random.Range(0.9f, 1.1f);
			}
			base.Animator.speed = idleSpeed;
		}
		else
		{
			base.Animator.speed = 1f;
		}
	}

	protected void Start()
	{
		Alertness = InitialAlertness;
		if (AnimationOverrides != null)
		{
			SetAnimator(AnimationOverrides);
		}
		base.Animator.SetFloat(IdleVariationParamNameHash, UnityEngine.Random.Range(0, 3));
	}

	private void OnTransformEffect()
	{
		this.OnTransformEffectHandler?.Invoke();
	}

	private void OnTransform()
	{
		this.OnTransformHandler?.Invoke();
	}
}
