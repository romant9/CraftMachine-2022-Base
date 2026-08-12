using BaseModel;

namespace TWDModel
{
	public class SpeedUpCuringAllSurvivorsCommand : ConsumeCurrencyCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			MedicTentModel medicTentModel = ((PlayerModel)manager.GetPlayer()).Camp.GetBuilding("MedicTent") as MedicTentModel;
			TWDModelResult result = TWDModelResult.Error;
			if (medicTentModel != null)
			{
				result = medicTentModel.CureAllSurvivors();
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
