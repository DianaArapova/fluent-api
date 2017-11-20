using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
	public class PropertyPrintingConfing<TOwner, TPropType> : IPropertyPrintingConfig<TOwner>
	{
		private readonly PrintingConfig<TOwner> printingConfig;
		public PropertyPrintingConfing(PrintingConfig<TOwner> printingConfig)
		{
			this.printingConfig = printingConfig;
		}

		public PrintingConfig<TOwner> Using(Func<TPropType, string> seializeFunc)
		{
			return printingConfig;
		}

		PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner>
			.PrintingConfig => printingConfig;


	}
}
