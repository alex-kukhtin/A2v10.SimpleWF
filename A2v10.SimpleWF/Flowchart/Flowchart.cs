
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace A2v10.SimpleWF
{
	public class Flowchart : Activity
	{
		public IList<Activity> Steps { get; } = new List<Activity>();

		public override void Compile(Compiler compiler)
		{
			compiler.StartActivity(this);

			var start = Steps.FirstOrDefault(x => x.IsStart);
			compiler.Emit(OpCode.Invoke, start.Ref);
			compiler.Emit(OpCode.Ret);
			compiler.EndActivity(this);

			foreach (var step in Steps)
				step.Compile(compiler);
		}
	}
}
