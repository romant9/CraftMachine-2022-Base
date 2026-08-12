using System;
using System.Collections;
using UnityEngine;

public class ShadowBlobOrient : MonoBehaviour
{
	public enum shadowShapeType
	{
		Character = 0,
		Round = 1,
		Square = 2,
		Car = 3,
		Custom1 = 4,
		Custom2 = 5,
		Custom3 = 6,
		Custom4 = 7
	}

	public enum shadowMoveType
	{
		Rotate = 0,
		Translate = 1
	}

	public enum parentType
	{
		thisX = 0,
		thisY = 1,
		thisZ = 2,
		rootX = 3,
		rootY = 4,
		rootZ = 5
	}

	private GameObject lightObject;

	public Material shadowMaterial;

	public shadowShapeType shadowShape;

	public shadowMoveType shadowMove;

	private int shadowTextureIndex;

	private int shadowTextureTiles = 8;

	private Vector2 size;

	private Vector2 offset;

	public float shadowMoveAmount = 0.3f;

	public bool Static = true;

	public bool inheritRot = true;

	public parentType inheritFrom = parentType.thisY;

	public Vector3 shadowScale = new Vector3(1f, 1f, 1f);

	public Vector3 shadowOffset = new Vector3(0f, 0.02f, 0f);

	public Vector3 shadowRotOffset = new Vector3(0f, 0f, 0f);

	public bool inheritYpos;

	public float ShadowOpacity = 1f;

	public float FadeOutTime = 0.5f;

	private GameObject shadowPlane;

	private GameObject rotator;

	private Transform rootTransform;

	private Vector3 parentRot;

	private MeshRenderer shadowRenderer;

	private Transform thisTransform;

	private Transform shadowTransform;

	private Transform rotatorTransform;

	private float bboxYmin;

	private Mesh shadowMesh;

	private float shadowLevel;

	private bool fadingOut;

	public void CreateShadow()
	{
		if (rotator == null)
		{
			thisTransform = base.transform;
			GameObject gameObject = GameObject.Find("ShadowContainer");
			if (gameObject == null)
			{
				gameObject = new GameObject("ShadowContainer");
			}
			MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
			if (component != null)
			{
				Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
				bboxYmin = component.sharedMesh.bounds.min.y;
			}
			else
			{
				bboxYmin = thisTransform.position.y;
			}
			lightObject = GameObject.Find("LightMain");
			rotator = new GameObject(base.gameObject.name + "_Shadow_rotator_null");
			rotatorTransform = rotator.transform;
			shadowPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
			if (Application.isEditor && !Application.isPlaying)
			{
				UnityEngine.Object.DestroyImmediate(shadowPlane.GetComponent<MeshCollider>());
			}
			else
			{
				UnityEngine.Object.Destroy(shadowPlane.GetComponent<MeshCollider>());
			}
			shadowTransform = shadowPlane.transform;
			shadowPlane.name = "ShadowPlane";
			shadowTransform.parent = rotatorTransform;
			rotatorTransform.parent = gameObject.transform;
			rotator.SetLayerRecursively(base.gameObject.layer);
			shadowRenderer = shadowPlane.GetComponent<MeshRenderer>();
			shadowRenderer.material = shadowMaterial;
			shadowMesh = shadowPlane.GetComponent<MeshFilter>().mesh;
			shadowTextureIndex = (int)shadowShape;
			rootTransform = thisTransform.root;
			size = new Vector2(1f / (float)shadowTextureTiles, 1f);
			offset = new Vector2((float)shadowTextureIndex * size.x, 1f);
			GameObject gameObject2 = GameObject.Find("CombatSetup");
			if (gameObject2 != null)
			{
				Transform transform = gameObject2.transform;
				shadowLevel = transform.position.y;
			}
			if (inheritYpos)
			{
				shadowLevel = thisTransform.position.y;
			}
			UpdateUVs();
			UpdateAlpha();
			UpdateShadows();
			if (!Static)
			{
				rotatorTransform.position = new Vector3(0f, -1f, 0f);
			}
		}
	}

	private void DestroyShadow()
	{
		if (shadowPlane != null)
		{
			UnityEngine.Object.Destroy(shadowPlane);
			shadowPlane = null;
		}
		if (rotator != null)
		{
			UnityEngine.Object.Destroy(rotator);
			rotator = null;
		}
	}

	public void FadeAndDestroyShadow()
	{
		fadingOut = true;
		StartCoroutine(FadeOutShadow(FadeOutTime));
	}

