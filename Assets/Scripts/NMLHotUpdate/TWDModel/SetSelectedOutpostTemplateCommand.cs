using BaseModel;

namespace TWDModel
{
	public class SetSelectedOutpostTemplateCommand : ModelCommand
	{
		public string TemplateId { get; set; }

		public SetSelectedOutpostTemplateCommand()
		{
		}

		public SetSelectedOutpostTemplateCommand(string templateId)
		{
			TemplateId = templateId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			((TWDModelManager)manager).Player.SetSelectedOutpostTemplateDefinitionId(TemplateId);
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
