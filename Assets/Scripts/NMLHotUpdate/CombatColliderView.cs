using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class CombatColliderView : ModelView<CombatColliderModel>
{
	public static Vector3 WaistHeight = new Vector3(0f, 0.75f, 0f);

	[HideInInspector]
	public List<Vector2> HullVertices;

	public Bounds ColliderBounds
	{
		get
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if (component != null)
			{
				return new Bounds(component.center, component.size);
			}
			MeshCollider component2 = GetComponent<MeshCollider>();
			if (component2 != null)
			{
				return component2.sharedMesh.bounds;
			}
			return default(Bounds);
		}
	}

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
	}

	private bool CanBlockMovement()
	{
		return base.Model.BlockMovement;
	}

	private bool CanBlockVision()
	{
		return base.Model.BlockVision;
	}

	public bool BlocksMovement(Vector3 sourcePosition, Vector3 targetPosition)
	{
		if (!CanBlockMovement())
		{
			return false;
		}
		return LineIntersectsOBB(sourcePosition, targetPosition, GetComponent<Collider>());
	}

	public bool BlocksVision(Vector3 sourcePosition, Vector3 targetPosition)
	{
		if (!CanBlockVision())
		{
			return false;
		}
		return LineIntersectsOBB(sourcePosition, targetPosition, GetComponent<Collider>());
	}

	public static bool LineIntersectsOBB(Vector3 sourcePosition, Vector3 targetPosition, Collider collider)
	{
		Vector3 origin = sourcePosition + WaistHeight;
		Vector3 vector = targetPosition - sourcePosition;
		if (vector.sqrMagnitude == 0f)
		{
			return false;
		}
		Ray ray = new Ray(origin, vector.normalized);
		RaycastHit hitInfo;
		return collider.Raycast(ray, out hitInfo, vector.magnitude);
	}

	public bool Contains(Vector3 position)
	{
		Bounds colliderBounds = ColliderBounds;
		Vector3 point = base.transform.worldToLocalMatrix.MultiplyPoint(position);
		return colliderBounds.Contains(point);
	}
}