	private IEnumerator FadeOutShadow(float fadeTime)
	{
		if (shadowMesh != null)
		{
			float elapsedTime = 0f;
			Color[] colors = new Color[shadowMesh.vertexCount];
			float startAlpha = shadowMesh.colors[0].a;
			float newAlpha = startAlpha;
			while (shadowMesh != null && newAlpha > 0f)
			{
				newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeTime);
				for (int i = 0; i < shadowMesh.vertexCount; i++)
				{
					colors[i] = new Color(1f, 1f, 1f, newAlpha);
				}
				shadowMesh.colors = colors;
				elapsedTime += Time.deltaTime;
				yield return null;
			}
		}
		DestroyShadow();
	}

	private void Start()
	{
		if (base.enabled)
		{
			CreateShadow();
		}
	}

	private void OnEnable()
	{
		CreateShadow();
	}

	private void OnDisable()
	{
		DestroyShadow();
	}

	private void Update()
	{
		if (!fadingOut)
		{
			if (rotator == null)
			{
				CreateShadow();
			}
			if (!Static)
			{
				UpdateShadows();
			}
		}
	}

	private void UpdateUVs()
	{
		Vector2[] array = new Vector2[shadowMesh.vertexCount];
		array[2] = new Vector2(offset.x, 1f);
		array[0] = new Vector2(offset.x, 0f);
		array[3] = new Vector2(offset.x + size.x, 1f);
		array[1] = new Vector2(offset.x + size.x, 0f);
		shadowMesh.uv = array;
	}

	private void UpdateAlpha()
	{
		float t = bboxYmin - shadowLevel;
		float num = Mathf.SmoothStep(1f, 0f, t);
		Color[] array = new Color[shadowMesh.vertexCount];
		for (int i = 0; i < shadowMesh.vertexCount; i++)
		{
			array[i] = new Color(1f, 1f, 1f, num * ShadowOpacity);
		}
		shadowMesh.colors = array;
		if (num <= 0.001f)
		{
			shadowRenderer.enabled = false;
		}
		else if (shadowRenderer != null)
		{
			shadowRenderer.enabled = true;
		}
	}

	private void UpdateShadows()
	{
		if (rotator == null)
		{
			return;
		}
		parentRot = new Vector3(0f, 0f, 0f);
		if (inheritRot)
		{
			switch (inheritFrom)
			{
			case parentType.thisX:
				parentRot = new Vector3(0f, thisTransform.localEulerAngles.x, 0f);
				break;
			case parentType.thisY:
				parentRot = new Vector3(0f, thisTransform.localEulerAngles.y, 0f);
				break;
			case parentType.thisZ:
				parentRot = new Vector3(0f, thisTransform.localEulerAngles.z, 0f);
				break;
			case parentType.rootX:
				parentRot = new Vector3(0f, rootTransform.localEulerAngles.x, 0f);
				break;
			case parentType.rootY:
				parentRot = new Vector3(0f, rootTransform.localEulerAngles.y, 0f);
				break;
			case parentType.rootZ:
				parentRot = new Vector3(0f, rootTransform.localEulerAngles.z, 0f);
				break;
			}
		}
		float num = 0f;
		float num2 = 90f;
		if (lightObject != null)
		{
			num = lightObject.transform.eulerAngles.y;
			num2 = lightObject.transform.eulerAngles.x;
		}
		switch (shadowMove)
		{
		case shadowMoveType.Rotate:
			rotatorTransform.position = new Vector3(thisTransform.position.x, shadowLevel, thisTransform.position.z);
			rotatorTransform.eulerAngles = new Vector3(0f, num, 0f);
			shadowTransform.localEulerAngles = new Vector3(90f, 0f, 0f);
			shadowTransform.localPosition = shadowOffset;
			shadowTransform.localScale = new Vector3(shadowScale.x * 1f, 1.5f * shadowScale.z * Mathf.Cos(MathF.PI / 180f * num2 + 0f), shadowScale.y);
			break;
		case shadowMoveType.Translate:
		{
			Vector3 vector = new Vector3(shadowMoveAmount * Mathf.Sin(MathF.PI / 180f * num) * Mathf.Cos(MathF.PI / 180f * num2), 0f, shadowMoveAmount * Mathf.Cos(MathF.PI / 180f * num) * Mathf.Cos(MathF.PI / 180f * num2));
			rotatorTransform.position = new Vector3(thisTransform.position.x, shadowLevel, thisTransform.position.z) + vector;
			shadowTransform.localPosition = shadowOffset;
			rotatorTransform.eulerAngles = shadowRotOffset + parentRot;
			shadowTransform.localEulerAngles = new Vector3(90f, 0f, 0f);
			shadowTransform.localScale = shadowScale;
			break;
		}
		}
	}
}
