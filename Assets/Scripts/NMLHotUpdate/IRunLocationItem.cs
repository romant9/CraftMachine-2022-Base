using TWDModel;

public interface IRunLocationItem
{
	TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors);
}
