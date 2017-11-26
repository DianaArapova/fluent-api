using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
	public class PrintingConfig<TOwner>
    {
	    private readonly HashSet<Type> excludedTypes = 
			new HashSet<Type>();
	    private readonly HashSet<string> excludedProperties = 
			new HashSet<string>();
		public readonly Dictionary<Type, Delegate> TypeSerializator = 
			new Dictionary<Type, Delegate>();
	    public readonly Dictionary<string, Delegate> PropertySerializator = 
			new Dictionary<string, Delegate>();
	    public readonly Dictionary<Type, CultureInfo> Cultures = 
			new Dictionary<Type, CultureInfo>();

	    public int? Length { get; set; }

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
	        {
		        if (obj is string str && Length != null)
			        return str.Substring(0, Length.Value) + Environment.NewLine;
		        return obj + Environment.NewLine;
	        }

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
				          PrintWithInformation(obj, nestingLevel, propertyInfo));
            }
            return sb.ToString();
        }

	    string PrintWithInformation(object obj, int nestingLevel, PropertyInfo propertyInfo)
	    {
		    if (PropertySerializator.ContainsKey(propertyInfo.Name))
			    return PropertySerializator[propertyInfo.Name]
				           .DynamicInvoke(propertyInfo.GetValue(obj)) 
						   + Environment.NewLine;

		    if (TypeSerializator.ContainsKey(propertyInfo.PropertyType))
			    return TypeSerializator[propertyInfo.PropertyType]
				           .DynamicInvoke(propertyInfo.GetValue(obj)) 
						   + Environment.NewLine;

		    if (Cultures.ContainsKey(propertyInfo.PropertyType))
		    {
			    return ((IFormattable)propertyInfo.GetValue(obj))
			           .ToString(null, CultureInfo.CurrentCulture) 
					   + Environment.NewLine;
		    }

		    return PrintToString(propertyInfo.GetValue(obj),
			    nestingLevel + 1);
	    }

		private string GetPropertyName<TPropType>(Expression<Func<TOwner, TPropType>> selector)
	    {
		    return (((MemberExpression)selector.Body).Member as PropertyInfo)?.Name;

	    }

		public PropertyPrintingConfing<TOwner, TPropType> Printing<TPropType>()
	    {
		    return new PropertyPrintingConfing<TOwner,TPropType>(this);
	    }

	    public PropertyPrintingConfing<TOwner, TPropType> Printing<TPropType>
			(Expression<Func<TOwner, TPropType>> selector)
	    {
			return new PropertyPrintingConfing<TOwner, TPropType>(
				this, GetPropertyName(selector));
	    }

	    public PrintingConfig<TOwner> ExcludeProperty<TPropType>
			(Expression<Func<TOwner, TPropType>> selector)

	    {
			excludedProperties.Add(GetPropertyName(selector));
		    return this;
	    }

	    public PrintingConfig<TOwner> ExcludeType<T>()
	    {
		    excludedTypes.Add(typeof(T));
		    return this;
	    }
    }
}