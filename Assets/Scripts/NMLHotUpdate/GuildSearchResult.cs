using BaseModel;
using TWDModel;

public class GuildSearchResult
{
	public enum Source
	{
		Undefined = -1,
		Ad = 0,
		New = 1,
		SameCountry = 2,
		NearLevel = 3,
		Fallback = 4,
		Keyword = 5
	}

	public string modelJson;

	public Source source = Source.Undefined;

	public GuildModel model;

	public GuildSearchResult(string modelJson, Source source)
	{
		this.modelJson = modelJson;
		this.source = source;
	}

	public void DeserializeModel(IMessageSerializer serializer)
	{
		if (modelJson != null)
		{
			model = serializer.DeserializeObject<GuildModel>(modelJson);
			modelJson = null;
		}
	}
}
