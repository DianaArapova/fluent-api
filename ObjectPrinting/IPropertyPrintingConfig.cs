namespace ObjectPrinting
{
	public interface IPropertyPrintingConfig<TOwner, TProp>
	{
		PrintingConfig<TOwner> PrintingConfig { get; }
	}
}