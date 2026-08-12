using UnityEngine;

public class CombatBackUpCover : HUDElement
{
	[SerializeField]
	private float destroyTime = 2f;

	public override void Open()
	{
		base.Open();
		Invoke("Close", destroyTime);
	}

	private void OnDisable()
	{
	}

	public override void Close()
	{
		base.Close();
		Helpers.BackupEndUIEvent();
	}

	public override void Update()
	{
		base.Update();
	}
}
