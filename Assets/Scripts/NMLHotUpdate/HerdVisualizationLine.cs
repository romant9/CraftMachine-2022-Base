using System.Collections.Generic;
using Client.Utils;
using UnityEngine;

public class HerdVisualizationLine : MonoBehaviour
{
	public GameObject DrawnPath;

	private ActorView herdedActor;

	private ActorView instigator;

	private Mesh drawnPathMesh;

	private Vector3 A;

	private Vector3 B;

	private Vector3 C;

	private Vector3 D;

	private readonly float actorHeightOffset = 2f;

	private readonly float bezierControlPointOffset = 3.5f;

	public void SetActorViewDependencies(ActorView target, ActorView enemy)
	{
		if (instigator != enemy)
		{
			if (instigator != null)
			{
				instigator.CharacterAnimationController.OnMove -= OnMove;
			}
			instigator = enemy;
			enemy.CharacterAnimationController.OnMove += OnMove;
		}
		if (herdedActor == null)
		{
			herdedActor = target;
			herdedActor.CharacterAnimationController.OnMove += OnMove;
		}
		CheckSetPath();
	}

	private void OnMove(bool moving)
	{
		if (moving)
		{
			drawnPathMesh?.Clear();
		}
		else
		{
			CheckSetPath();
		}
	}

	private void CheckSetPath()
	{
		MoveVisualizationTask mostRecentlyAddedTask = VisualizationQueue.Instance.GetMostRecentlyAddedTask<MoveVisualizationTask>(instigator.Model);
		if (mostRecentlyAddedTask == null || mostRecentlyAddedTask.IsActive)
		{
			SetPath();
		}
	}

	private void SetPath()
	{
		if (drawnPathMesh == null)
		{
			drawnPathMesh = DrawnPath.GetComponent<MeshFilter>().mesh;
		}
		if (drawnPathMesh != null)
		{
			DrawLine(GridView.Instance.GetPosition(herdedActor.Model.GridCoordinate).ToVector3(), GridView.Instance.GetPosition(instigator.Model.GridCoordinate).ToVector3());
		}
	}

	private void DrawLine(Vector3 startPoint, Vector3 endPoint)
	{
		Vector3 vector = base.transform.worldToLocalMatrix.MultiplyPoint(startPoint);
		Vector3 vector2 = base.transform.worldToLocalMatrix.MultiplyPoint(endPoint);
		A = vector + Vector3.up * actorHeightOffset;
		B = vector + Vector3.up * bezierControlPointOffset;
		C = vector2 + Vector3.up * bezierControlPointOffset;
		D = vector2 + Vector3.up * actorHeightOffset;
		drawnPathMesh.Clear();
		PolylinePath polylinePath = new PolylinePath();
		Vector3 inStart = A;
		float num = 0.02f;
		int num2 = Mathf.FloorToInt(1f / num);
		for (int i = 1; i <= num2; i++)
		{
			float t = (float)i * num;
			Vector3 vector3 = DeCasteljausAlgorithm(t);
			polylinePath.AddSegment(new LineSegment(inStart, vector3, Vector3.up, Color.white));
			inStart = vector3;
		}
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		List<Color> list3 = new List<Color>();
		polylinePath.GetPathPoints(list, list2, list3, 8);
		List<Color> list4 = new List<Color>();
		List<Vector3> list5 = new List<Vector3>();
		List<Vector2> list6 = new List<Vector2>();
		List<int> list7 = new List<int>();
		float thickness = 0.25f;
		float textureScale = 0.5f;
		MeshGenerator.CreateThickline(list, list2, list3, thickness, list5, list6, list7, list4, textureScale);
		drawnPathMesh.vertices = list5.ToArray();
		drawnPathMesh.normals = null;
		drawnPathMesh.uv = list6.ToArray();
		drawnPathMesh.colors = list4.ToArray();
		list7.Reverse();
		drawnPathMesh.triangles = list7.ToArray();
	}

	private Vector3 DeCasteljausAlgorithm(float t)
	{
		float num = 1f - t;
		Vector3 vector = num * A + t * B;
		Vector3 vector2 = num * B + t * C;
		Vector3 vector3 = num * C + t * D;
		Vector3 vector4 = num * vector + t * vector2;
		Vector3 vector5 = num * vector2 + t * vector3;
		return num * vector4 + t * vector5;
	}

	public void Clear()
	{
		drawnPathMesh?.Clear();
		if (instigator != null)
		{
			instigator.CharacterAnimationController.OnMove -= OnMove;
			instigator = null;
		}
		if (herdedActor != null)
		{
			herdedActor.CharacterAnimationController.OnMove -= OnMove;
			herdedActor = null;
		}
	}

	private void OnDisable()
	{
		Clear();
	}
}
