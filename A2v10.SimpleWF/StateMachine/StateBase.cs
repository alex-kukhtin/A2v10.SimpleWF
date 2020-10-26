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

		public virtual ExecState ExecuteState(ExecuteContext context, out String nextState)
		{
			nextState = null;
			return ExecState.Complete;
		}
	}
}
