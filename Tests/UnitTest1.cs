using Microsoft.VisualStudio.TestTools.UnitTesting;
using StringFormatter;
using System;

namespace Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void Test()
		{
			var testObject = new TestClass();
			var stringFormatter = StringFormatter.StringFormatter.Shared;
			var result = stringFormatter.Format( "field = {field} name = {name} Bar = {Bar}", testObject);
			Assert.IsTrue( result.Equals( "field = 5 name = Foo Bar = Bar" ) );
			Console.WriteLine( result );
		}
		
		[TestMethod]
		public void DisplayingTest()
		{
			var testObject = new TestClass();
			var stringFormatter = StringFormatter.StringFormatter.Shared;
			var result = stringFormatter.Format( "{field} fields are in a {{big}} country {name}{Bar}", testObject );
			Assert.IsTrue( result.Equals( "5 fields are in a {big} country FooBar" ) );
			Console.WriteLine( result );
		}

		[TestMethod]
		public void DisbalanceTest1()
		{
			var testOject = new TestClass();
			var stringFormatter = StringFormatter.StringFormatter.Shared;
			string result;
			try
			{
				result = stringFormatter.Format( "My Age is field}", testOject );
				Assert.Fail();
			}
			catch (ArgumentException e)
			{
				e.Message.Equals( "\"}\" does not match for \"{\"" );				
			}
		}

		[TestMethod]
		public void DisbalanceTest2()
		{
			var testOject = new TestClass();
			var stringFormatter = StringFormatter.StringFormatter.Shared;
			string result;
			try
			{
				result = stringFormatter.Format( "My Age is {field", testOject );
				Assert.Fail();
			}
			catch ( ArgumentException e )
			{
				e.Message.Equals( "\"}\" does not match for \"{\"" );
			}
		}

		[TestMethod]
		public void DisbalanceTest3()
		{
			var testOject = new TestClass();
			var stringFormatter = StringFormatter.StringFormatter.Shared;
			string result;
			try
			{
				result = stringFormatter.Format( "My Name is name}  My Age is {field", testOject );
				Assert.Fail();
			}
			catch ( ArgumentException e )
			{
				Assert.IsTrue(e.Message.Equals( "\"}\" does not match for \"{\"" ));
			}
		}

		[TestMethod]
		public void CollectionTest1()
		{
			var testOject = new TestClass();
			var stringFormatter = StringFormatter.StringFormatter.Shared;
			string result = stringFormatter.Format("collection[1] = {collection[1]}", testOject);
			Assert.IsTrue( result.Equals( "collection[1] = 2" ) );
			Console.WriteLine( result );
		}
		
		//[TestMethod]
		//public void CollectionTest2()
		//{
		//	var testOject = new TestClass();
		//	var stringFormatter = StringFormatter.StringFormatter.Shared;
		//	string result = stringFormatter.Format( "ConcurrentCollection[1] = {concCollection[1]}", testOject );
		//	Assert.IsTrue( result.Equals( "ConcurrentCollection[1] = 2" ) );
		//	Console.WriteLine( result );
		//}

		[TestMethod]
		public void PrivateFieldTest()
		{
			var testOject = new TestClass();
			var stringFormatter = StringFormatter.StringFormatter.Shared;
			string result;
			try
			{
				result = stringFormatter.Format( "private field _x = {_x}", testOject );
				Assert.Fail();
			}
			catch (ArgumentException e)
			{
				Assert.IsTrue( e.Message.Equals( "There is not such field or property \"_x\" at TestClass" ) );
			}
			
		}
		[TestMethod]
		public void InnerMethodTest()
		{
			var testObject = new TestClass();
			var result = testObject.ToString();
			Assert.IsTrue( result.Equals( "_x = 42" ) );
			Console.Write( result );

		}


	}
}