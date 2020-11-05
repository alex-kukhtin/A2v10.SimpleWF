
using System;
using System.Collections;
using System.Collections.Generic;

namespace A2v10.SimpleWF
{
	public class Sequence : Activity
	{
		public IList<Activity> Steps { get; } = new List<Activity>();

		public override void Compile(Compiler compiler)
		{
			compiler.StartActivity(this);
			// order is important!
			for (var i = 0; i < Steps.Count; i++)
				compiler.Emit(OpCode.Invoke, Steps[i].Ref);
			compiler.EndActivity(this);

			// compile children
			foreach (var step in Steps)
				step.Compile(compiler);
		}

		public override ExecState ExecuteImmediate(ExecuteContext context)
		{
			Int32 index = 0;
			if (context.IsContinue)
				index = context.Restore<Int32>(Ref, "Index");
			while (index < Steps.Count)
			{
				context.Store(Ref, "Index", index);
				var st = Steps[index].ExecuteImmediate(context);
				if (st != ExecState.Complete)
					return st;
				index += 1;
			}
			return ExecState.Complete;
		}


		public override DynamicObject Store()
		{
			var d = new DynamicObject();
			d.Set("Ref", Ref);
			d.Set("Type", nameof(Sequence));
			var list = new List<DynamicObject>();
			foreach (var s in Steps)
				list.Add(s.Store());
			d.Set("Steps", list);
			return d;
		}

	}
}
