using System;

namespace TWDModel
{
	[Serializable]
	public class LoopingSoundNode : NodeBase
	{
		public const string StatesChanged = "LoopingSoundPlayState";

		public LevelLoopingSoundModel Sound;

		public LoopingSoundNode()
		{
		}

		public LoopingSoundNode(LoopingSoundNode node)
			: base(node)
		{
			Sound = node.Sound;
		}

		public override NodeBase RecordValue()
		{
			return new LoopingSoundNode(this);
		}

		public override void Initialize()
		{
			base.Initialize();
			NotifyChange("LoopingSoundPlayState");
		}

		[GraphItInput("Start All Looping Sounds", "")]
		public void StartAllLoopingSounds()
		{
			foreach (LevelLoopingSoundModel model in base.manager.GetModels<LevelLoopingSoundModel>())
			{
				model.SetLoopingSoundPlayState(LoopingSoundPlayState.Started);
			}
			NotifyChange("LoopingSoundPlayState");
		}

		[GraphItInput("Stop All Looping Sounds", "")]
		public void StopAllLoopingSounds()
		{
			foreach (LevelLoopingSoundModel model in base.manager.GetModels<LevelLoopingSoundModel>())
			{
				if (model.LoopingSoundPlayState == LoopingSoundPlayState.Started)
				{
					model.SetLoopingSoundPlayState(LoopingSoundPlayState.Stopped);
				}
			}
			NotifyChange("LoopingSoundPlayState");
		}

		[GraphItInput("Start Selected Sound", "")]
		public void StartSound()
		{
			if (Sound != null)
			{
				Sound.SetLoopingSoundPlayState(LoopingSoundPlayState.Started);
				NotifyChange("LoopingSoundPlayState");
			}
		}

		[GraphItInput("Stop Selected Sound", "")]
		public void StopSound()
		{
			if (Sound != null)
			{
				Sound.SetLoopingSoundPlayState(LoopingSoundPlayState.Stopped);
				NotifyChange("LoopingSoundPlayState");
			}
		}
	}
}
