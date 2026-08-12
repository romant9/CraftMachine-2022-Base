using UnityEngine;

[RequireComponent(typeof(UIDragScrollView))]
public class NestedUIDragScrollView : MonoBehaviour
{
	public UIDragScrollView target;

	private UIDragScrollView uiDragScrollViewRef;

	private UIScrollView.Movement currentDragDirection = UIScrollView.Movement.Custom;

	private UIDragScrollView uiDragScrollView
	{
		get
		{
			if (uiDragScrollViewRef == null)
			{
				uiDragScrollViewRef = base.gameObject.GetComponent<UIDragScrollView>();
			}
			return uiDragScrollViewRef;
		}
	}

	public virtual void OnDisable()
	{
		Clear();
	}

	public virtual void OnPress(bool pressed)
	{
		if (target != null)
		{
			target.SendMessage("OnPress", pressed, SendMessageOptions.DontRequireReceiver);
			if (pressed)
			{
				currentDragDirection = UIScrollView.Movement.Custom;
			}
		}
	}

	public virtual void OnDrag(Vector2 delta)
	{
		if (!(uiDragScrollView != null) || !(uiDragScrollView.scrollView != null) || !(target != null))
		{
			return;
		}
		if (currentDragDirection == UIScrollView.Movement.Custom)
		{
			if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
			{
				currentDragDirection = UIScrollView.Movement.Horizontal;
			}
			else
			{
				currentDragDirection = UIScrollView.Movement.Vertical;
			}
		}
		if (currentDragDirection != uiDragScrollView.scrollView.movement)
		{
			target.SendMessage("OnDrag", delta, SendMessageOptions.DontRequireReceiver);
			if (uiDragScrollView != null)
			{
				uiDragScrollView.scrollView.Press(pressed: false);
			}
		}
		else
		{
			OnPress(pressed: false);
		}
	}

	private void Clear()
	{
		uiDragScrollViewRef = null;
		currentDragDirection = UIScrollView.Movement.Custom;
	}
}
