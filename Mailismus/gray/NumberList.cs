
namespace com.grey.@base.collections 
{


	// A fast and simple class for handling lists of Integer primitives.
	// Note that ar_off is always zero for this class, so no need to add it
	public class NumberList
		:System.Collections.Generic.List<int>
	{
		private readonly int increment;

		public void clear() { this.Clear(); }
		public int get(int idx) { return this[idx]; }


		public NumberList() 
			:base(16)
		{  }


		public NumberList(int incr)
			:base(incr)
		{ }

		public void append(int v)
		{
			this.Add(v);
		}

		public void append(System.Collections.Generic.IEnumerable<int> lst)
		{
			foreach (int x in lst)
			{
				this.Add(x);
			}
		}


		public void sort()
		{
			this.Sort();
		}
	}


}
