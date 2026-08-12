using System.Collections;
using System.Collections.Generic;
using System.Threading;
using BaseModel;
using TWDModel;
using UnityEngine;

internal class FogOfWarVisualization : MonoBehaviour
{
	protected class ColliderInfo
	{
		public List<Vector2> hullEdgeVertices;

		public ColliderInfo(List<Vector2> hullEdgeVertices)
		{
			this.hullEdgeVertices = hullEdgeVertices;
		}
	}

	private class ThreadParameters
	{
		public List<Vector3> observerLocations;

		public List<CombatColliderView> colliders;

		public List<Vector2> clipBounds;

		public List<Vector2> uvs = new List<Vector2>();

		public List<Vector3> vertices = new List<Vector3>();

		public List<int> triangles = new List<int>();

		public List<Color32> colors = new List<Color32>();

		public float EdgeOffset;

		public float EdgeWidth;
	}

	[SerializeField]
	private float mEdgeWidth = 0.5f;

	[SerializeField]
	private float mEdgeOffset;

	[SerializeField]
	[Tooltip("This object's Renderer bounds are used to clip the fog of war")]
	private GameObject CustomBoundsObject;

	private List<CombatColliderView> mColliders = new List<CombatColliderView>();

	private List<Vector2> mClipBounds = new List<Vector2>();

	private int lastObserverHash;

	private bool currentlyUpdating;

	private bool warningPrinted;

	private float lastUpdateTime = -1f;

	public float UpdateInterval
	{
		get
		{
			if (!PlatformInfo.HasFlag(PlatformFlag.LowMemory) && !PlatformInfo.HasFlag(PlatformFlag.SlowCPU))
			{
				return 0f;
			}
			return 1f;
		}
	}

	Shader SavedShader;
	public void Initialize()
	{
		base.transform.position = new Vector3(0f, base.transform.position.y, 0f);
		UpdateColliders();
		UpdateBounds();
		GetComponent<MeshFilter>().sharedMesh = new Mesh();
		warningPrinted = false;
		GameManager.Instance.OnLoadCompleted += OnLoadCompleted;

		if (OfflineManager.IsUseMatFix)
		{
			var mat = GetComponent<Renderer>().material;
			SavedShader = mat.shader;
			mat.shader = Shader.Find("UI/Unlit/Transparent");

			//mat.shader = Shader.Find("ParticleEffect_Shader/(Shader) Normal Additive");
		}
	}

	private void OnDestroy()
	{
		var mat = GetComponent<Renderer>().material;
		if (SavedShader != null) mat.shader = SavedShader;
	}

	private void OnLoadCompleted()
	{
		UpdateColliders();
	}

	private void Stop()
	{
		StopAllCoroutines();
		GameManager.Instance.OnLoadCompleted -= OnLoadCompleted;
	}

	private List<Vector3> GetObserverLocations()
	{
		List<Vector3> list = new List<Vector3>(4);
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (combat == null)
		{
			return list;
		}
		List<ActorModel> factionActors = combat.GetFactionActors(Faction.Survivor);
		List<ActorModel> list2 = ((combat.Lures.Models.Count > 0) ? combat.Lures.Models.FindAll((ActorModel t) => t.IsFlare) : null);
		for (int num = 0; num < factionActors.Count; num++)
		{
			ActorModel actorModel = factionActors[num];
			if (!actorModel.IsDead)
			{
				ActorView actorView = GameManager.Instance.GetViewForModel(actorModel) as ActorView;
				if (actorView != null)
				{
					list.Add(actorView.transform.position);
				}
			}
		}
		for (int num2 = 0; num2 < (list2?.Count ?? 0); num2++)
		{
			ActorView actorView2 = GameManager.Instance.GetViewForModel(list2[num2]) as ActorView;
			if (actorView2 != null)
			{
				list.Add(actorView2.transform.position);
			}
		}
		return list;
	}

	protected int CalculateObserverHash()
	{
		int num = 0;
		List<Vector3> observerLocations = GetObserverLocations();
		for (int i = 0; i < observerLocations.Count; i++)
		{
			num ^= observerLocations[i].GetHashCode();
		}
		return num;
	}

