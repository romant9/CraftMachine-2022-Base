using BaseModel;

namespace TWDModel
{
	public class RandomizeSurvivorCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			(manager as TWDModelManager).CombatModel.AddLootSurvivor(tWDModelManager.Player.LootManager.GetGeneratedSurvivor());
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
