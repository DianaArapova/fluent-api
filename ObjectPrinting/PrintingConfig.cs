using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
	public class PrintingConfig<TOwner>
	{
		private ImmutableHashSet<Type> excludedTypes = 
			ImmutableHashSet<Type>.Empty;
	    private ImmutableHashSet<string> excludedProperties =
		    ImmutableHashSet<string>.Empty;
		public ImmutableDictionary<Type, Delegate> TypeSerializator = 
			ImmutableDictionary<Type, Delegate>.Empty;
	    public ImmutableDictionary<string, Delegate> PropertySerializator = 
			ImmutableDictionary<string, Delegate>.Empty;
	    public ImmutableDictionary<Type, CultureInfo> Cultures = 
			ImmutableDictionary<Type, CultureInfo>.Empty;

	    public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0, new Stack<object>());
        }

		private string PrintToString(object obj, int nestingLevel, Stack<object> grayObjectInDfs)
		{
			//TODO apply configurations
			if (obj == null)
				return "null" + Environment.NewLine;

			var finalTypes = new[]
			{
				typeof(int), typeof(double), typeof(float), typeof(string),
				typeof(DateTime), typeof(TimeSpan),
			};
			
			if (finalTypes.Contains(obj.GetType()))
			{
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
				grayObjectInDfs.Push(obj);
				if (grayObjectInDfs.Contains(propertyInfo.GetValue(obj)))
				{
					sb.Append(identation).Append(propertyInfo.Name).Append(" = a looped object")
						.Append(Environment.NewLine);
					continue;
				}
				
				if (obj is IEnumerable)
				{
					sb.Append(PrintWithInformation(obj, nestingLevel, propertyInfo, grayObjectInDfs));
					break;
				}
				sb.Append(identation + propertyInfo.Name + " = " +
				          PrintWithInformation(obj, nestingLevel, propertyInfo, grayObjectInDfs));
			}
			grayObjectInDfs.Pop();
			return sb.ToString();
		}

	    string PrintWithInformation(object obj, int nestingLevel, PropertyInfo propertyInfo, Stack<object> grayObjectInDfs)
	    {
		    if (PropertySerializator.ContainsKey(propertyInfo.Name))
			    return PropertySerializator[propertyInfo.Name]
				           .DynamicInvoke(propertyInfo.GetValue(obj)) 
						   + Environment.NewLine;

		    if (TypeSerializator.ContainsKey(propertyInfo.PropertyType))
			    return TypeSerializator[propertyInfo.PropertyType]
				           .DynamicInvoke(propertyInfo.GetValue(obj)) 
						   + Environment.NewLine;

		    if (obj is IEnumerable enumerable)
		    {   
				var sb = new StringBuilder();
			    foreach (var a in enumerable)
			    {
					sb.Append(PrintToString(a, nestingLevel + 1, new Stack<object>()));
			    }
			    return sb.ToString();
			}

		    if (Cultures.ContainsKey(propertyInfo.PropertyType))
		    {
			    return ((IFormattable)propertyInfo.GetValue(obj))
			           .ToString(null, CultureInfo.CurrentCulture) 
					   + Environment.NewLine;
		    }

		    return PrintToString(propertyInfo.GetValue(obj),
			    nestingLevel + 1, grayObjectInDfs);
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
		    var configure = CopyCurrentConfig();
		    configure.excludedProperties = configure.excludedProperties.Add(GetPropertyName(selector));
		    return configure;
	    }

	    public PrintingConfig<TOwner> ExcludeType<T>()
	    {
		    var configure = CopyCurrentConfig();
		    configure.excludedTypes = configure.excludedTypes.Add(typeof(T));
		    return configure;
	    }

		public PrintingConfig<TOwner> CopyCurrentConfig()
		{
			return (PrintingConfig<TOwner>)MemberwiseClone();
		}
	}
}