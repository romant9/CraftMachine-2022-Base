using UnityEngine;

public class FlipBookAutoPlay : MonoBehaviour
{
	[SerializeField]
	private int Rows;

	[SerializeField]
	private int Columns;

	[SerializeField]
	private float Speed;

	[SerializeField]
	private bool IsLooping;

	private UITexture flipBookTexture;

	private float CurrentFrame;

	private int speedParameterID;

	private float OverAllFrameCount => (float)(Rows * Columns) - 0.1f;

	private void Start()
	{
		speedParameterID = Shader.PropertyToID("_CurrentFrame");
		flipBookTexture = GetComponent<UITexture>();
		flipBookTexture.onRender = OnRenderFlipBook;
	}

	private void Update()
	{
		if (flipBookTexture == null)
		{
			return;
		}
		CurrentFrame += Speed * Time.deltaTime;
		if (CurrentFrame >= OverAllFrameCount)
		{
			if (!IsLooping)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				CurrentFrame = 0f;
			}
		}
	}

	private void OnRenderFlipBook(Material material)
	{
		if (CurrentFrame < OverAllFrameCount)
		{
			material.SetFloat(speedParameterID, CurrentFrame);
		}
	}
}
