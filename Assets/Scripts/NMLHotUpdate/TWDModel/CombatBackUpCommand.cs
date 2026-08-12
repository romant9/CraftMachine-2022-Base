using BaseModel;

namespace TWDModel
{
	public class CombatBackUpCommand : ModelCommand
	{
		public int Turn { get; set; }

		public CombatBackUpCommand()
		{
		}

		public CombatBackUpCommand(int turn)
		{
			Turn = turn;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { CombatModel: not null } tWDModelManager) || !tWDModelManager.Player.SubscriptionManager.IsSubscriptionActive)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			bool flag = false;
			if (tWDModelManager.CombatModel.BackUpCount > 0 && !flag)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			int num = tWDModelManager.CombatModel.TurnManager.TurnCount - Turn + 1;
			int num2 = tWDModelManager.Player.CombatBackups.Count - Turn;
			if (num < 0 || num2 < 0)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Player.CombatBackups.RemoveAfter(num2);
			tWDModelManager.Player.Combat.TurnManager.TurnCount = num;
			tWDModelManager.Player.CombatBackups[num2].BackUp();
			tWDModelManager.Player.Combat.BackUpCount++;
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
