﻿
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
			// order is important!
			for (var i = 0; i < Steps.Count; i++)
			{
				var st = Steps[i].ExecuteImmediate(context);
				if (st != ExecState.Complete)
					return st;
			}
			return ExecuteNext(context);
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