	protected void UpdateBounds()
	{
		Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
		if (CustomBoundsObject != null)
		{
			Renderer component = CustomBoundsObject.GetComponent<Renderer>();
			Collider component2 = CustomBoundsObject.GetComponent<Collider>();
			if (component != null)
			{
				bounds = component.bounds;
			}
			else if (component2 != null)
			{
				bounds = component2.bounds;
			}
			else
			{
				Debug.LogWarning("Fog of War Custom bounds object has no collider or renderer!");
			}
			mClipBounds = new List<Vector2>
			{
				new Vector2(bounds.min.x, bounds.min.z),
				new Vector2(bounds.max.x, bounds.min.z),
				new Vector2(bounds.max.x, bounds.max.z),
				new Vector2(bounds.min.x, bounds.max.z)
			};
		}
		else
		{
			Renderer[] array = Object.FindObjectsOfType<Renderer>();
			for (int i = 0; i < array.Length; i++)
			{
				bounds.Encapsulate(array[i].bounds);
			}
			mClipBounds = new List<Vector2>
			{
				new Vector2(bounds.min.x, bounds.min.z),
				new Vector2(bounds.max.x, bounds.min.z),
				new Vector2(bounds.max.x, bounds.max.z),
				new Vector2(bounds.min.x, bounds.max.z)
			};
		}
	}

	protected void UpdateColliders()
	{
		if (CombatView.Instance == null || CombatView.Instance.Model == null)
		{
			return;
		}
		CombatColliderView[] array = Object.FindObjectsOfType<CombatColliderView>();
		mColliders.Clear();
		foreach (CombatColliderView combatColliderView in array)
		{
			if (combatColliderView.Model == null)
			{
				if (warningPrinted)
				{
				}
				continue;
			}
			combatColliderView.Model.Changed -= OnColliderChanged;
			if (combatColliderView.Model.BlockVision && combatColliderView.Model.IsEnabled)
			{
				if (combatColliderView.Model.IsDynamic)
				{
					combatColliderView.Model.Changed += OnColliderChanged;
				}
				mColliders.Add(combatColliderView);
			}
		}
	}

	private void Update()
	{
		if (!GameManager.IsInitialized)
		{
			return;
		}
		float num = Time.time - lastUpdateTime;
		if (!currentlyUpdating && num >= UpdateInterval)
		{
			int num2 = CalculateObserverHash();
			if (num2 != lastObserverHash)
			{
				StartCoroutine(UpdateGeometry());
				lastObserverHash = num2;
			}
		}
	}

	protected IEnumerator UpdateGeometry()
	{
		List<Vector3> observerLocations = GetObserverLocations();
		if (observerLocations.Count != 0)
		{
			currentlyUpdating = true;
			ThreadParameters p = new ThreadParameters
			{
				observerLocations = observerLocations,
				colliders = mColliders,
				clipBounds = mClipBounds,
				EdgeOffset = mEdgeOffset,
				EdgeWidth = mEdgeWidth,
				uvs = new List<Vector2>(),
				vertices = new List<Vector3>(),
				triangles = new List<int>(),
				colors = new List<Color32>()
			};
			Thread t = new Thread(thread);
			t.Priority = System.Threading.ThreadPriority.BelowNormal;
			t.Start(p);
			while (t.IsAlive)
			{
				yield return null;
			}
			Mesh mesh = GetComponent<MeshFilter>().mesh;
			mesh.Clear();
			mesh.vertices = p.vertices.ToArray();
			mesh.triangles = p.triangles.ToArray();
			mesh.uv = p.uvs.ToArray();
			mesh.colors32 = p.colors.ToArray();
			currentlyUpdating = false;
			lastUpdateTime = Time.time;
		}
	}

