using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
	public class PrintingConfig<TOwner>
    {
	    private readonly HashSet<Type> excludedTypes = new HashSet<Type>();
	    private readonly HashSet<string> excludedProperties = new HashSet<string>();

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
	            if (excludedProperties.Contains(propertyInfo.Name))
		            continue;
	            if (excludedTypes.Contains(propertyInfo.PropertyType))
		            continue;
				sb.Append(identation + propertyInfo.Name + " = " +
                          PrintToString(propertyInfo.GetValue(obj),
                              nestingLevel + 1));
            }
            return sb.ToString();
        }

	    public PropertyPrintingConfing<TOwner, TPropType> Printing<TPropType>()
	    {
		    return new PropertyPrintingConfing<TOwner,TPropType>(this);
	    }

	    public PropertyPrintingConfing<TOwner, TPropType> Printing<TPropType>
			(Expression<Func<TOwner, TPropType>> selector)
	    {
			return new PropertyPrintingConfing<TOwner, TPropType>(this);
	    }

	    public PrintingConfig<TOwner> ExcludeProperty<TPropType>
			(Expression<Func<TOwner, TPropType>> selector)

	    {
		    var propetyInformation =
			    ((MemberExpression) selector.Body).Member as PropertyInfo;
		    if (propetyInformation != null)
				excludedProperties.Add(propetyInformation.Name);
		    return this;
	    }

	    public PrintingConfig<TOwner> ExcludeType<T>()
	    {
		    excludedTypes.Add(typeof(T));
		    return this;
	    }
    }
}