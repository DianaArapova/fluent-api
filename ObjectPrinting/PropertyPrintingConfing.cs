using System;

namespace ObjectPrinting
{
	public class PropertyPrintingConfing<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
	{
		private readonly PrintingConfig<TOwner> printingConfig;
		private readonly string propertyName;

		public PropertyPrintingConfing(PrintingConfig<TOwner> printingConfig, string propertyName = null)
		{
			this.propertyName = propertyName;
			this.printingConfig = printingConfig;
		}

		public PrintingConfig<TOwner> Using(Func<TPropType, string> serializeFunc)
		{
			var configure = printingConfig.CopyCurrentConfig();
			if (propertyName != null)
				configure.PropertySerializator = 
					configure.PropertySerializator.SetItem(propertyName, serializeFunc);
			else
				configure.TypeSerializator = 
					configure.TypeSerializator.SetItem(typeof(TPropType), serializeFunc);
			return configure;
		}

		PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>
			.PrintingConfig => printingConfig;


	}
}
