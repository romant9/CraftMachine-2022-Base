using UnityEngine;

public class CameraRunControlsHUD : HUDElement
{
	private enum State
	{
		Idle = 0,
		Playing = 1,
		Paused = 2
	}

	private State state;

	public void OnPlay()
	{
		if (!(Camera.main == null))
		{
			Animator component = Camera.main.gameObject.GetComponent<Animator>();
			switch (state)
			{
			case State.Idle:
				component.SetTrigger("Play");
				state = State.Playing;
				break;
			case State.Playing:
				component.speed = 0f;
				state = State.Paused;
				break;
			case State.Paused:
				component.speed = 1f;
				state = State.Playing;
				break;
			}
		}
	}

	public void OnRestart()
	{
		if (!(Camera.main == null))
		{
			Animator component = Camera.main.gameObject.GetComponent<Animator>();
			switch (state)
			{
			case State.Paused:
				component.speed = 1f;
				component.SetTrigger("Reset");
				state = State.Idle;
				break;
			case State.Playing:
				component.SetTrigger("Reset");
				state = State.Idle;
				break;
			}
		}
	}
}
