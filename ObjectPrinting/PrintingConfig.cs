using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            //TODO apply configurations
            if (obj == null)
                return "null" + Environment.NewLine;

            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };
            if (finalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                sb.Append(identation + propertyInfo.Name + " = " +
                          PrintToString(propertyInfo.GetValue(obj),
                              nestingLevel + 1));
            }
            return sb.ToString();
        }

	    public PrintingConfig<TOwner> ExludeType<TProp>()
	    {
		    return this;
	    }

	    public PropertyPrintingConfing<TOwner, TPropType> Printing<TPropType>()
	    {
		    return new PropertyPrintingConfing<TOwner,TPropType>(this);
	    }

	    public void ExcludingProperty(Func<object, bool> func)
	    {
		    return;
	    }

	    public PropertyPrintingConfing<TOwner, TPropType> Printing<TPropType>
			(Expression<Func<TOwner, TPropType>> selector)
	    {
			return new PropertyPrintingConfing<TOwner, TPropType>(this);
	    }
    }

	public static class PropertyPrintingConfingExtention
	{
		public static PrintingConfig<TOwner> Using<TOwner>(
			this PropertyPrintingConfing<TOwner, int> a,
			CultureInfo cultureInfo)
		{
			return ((IPropertyPrintingConfig<TOwner, int>) a).PrintingConfig;
		}
	}

	public class PropertyPrintingConfing<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
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

		PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.
			PrintingConfig => printingConfig;
	}

	public interface IPropertyPrintingConfig<TOwner, TProperty>
	{
		PrintingConfig<TOwner> PrintingConfig { get; }
	}
}