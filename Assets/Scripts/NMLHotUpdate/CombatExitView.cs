using BaseModel;
using Client.Constants;
using TWDModel;
using UnityEngine;

public class CombatExitView : ModelView<CombatExitModel>
{
	private ExitGridAreaVisualization visualizer;

	private Vector3 displayPosition;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		CombatExitModel combatExitModel = (CombatExitModel)model;
		visualizer = GridView.Instance.GetComponentInChildren<ExitGridAreaVisualization>();
		visualizer.Initialize(combat.Grid, combatExitModel.GridCoordinates);
		CheckEnabled();
		base.Model.Changed += OnModelChange;
		ExitLocationConfig[] componentsInChildren = GetComponentsInChildren<ExitLocationConfig>(includeInactive: true);
		if (componentsInChildren.Length != 0)
		{
			displayPosition = componentsInChildren[0].transform.position;
		}
	}

	public Vector3 GetDisplayPosition()
	{
		return displayPosition;
	}

	private void OnModelChange(ModelObject model, string changed, object args)
	{
		if (changed == "enableStateChanged")
		{
			CheckEnabled();
		}
	}

	private void EnabledVisualization()
	{
		if (visualizer != null)
		{
			MeshRenderer component = visualizer.ShapeFill.GetComponent<MeshRenderer>();
			MeshRenderer component2 = visualizer.ShapeOutline.GetComponent<MeshRenderer>();
			CombatView instance = CombatView.Instance;
			if (instance.ExitAreaFence != null)
			{
				instance.ExitAreaFence.SetActive(value: true);
			}
			if (component != null && component2 != null)
			{
				component.material.SetColor(MaterialParameters.TintColor, visualizer.EnabledColor);
				component2.material.SetColor(MaterialParameters.TintColor, visualizer.EnabledOutlineColor);
			}
		}
	}

	private void DisabledVisualization()
	{
		if (visualizer != null)
		{
			MeshRenderer component = visualizer.ShapeFill.GetComponent<MeshRenderer>();
			MeshRenderer component2 = visualizer.ShapeOutline.GetComponent<MeshRenderer>();
			CombatView instance = CombatView.Instance;
			if (instance.ExitAreaFence != null)
			{
				instance.ExitAreaFence.SetActive(value: false);
			}
			if (component != null && component2 != null)
			{
				component.material.SetColor(MaterialParameters.TintColor, visualizer.DisabledColor);
				component2.material.SetColor(MaterialParameters.TintColor, visualizer.DisabledOutlineColor);
			}
		}
	}

	private void CheckEnabled()
	{
		if (base.Model.Enabled)
		{
			EnabledVisualization();
		}
		else
		{
			DisabledVisualization();
		}
	}

	public void UpdateViewFromTask()
	{
		CheckEnabled();
	}
}
