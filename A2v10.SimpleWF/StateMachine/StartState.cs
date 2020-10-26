using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.SimpleWF
{
	public class StartState : StateBase
	{
		public override Boolean IsStart => true;

		public override void Compile(Compiler compiler)
		{
			compiler.StartActivity(this);
			compiler.Emit(OpCode.Store, NextState);
			compiler.EndActivity(this);
		}

		public override ExecState ExecuteState(ExecuteContext context, out String nextState)
		{
			nextState =  NextState;
			return ExecState.Complete;
		}
	}
}
