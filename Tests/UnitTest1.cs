using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringFormatter;
using System;

namespace Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void CommonTest()
		{
			var testObject = new TestClass();
			var stringFormatter = StringFormatter.StringFormatter.Shared;
			var result = stringFormatter.Format( "field = {field} name = {name} Bar = {Bar} ", testObject);
			Assert.IsTrue( result.Equals( "field = 5 name = Foo Bar = Bar " ) );
			Console.WriteLine( result );
		}
	}
}