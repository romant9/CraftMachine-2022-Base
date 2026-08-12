using UnityEngine;

public class TooltipBox : TooltipBase
{
	[Header("Optional")]
	[Tooltip("Used offset the content to overflow over the pointer")]
	[SerializeField]
	protected Vector2 PointerSizeOffset;

	[Tooltip("If the tip of the pointer is positioned offset from the center. This can be used to insert that offset. If not set correct it could break the AUTO position.")]
	[SerializeField]
	protected Vector2 PointerPositionOffset;

	[Header("Mandatory")]
	[Tooltip("Pointer that will point at the target")]
	[SerializeField]
	private UISprite Pointer;

	[Tooltip("Content that will be positioned to the pointer")]
	[SerializeField]
	private Transform ContentParent;

	[SerializeField]
	[Tooltip("What will define the size of the content. Usually a background sprite.")]
	protected UISprite ContentSize;

	public override void Show()
	{
		base.Show();
		Position();
	}

	public override void Hide()
	{
		base.Hide();
	}

	public void Position()
	{
		if (!(Pointer != null) || !(ContentParent != null) || !(ContentSize != null))
		{
			return;
		}
		ContentSize.ResetAndUpdateAnchors();
		Vector2 vector = Helpers.CalculateNguiScreenSize(base.gameObject);
		Vector2 vector2 = Pointer.localSize + PointerSizeOffset;
		Vector2 localSize = ContentSize.localSize;
		float num = 0f;
		float num2 = 3f;
		TooltipTarget.Orientation orientation = TooltipTarget.Orientation.AUTO;
		if (base.TooltipTarget != null)
		{
			orientation = base.TooltipTarget.OrientationOverride;
			num = base.TooltipTarget.OffsetFromTarget;
		}
		Vector3 one = Vector3.one;
		Vector3 localPosition = Vector3.one;
		if (orientation == TooltipTarget.Orientation.AUTO)
		{
			orientation = ((!((double)(base.gameObject.transform.localPosition.x + localSize.x + vector2.x * 0.5f + PointerPositionOffset.x + num) > (double)vector.x * 0.5)) ? TooltipTarget.Orientation.RIGHT : TooltipTarget.Orientation.LEFT);
		}
		if (orientation == TooltipTarget.Orientation.AUTOVERTICAL)
		{
			orientation = ((!((double)(base.gameObject.transform.localPosition.y + localSize.y + vector2.y * 0.5f + PointerPositionOffset.y + num) > (double)vector.y * 0.5)) ? TooltipTarget.Orientation.UP : TooltipTarget.Orientation.DOWN);
		}
		switch (orientation)
		{
		case TooltipTarget.Orientation.UP:
			Pointer.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			one.x = 0f - localSize.x * 0.5f;
			one.y = vector2.y + localSize.y + num;
			localPosition.y = num + num2;
			break;
		case TooltipTarget.Orientation.DOWN:
			Pointer.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
			one.x = 0f - localSize.x * 0.5f;
			one.y = 0f - (vector2.y + num);
			localPosition.y = 0f - num - num2;
			break;
		case TooltipTarget.Orientation.LEFT:
			Pointer.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
			one.x = 0f - (vector2.y + localSize.x + num);
			one.y = vector2.x * 0.5f;
			localPosition.x = 0f - num - num2;
			break;
		case TooltipTarget.Orientation.RIGHT:
			Pointer.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
			one.x = vector2.y + num;
			one.y = vector2.x * 0.5f;
			localPosition.x = num + num2;
			break;
		case TooltipTarget.Orientation.CENTER:
			Pointer.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			one.x = 0f - localSize.x * 0.5f;
			one.y = localSize.y * 0.5f;
			localPosition = Vector3.zero;
			break;
		}
		Pointer.transform.localPosition = localPosition;
		if ((orientation == TooltipTarget.Orientation.LEFT || orientation == TooltipTarget.Orientation.RIGHT) && base.gameObject.transform.localPosition.y - localSize.y + vector2.y * 0.5f + PointerPositionOffset.y < 0f - vector.y * 0.5f)
		{
			float num3 = 0f - vector.y * 0.5f - base.gameObject.transform.localPosition.y;
			one.y = num3 + localSize.y;
		}
		if (orientation == TooltipTarget.Orientation.UP || orientation == TooltipTarget.Orientation.DOWN)
		{
			if (base.gameObject.transform.localPosition.x + localSize.x * 0.5f + vector2.x * 0.5f + PointerPositionOffset.x > vector.x * 0.5f)
			{
				float num4 = vector.x * 0.5f - base.gameObject.transform.localPosition.x;
				one.x = num4 - localSize.x;
			}
			else if (base.gameObject.transform.localPosition.x - localSize.x * 0.5f - vector2.x * 0.5f - PointerPositionOffset.x < 0f - vector.x * 0.5f)
			{
				float x = 0f - vector.x * 0.5f - base.gameObject.transform.localPosition.x;
				one.x = x;
			}
		}
		ContentParent.localPosition = one;
	}
}
