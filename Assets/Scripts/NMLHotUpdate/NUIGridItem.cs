using UnityEngine;

public class NUIGridItem : MonoBehaviourExtended
{
	private Vector3 localSize = new Vector3(0f, 0f, 0f);

	private bool init;

	private static Vector3[] corners = new Vector3[4];

	public virtual void Start()
	{
		if (!init)
		{
			Init();
		}
	}

	public virtual bool Init()
	{
		init = true;
		return UpdateSize();
	}

	public virtual Vector3 GetLocalSize()
	{
		if (!init)
		{
			Init();
		}
		return localSize;
	}

	public virtual Bounds CreateLocalBounds()
	{
		if (!init)
		{
			Init();
		}
		return new Bounds(base.transform.localPosition, localSize);
	}

	public virtual void SetLocalPosition(Vector3 newPosition)
	{
		base.transform.localPosition = newPosition;
	}

	public Vector3[] GetLocalCorners()
	{
		float num = base.transform.localPosition.x - GetLocalSize().x * 0.5f;
		float num2 = base.transform.localPosition.y - GetLocalSize().y * 0.5f;
		float x = num + GetLocalSize().x;
		float y = num2 + GetLocalSize().y;
		corners[0] = new Vector3(num, num2);
		corners[1] = new Vector3(num, y);
		corners[2] = new Vector3(x, y);
		corners[3] = new Vector3(x, num2);
		return corners;
	}

	public virtual void AddedToList(UIPanel parentPanel)
	{
	}

	public virtual bool UpdateSize()
	{
		localSize = Vector3.zero;
		BoxCollider component = GetComponent<BoxCollider>();
		if (component != null)
		{
			localSize = component.size;
			return localSize != Vector3.zero;
		}
		UIWidget component2 = GetComponent<UIWidget>();
		if (component2 != null)
		{
			localSize = component2.localSize;
		}
		return localSize != Vector3.zero;
	}
}
