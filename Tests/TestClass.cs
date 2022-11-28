using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
	public class TestClass
	{
		public int field = 5;
		public string name = "Foo";
		public string Bar { get;  } = "Bar";

		public TestClass(int f, string n, string b)
		{
			field = f;
			name = n;
			Bar = b;
		}

		public TestClass()
		{

		}

	}
}
