namespace TWDModel
{
	public class TutorialQuest : QuestModel
	{
		public string TutorialIdentifier { get; private set; }

		public override int CompletedSteps
		{
			get
			{
				if (!base.manager.Player.Tutorial.HasCompletedPart(TutorialIdentifier))
				{
					return 0;
				}
				return 1;
			}
		}

		public TutorialQuest()
		{
		}

		public TutorialQuest(string tutorialIdentifier)
		{
			TutorialIdentifier = tutorialIdentifier;
		}

		public override void Start()
		{
			base.Start();
			if (!base.manager.Player.Tutorial.HasCompletedPart(TutorialIdentifier) && base.manager.Player.Tutorial.CurrentPartId != TutorialIdentifier)
			{
				base.manager.Player.Tutorial.SetPart(TutorialIdentifier);
			}
		}
	}
}
