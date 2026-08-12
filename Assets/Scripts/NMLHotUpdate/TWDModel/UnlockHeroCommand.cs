using BaseModel;

namespace TWDModel
{
	public class UnlockHeroCommand : ConsumeCurrencyCommand
	{
		public CurrencyType Type { get; private set; }

		public UnlockHeroCommand()
		{
		}

		public UnlockHeroCommand(CurrencyType type)
		{
			Type = type;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = (manager as TWDModelManager).Player.SurvivorContainer.UnlockHero(Type);
			return new NGModelCommandRespond(this, result);
		}
	}
}
