using System;

namespace TWDModel
{
	[Serializable]
	public class MuteNode : NodeBase
	{
		[GraphItVariable("Mute (true) or unmute (false).")]
		public bool Mute = true;

		public MuteNode()
		{
		}

		public MuteNode(MuteNode node)
			: base(node)
		{
			Mute = node.Mute;
		}

		public override NodeBase RecordValue()
		{
			return new MuteNode(this);
		}

		[GraphItInput("Set Music Mute Forced State", "")]
		public void SetMusicMuteForcedState()
		{
			CombatModel combat = base.manager.Player.Combat;
			if (combat != null)
			{
				combat.MusicMuteForced = Mute;
				combat.NotifyChange("MuteStateChanged", "music");
			}
		}
	}
}
