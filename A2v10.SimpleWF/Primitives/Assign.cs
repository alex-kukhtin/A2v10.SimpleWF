﻿
using System;
using System.Net;

namespace A2v10.SimpleWF
{
	public class Assign : Activity
	{
		public String To { get; set; }
		public String Value { get; set; }

		public override void Compile(Compiler compiler)
		{
			compiler.StartActivity(this);
			compiler.Emit(OpCode.Script, $"{To} = ({Value});");
			compiler.EndActivity(this);
		}

		public override ExecState ExecuteImmediate(ExecuteContext context)
		{
			if (context.IsContinue)
				return ExecState.Complete;
			context.Execute($"{To} = ({Value});");
			return ExecuteNext(context);
		}
	}
}