	private static void thread(object po)
	{
		ThreadParameters threadParameters = (ThreadParameters)po;
		List<List<List<Vector2>>> list = new List<List<List<Vector2>>>();
		for (int i = 0; i < threadParameters.observerLocations.Count; i++)
		{
			Vector2 vector = new Vector2(threadParameters.observerLocations[i].x, threadParameters.observerLocations[i].z);
			List<List<Vector2>> list2 = new List<List<Vector2>>();
			for (int j = 0; j < threadParameters.colliders.Count; j++)
			{
				List<Vector2> hullVertices = threadParameters.colliders[j].HullVertices;
				List<Vector2> list3 = new List<Vector2>();
				List<bool> list4 = new List<bool>();
				List<Vector2> list5 = new List<Vector2>();
				int num = -1;
				for (int k = 0; k < hullVertices.Count; k++)
				{
					Vector2 vector2 = hullVertices[k];
					Vector2 vector3 = hullVertices[(k != hullVertices.Count - 1) ? (k + 1) : 0];
					Vector2 lhs = new Vector2(vector3.y - vector2.y, vector2.x - vector3.x);
					Vector2 rhs = vector - vector2;
					bool flag = Vector2.Dot(lhs, rhs) > 0f;
					if (num == -1 && flag)
					{
						num = k;
					}
					list4.Add(flag);
				}
				if (num != -1)
				{
					int num2 = num;
					while (list4[num2] && list3.Count < hullVertices.Count)
					{
						list3.Insert(0, hullVertices[num2]);
						num2 = ((num2 == 0) ? (hullVertices.Count - 1) : (num2 - 1));
					}
					num2 = num;
					while (list4[num2] && list3.Count < hullVertices.Count)
					{
						num2 = ((num2 != hullVertices.Count - 1) ? (num2 + 1) : 0);
						list3.Add(hullVertices[num2]);
					}
					for (int l = 0; l < list3.Count; l++)
					{
						list5.Add(list3[l]);
					}
					for (int num3 = list3.Count - 1; num3 >= 0; num3--)
					{
						Vector2 normalized = (list3[num3] - vector).normalized;
						int num4 = 100;
						list5.Add(list3[num3] + normalized * num4);
					}
					list2.Add(list5);
				}
			}
			list.Add(list2);
		}
		List<List<Vector2>> list6 = new List<List<Vector2>>();
		PolyClipping.IntersectAndClip(list, threadParameters.clipBounds, list6);
		Color32 item = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
		List<int> list7 = new List<int>();
		List<Vector2> list8 = new List<Vector2>();
		for (int m = 0; m < list6.Count; m++)
		{
			List<Vector2> list9 = list6[m];
			PolyClean(list9, 0.1f);
			list7.Clear();
			MeshGenerator.Triangulate(list9, list7);
			list8.Clear();
			Vector2 vector4 = ((list9.Count > 0) ? list9[list9.Count - 1] : new Vector2(0f, 0f));
			Vector2 vector5 = ((list9.Count > 0) ? GetNormal(list9[0] - vector4).normalized : new Vector2(0f, 1f));
			for (int n = 0; n < list9.Count; n++)
			{
				Vector2 vector6 = list9[n];
				int index = ((n != list9.Count - 1) ? (n + 1) : 0);
				Vector2 normalized2 = GetNormal(list9[index] - vector6).normalized;
				Vector2 normalized3 = (normalized2 + vector5).normalized;
				float a = 1f / Mathf.Abs(Vector2.Dot(normalized2, normalized3));
				a = Mathf.Min(a, 4f);
				list9[n] += normalized3 * threadParameters.EdgeOffset;
				list8.Add(list9[n] + normalized3 * threadParameters.EdgeWidth * a);
				vector4 = vector6;
				vector5 = normalized2;
			}
			int count = threadParameters.vertices.Count;
			for (int num5 = 0; num5 < list9.Count; num5++)
			{
				threadParameters.vertices.Add(ToVector3(list9[num5]));
				threadParameters.uvs.Add(list9[num5]);
				threadParameters.colors.Add(Color.white);
			}
			int count2 = threadParameters.vertices.Count;
			for (int num6 = 0; num6 < list9.Count; num6++)
			{
				threadParameters.vertices.Add(ToVector3(list8[num6]));
				threadParameters.uvs.Add(list8[num6]);
				threadParameters.colors.Add(item);
			}
			for (int num7 = 0; num7 < list7.Count; num7++)
			{
				threadParameters.triangles.Add(list7[num7] + count);
			}
			int num8 = list9.Count - 1;
			for (int num9 = 0; num9 < list9.Count; num9++)
			{
				threadParameters.triangles.Add(num8 + count);
				threadParameters.triangles.Add(num9 + count);
				threadParameters.triangles.Add(num8 + count2);
				threadParameters.triangles.Add(num8 + count2);
				threadParameters.triangles.Add(num9 + count);
				threadParameters.triangles.Add(num9 + count2);
				num8 = num9;
			}
		}
	}

	private static void PolyClean(List<Vector2> polygon, float threshold)
	{
		for (int num = polygon.Count - 1; num >= 0; num--)
		{
			Vector2 vector = polygon[num];
			int index = ((num == 0) ? (polygon.Count - 1) : (num - 1));
			Vector2 vector2 = polygon[index];
			if ((vector - vector2).magnitude < threshold)
			{
				polygon.RemoveAt(num);
			}
		}
	}

	private static Vector2 GetNormal(Vector2 p)
	{
		return new Vector2(p.y, 0f - p.x);
	}

	private void OnColliderChanged(ModelObject model, string changed, object args)
	{
		if (model is CombatColliderModel combatColliderModel)
		{
			if (changed == "IsEnabled" && combatColliderModel.BlockVision)
			{
				lastObserverHash = -1;
			}
			UpdateColliders();
		}
	}

	private static Vector3 ToVector3(Vector2 a)
	{
		return new Vector3(a.x, 0f, a.y);
	}

	public void OnValidate()
	{
		base.gameObject.isStatic = false;
	}
}
