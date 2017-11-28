using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
	class E
	{
		public int[] arr { get; set; }
	}
	[TestFixture]
	public class ObjectPrinterAcceptanceTests
	{
		[Test]
		public void PrintIEnumerable()
		{
			var printer = ObjectPrinter.For<E>();

			Console.WriteLine(printer
				.PrintToString(new E {arr = new { 1, 2} }));
	}
		[Test]
		public void Demo()
		{
			var person = new Person { Name = "Alex", Age = 19 };

			var printer = ObjectPrinter.For<Person>()
				//1. Исключить из сериализации свойства определенного типа
				.ExcludeType<Guid>()
				//2. Указать альтернативный способ сериализации для определенного типа
				.Printing<int>().Using(i => i.ToString())
				//3. Для числовых типов указать культуру
				.Printing<double>().Using(CultureInfo.CurrentCulture)
				//4. Настроить сериализацию конкретного свойства
				.Printing(p => p.Age).Using(age => age.ToString())
				//5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
				.Printing<string>().TakeChars(3)
				//6. Исключить из сериализации конкретного свойства
				.ExcludeProperty(p => p.Name);

			string s1 = printer.PrintToString(person);
			Console.WriteLine(s1);
			//7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию		
			Console.WriteLine(person.PrintToString());
			//8. ...с конфигурированием
			Console.WriteLine(person.PrintToString(prop => prop.Printing<string>().TakeChars(1).ExcludeType<Guid>()));
		}

		[Test]
		public void PrintingConfig_TakeChar_ShouldCutCorrectly()
		{
			var person = new Person { Name = "Diana", Age = 20, Height = 165.4 };
			var expected = string.Join(Environment.NewLine,
				               "Person", "\tId = Guid", "\tName = Di",
				               "\tHeight = 165,4", "\tAge = 20") + Environment.NewLine;
			ObjectPrinter.For<Person>()
				.Printing<string>().TakeChars(2)
				.PrintToString(person)
				.Should()
				.Be(expected);
		}

		[Test]
		public void PrintingConfig_SetCulture_ShouldGetCorrectrlyCulture()
		{
			var person = new Person { Name = "Diana", Age = 20, Height = 165.4 };
			var expected = string.Join(Environment.NewLine,
				               "Person", "\tId = Guid", "\tName = Diana",
				               "\tHeight = 165,4", "\tAge = 20") + Environment.NewLine;
			ObjectPrinter.For<Person>()
				.Printing<double>().Using(CultureInfo.CurrentCulture)
				.PrintToString(person)
				.Should()
				.Be(expected);
		}

		[Test]
		public void PrintingConfing_ExcludeProperty()
		{
			var person = new Person { Name = "Diana", Age = 20, Height = 165.4 };
			var expected = string.Join(Environment.NewLine,
				               "Person", "\tId = Guid", "\tName = Diana",
				               "\tHeight = 165,4") + Environment.NewLine;
			ObjectPrinter.For<Person>()
				.ExcludeProperty(p => p.Age)
				.PrintToString(person)
				.Should()
				.Be(expected);
		}

		[Test]
		public void PrintingConfing_ExcludeType()
		{
			var person = new Person { Name = "Diana", Age = 20, Height = 165.4 };
			var expected = string.Join(Environment.NewLine,
				               "Person", "\tName = Diana",
				               "\tHeight = 165,4", "\tAge = 20") + Environment.NewLine;
			ObjectPrinter.For<Person>()
				.ExcludeType<Guid>()
				.PrintToString(person)
				.Should()
				.Be(expected);
		}

		[Test]
		public void PrintingConfing_IsImmutable()
		{
			var person = new Person { Name = "Diana", Age = 20, Height = 165.4 };
			var expected1 = string.Join(Environment.NewLine,
				               "Person", "\tId = Guid", "\tName = Di",
				               "\tHeight = 165,4", "\tAge = 20") + Environment.NewLine;
			var expected2 = string.Join(Environment.NewLine,
				                "Person", "\tId = Guid", "\tName = D",
				                "\tHeight = 165,4", "\tAge = 20") + Environment.NewLine;
			var painter = ObjectPrinter.For<Person>();
			var take1 = painter.Printing<string>()
				.TakeChars(2);
			var take2 = painter.Printing<string>()
				.TakeChars(1);
			take1.PrintToString(person)
				.Should()
				.Be(expected1);
			take2.PrintToString(person)
				.Should()
				.Be(expected2);
		}
	}
}