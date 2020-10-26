
using System;
using System.Collections;
using System.Collections.Generic;

namespace A2v10.SimpleWF
{
	public class Decision : Activity
	{
		public String Condition { get; set; }

		public String Then { get; set; }
		public String Else { get; set; }

		public override void Compile(Compiler compiler)
		{
			compiler.StartActivity(this);
			compiler.Emit(OpCode.Condition, Condition);
			compiler.EmitOffset(OpCode.BrFalse, +3); // to else
			compiler.Emit(OpCode.Invoke, Then);
			compiler.EmitOffset(OpCode.Goto, 2); // to next
			compiler.Emit(OpCode.Invoke, Else);
			compiler.EndActivity(this);
		}

		public override ExecState ExecuteImmediate(ExecuteContext context)
		{
			var cond = context.Evaluate(Condition);
			if (cond)
			{
				if (Then != null)
					return Parent.FindActivity(Then).ExecuteImmediate(context);
			}
			else 
			{
				if (Else != null)
					return Parent.FindActivity(Else).ExecuteImmediate(context);
			}
			return ExecuteNext(context);
		}
	}
}
