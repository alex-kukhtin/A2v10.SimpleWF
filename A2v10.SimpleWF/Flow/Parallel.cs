
using Jint.Runtime.Debugger;
using System;
using System.Collections;
using System.Collections.Generic;

namespace A2v10.SimpleWF
{
	public enum CompletionCondition
	{
		Any,
		All
	}

	public class Parallel : Activity
	{
		public IList<Activity> Branches { get; } = new List<Activity>();

		public CompletionCondition CompletionCondition { get; set; }

		public override void Compile(Compiler compiler)
		{
			compiler.StartActivity(this);
			foreach (var br in Branches)
			{
				compiler.Emit(OpCode.Invoke, br.Ref);
			}
			compiler.EndActivity(this);

			// compile children
			foreach (var step in Branches)
				step.Compile(compiler);
		}
	}
}
