﻿namespace ObjectPrinting
{
	public interface IPropertyPrintingConfig<TOwner, TProperty>
	{
		PrintingConfig<TOwner> PrintingConfig { get; }
	}
}