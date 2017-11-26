using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
	public static class PropertyPrintingConfingExtention
	{
		public static PrintingConfig<TOwner> Using<TOwner>(
			this PropertyPrintingConfing<TOwner, double> propertyPrintingConfig,
			CultureInfo cultureInfo)
		{
			var printingConfig = ((IPropertyPrintingConfig<TOwner, double>)propertyPrintingConfig)
				.PrintingConfig;
			printingConfig.Cultures.SetItem(typeof(double), cultureInfo);
			return printingConfig;
		}

		public static PrintingConfig<TOwner> TakeChars<TOwner>(
			this PropertyPrintingConfing<TOwner, string> propertyPrintingConfig, int length)
		{
			var printingConfig = ((IPropertyPrintingConfig<TOwner, string>)propertyPrintingConfig)
				.PrintingConfig.CopyCurrentConfig();
			printingConfig.TypeSerializator = printingConfig.TypeSerializator.SetItem(
				typeof(string),
				(Func <string, string>) (obj => obj.Substring(0, length)));
			
			return printingConfig;
		}
	}
}
