
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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

		public override ExecState ExecuteImmediate(ExecuteContext context)
		{
			String stepName = "";
			if (context.IsContinue)
				stepName = context.Restore<String>(Ref, "Step");
			else
				stepName = Steps.FirstOrDefault(x => x.IsStart).Ref;
			var step = FindActivity(stepName);
			var st = step.ExecuteImmediate(context);
			if (st != ExecState.Complete)
				return st;
			return ExecuteNext(context);
		}


		public override Activity FindActivity(string reference)
		{
			return Steps.First(x => x.Ref == reference);
		}

		public override void OnInit(Activity parent)
		{
			Parent = parent;
			foreach (var p in Steps)
				p.OnInit(this);
		}
	}
}
