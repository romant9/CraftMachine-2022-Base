using BaseModel;
using BaseModel.ContentTypes;

namespace TWDModel
{
	public class VideoAdFillAnalyticsCommand : ModelCommand
	{
		public AdProvider Provider;

		public bool Available;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager as TWDModelManager).Metrics.AddReceive().AddVideoAd(Provider, AdStatus.OK).AddFill(Available)
				.Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
