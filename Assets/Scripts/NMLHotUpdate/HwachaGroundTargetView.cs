using System;
using BaseModel;
using UnityEngine;

public class HwachaGroundTargetView : CombatSupportGroundTargetView
{
	[SerializeField]
	private HwachaArcherController hwachaArcherController;

	[SerializeField]
	private int arrowCount;

	[SerializeField]
	private float arrowSpeedVariance;

	[SerializeField]
	private float lifetime;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		hwachaArcherController.Initialize(arrowCount);
	}

	public override void Execute(Vector3 position)
	{
		base.transform.position = position + new Vector3(-10f, 10f, 0f);
		for (int i = 0; i < arrowCount; i++)
		{
			hwachaArcherController.ShootArrow(position, UnityEngine.Random.Range(0f - arrowSpeedVariance, arrowSpeedVariance));
		}
		GameManager.Instance.TimingManager.Timer(TimeSpan.FromSeconds(lifetime), delegate
		{
			hwachaArcherController.ClearArrows();
			base.gameObject.SetActive(value: false);
		});
	}
}
