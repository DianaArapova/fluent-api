using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
	public static class ObjectExtention
	{
		public static string PrintToString<T>(this T obj)
		{
			return ObjectPrinter.For<T>().PrintToString(obj);
		}

		public static string PrintToString<T>(this T obj,
			Func<PrintingConfig<T>, PrintingConfig<T>> funcForPrint)
		{
			return funcForPrint(ObjectPrinter.For<T>()).PrintToString(obj);
		}
	}
}
