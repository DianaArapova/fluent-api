using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			if (propertyName != null)
				printingConfig.PropertySerializator[propertyName] = serializeFunc;
			else
				printingConfig.TypeSerializator[typeof(TPropType)] = serializeFunc;
			return printingConfig;
		}

		PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>
			.PrintingConfig => printingConfig;


	}
}
