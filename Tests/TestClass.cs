using System;
using System.Collections.Concurrent;
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

		private int _x = 42;
		public string Bar { get;  } = "Bar";
		public List<int> collection = new List<int>(){ 1, 2, 3, 4, 5, 6, 7, 8, 9};

		public int[] array = new int[] { 1, 2, 3, 4, 5, 6 };

		public ConcurrentBag<int> concCollection = new ConcurrentBag<int> { 10, 11, 12, 13, 14, 15, 16 };

		public TestClass(int f, string n, string b, List<int> c1, int x)
		{
			field = f;
			name = n;
			Bar = b;
			collection = c1;
			_x = x;
		}

		public TestClass()
		{			
		}

		public override string ToString()
		{
			return StringFormatter.StringFormatter.Shared.Format( "_x = {_x}", this );
		}

	}
}
