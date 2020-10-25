
using System;
using System.Collections;
using System.Collections.Generic;

namespace A2v10.SimpleWF
{
	public class If : Activity
	{
		public String Condition { get; set; }
		public Activity Then { get; set; }
		public Activity Else { get; set; }

		public override void Compile(Compiler compiler)
		{
			compiler.StartActivity(this);
			compiler.Emit(OpCode.Condition, Condition);

			compiler.EmitOffset(OpCode.BrFalse, +3); // to else
			compiler.EmitInvoke(Then);
			compiler.EmitOffset(OpCode.Goto, 2); // to next
			compiler.EmitInvoke(Else);

			compiler.EndActivity(this);

			Then?.Compile(compiler);
			Else?.Compile(compiler);
		}
	}
}
