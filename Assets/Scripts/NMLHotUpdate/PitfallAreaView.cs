using BaseModel;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class PitfallAreaView : CombatAreaView
{
	public override void Kill()
	{
		VisualizationQueue.Instance.Add(new CustomVisualizationTask(delegate
		{
			base.Kill();
		}));
	}

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		CombatArea combatArea = (CombatArea)base.Model;
		if (combatArea.Radius <= 1L)
		{
			SetNewScale(0.25f);
			return;
		}
		GridCoordinate coordinate = new GridCoordinate(combatArea.Coordinate.X + 1, combatArea.Coordinate.Y - 1);
		GridCoordinate coordinate2 = combatArea.Coordinate;
		Vector3 vector = GridView.Instance.GetPosition(coordinate).ToVector3();
		Vector3 vector2 = GridView.Instance.GetPosition(coordinate2).ToVector3();
		base.transform.position = (vector + vector2) / 2f;
		SetNewScale(0.5f);
	}
}
