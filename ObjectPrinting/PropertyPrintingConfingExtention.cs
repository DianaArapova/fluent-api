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
			this PropertyPrintingConfing<TOwner, int> a,
			CultureInfo cultureInfo)
		{
			return ((IPropertyPrintingConfig<TOwner, int>)a).PrintingConfig;
		}

		public static PrintingConfig<TOwner> TakeChars<TOwner>(
			this PropertyPrintingConfing<TOwner, string> a, int i)
		{

			return ((IPropertyPrintingConfig<TOwner, string>)a).PrintingConfig;
		}
	}
}
