using UnityEngine;

[RequireComponent(typeof(ParticleSystemRenderer))]
public class EffectUIRenderQueue : MonoBehaviour
{
	[SerializeField]
	private int queueOffset = -1;

	[SerializeField]
	private int sortingOrder;

	[SerializeField]
	private bool findParentPanel = true;

	[SerializeField]
	private bool showDebugInfo;

	private ParticleSystemRenderer[] particleRenderers;

	private UIPanel parentPanel;

	private UIWidget referenceWidget;

	private Material[] materials;

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		particleRenderers = GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true);
		if (particleRenderers == null || particleRenderers.Length == 0)
		{
			Debug.LogError("EffectUIRenderQueue: ParticleSystemRenderer not found!");
			return;
		}
		if (findParentPanel)
		{
			parentPanel = GetComponentInParent<UIPanel>();
			if (parentPanel == null)
			{
				Transform parent = base.transform.parent;
				while (parent != null)
				{
					parentPanel = parent.GetComponent<UIPanel>();
					if (parentPanel != null)
					{
						break;
					}
					parent = parent.parent;
				}
			}
			if (parentPanel != null)
			{
				referenceWidget = parentPanel.GetComponent<UIWidget>();
				if (referenceWidget == null)
				{
					referenceWidget = parentPanel.GetComponentInChildren<UIWidget>();
				}
			}
		}
		UpdateRenderQueue();
	}

	private void UpdateRenderQueue()
	{
		if (particleRenderers == null || particleRenderers.Length == 0)
		{
			return;
		}
		materials = particleRenderers[0].materials;
		if (materials == null || materials.Length == 0)
		{
			Debug.LogWarning("EffectUIRenderQueue: No materials found on ParticleSystemRenderer!");
			return;
		}
		int num = ((referenceWidget != null && referenceWidget.drawCall != null) ? (referenceWidget.drawCall.finalRenderQueue + queueOffset) : ((!(parentPanel != null)) ? (3000 + queueOffset) : (3000 + parentPanel.depth + queueOffset)));
		ParticleSystemRenderer[] array = particleRenderers;
		foreach (ParticleSystemRenderer particleSystemRenderer in array)
		{
			if (particleSystemRenderer == null)
			{
				continue;
			}
			Material[] array2 = particleSystemRenderer.materials;
			if (array2 == null)
			{
				continue;
			}
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j] != null)
				{
					array2[j].renderQueue = num;
				}
			}
			particleSystemRenderer.sortingOrder = sortingOrder;
		}
		if (showDebugInfo)
		{
			string.Format("EffectUIRenderQueue Debug:\n  Parent Panel Depth: {0}\n  Reference Widget: {1}\n  Target Render Queue: {2}\n  Queue Offset: {3}\n  Sorting Order: {4}\n  Renderers Count: {5}", (parentPanel != null) ? parentPanel.depth.ToString() : "None", (referenceWidget != null) ? referenceWidget.name : "None", num, queueOffset, sortingOrder, (particleRenderers != null) ? particleRenderers.Length.ToString() : "0");
		}
	}

	private void LateUpdate()
	{
		if (referenceWidget != null && referenceWidget.drawCall != null && materials != null && materials.Length != 0)
		{
			int num = referenceWidget.drawCall.finalRenderQueue + queueOffset;
			if (materials[0].renderQueue != num)
			{
				UpdateRenderQueue();
			}
		}
		else if (parentPanel != null && referenceWidget == null)
		{
			referenceWidget = parentPanel.GetComponent<UIWidget>();
			if (referenceWidget == null)
			{
				referenceWidget = parentPanel.GetComponentInChildren<UIWidget>();
			}
			if (referenceWidget != null && referenceWidget.drawCall != null)
			{
				UpdateRenderQueue();
			}
		}
	}

	private void OnDestroy()
	{
		if (materials == null)
		{
			return;
		}
		for (int i = 0; i < materials.Length; i++)
		{
			if (materials[i] != null)
			{
				Object.DestroyImmediate(materials[i]);
			}
		}
	}
}
