using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.SimpleWF
{
	public class StateBase : Activity
	{
		public virtual Boolean IsFinal => false;

		public String Name { get; set; }
		public String NextState { get; set; }
	}
}
