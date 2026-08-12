using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AfterImage : MonoBehaviour
{
	private float effectTimer;

	[SerializeField]
	private float Frequency = 0.2f;

	[SerializeField]
	private List<MonoBehaviour> ImageEffects;

	[SerializeField]
	private UnityEvent actions;

	private static Type[] RendererTypes => new Type[3]
	{
		typeof(MeshRenderer),
		typeof(MeshFilter),
		typeof(SkinnedMeshRenderer)
	};

	private void Update()
	{
		UpdateAfterImage();
	}

	private void CopyState(GameObject parent, GameObject target)
	{
		GameObject newState = new GameObject();
		newState.name = target.name + " (afterimage)";
		if (parent != null)
		{
			newState.transform.SetParent(parent.transform);
		}
		Helpers.CopyTransform(newState, target);
		Helpers.IterateByRenderType(target, RendererTypes, delegate(Type type, Component component)
		{
			if (type == typeof(SkinnedMeshRenderer))
			{
				SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)component;
				Mesh mesh = new Mesh();
				skinnedMeshRenderer.BakeMesh(mesh);
				newState.AddComponent<MeshFilter>().mesh = mesh;
				Helpers.CopyShader(newState.AddComponent<MeshRenderer>(), skinnedMeshRenderer);
			}
			if (type == typeof(MeshFilter))
			{
				MeshFilter to = (MeshFilter)component;
				Helpers.CopyComponent(newState, to);
			}
			if (type == typeof(MeshRenderer))
			{
				MeshRenderer to2 = (MeshRenderer)component;
				Helpers.CopyShader(Helpers.CopyComponent(newState, to2), to2);
			}
		});
		foreach (MonoBehaviour imageEffect in ImageEffects)
		{
			if (!(imageEffect == null))
			{
				Helpers.CopyComponent(newState, imageEffect);
			}
		}
		actions?.Invoke();
		Transform transform = target.transform;
		for (int num = 0; num < transform.childCount; num++)
		{
			Transform child = transform.GetChild(num);
			CopyState(newState, child.gameObject);
		}
	}

	private void UpdateAfterImage()
	{
		effectTimer += Time.deltaTime;
		if (!(effectTimer < Frequency))
		{
			Transform parent = base.transform.parent;
			CopyState(parent ? parent.gameObject : null, base.gameObject);
			effectTimer = 0f;
		}
	}
}
