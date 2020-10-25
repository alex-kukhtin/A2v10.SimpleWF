
using System;

namespace A2v10.SimpleWF
{
	public class Transition : Activity
	{
		public String Condition { get; set; }

		public String Destination { get; set; }

		public Activity Trigger { get; set; }
		public Activity Action { get; set; }

		public override void Compile(Compiler compiler)
		{
			// compile children
			if (Trigger != null)
				Trigger.Compile(compiler);
			if (Action != null)
				Action.Compile(compiler);
		}

		public void CompileAction(Compiler compiler)
		{
			if (Trigger != null)
				compiler.Emit(OpCode.Invoke, Trigger.Ref);
			compiler.Emit(OpCode.Condition, Condition);
			compiler.EmitOffset(OpCode.BrFalse, +3); // to next transition
			compiler.EmitInvoke(Action);
			compiler.Emit(OpCode.Store, Destination);
		}
	}
}
