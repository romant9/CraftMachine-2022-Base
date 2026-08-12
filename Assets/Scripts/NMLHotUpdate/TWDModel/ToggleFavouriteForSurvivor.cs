using BaseModel;

namespace TWDModel
{
	public class ToggleFavouriteForSurvivor : ModelCommand
	{
		public ToggleFavouriteForSurvivor()
		{
		}

		public ToggleFavouriteForSurvivor(SurvivorModel survivorModel)
			: base(survivorModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager.GetModel(base.ModelId) is SurvivorModel survivorModel))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			survivorModel.IsFavourite = !survivorModel.IsFavourite;
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
