using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GuildBattleMapButtonLines : MonoBehaviour
{
	public enum State
	{
		None = 0,
		SomeUnlocked = 1,
		AllUnlocked = 2,
		NoneUnlocked = 3
	}

	private State currentState;

	private Vector2 offset;

	private LineRenderer lineRendererInternal;

	public LineRenderer LineRenderer
	{
		get
		{
			if (lineRendererInternal == null)
			{
				lineRendererInternal = GetComponent<LineRenderer>();
			}
			return lineRendererInternal;
		}
	}

	public void Update()
	{
		if ((lineRendererInternal != null && currentState == State.AllUnlocked) || currentState == State.SomeUnlocked)
		{
			offset += new Vector2(1f, 1f) * Time.deltaTime;
			offset.y = 0f;
			lineRendererInternal.material.SetTextureOffset("_MainTex", offset);
		}
	}

	public void ChangeState(State newState, GuildBattleMapLineAssets assets)
	{
		if (LineRenderer != null && assets != null && assets.NotEmpty() && currentState != newState)
		{
			currentState = newState;
			if (currentState == State.AllUnlocked)
			{
				LineRenderer.material = assets.AllUnlockedLineMateria;
			}
			else if (currentState == State.SomeUnlocked)
			{
				LineRenderer.material = assets.SomeUnlockedMateria;
			}
			else if (currentState == State.NoneUnlocked)
			{
				LineRenderer.material = assets.NoneUnlockedLineMateria;
			}
		}
	}
}
