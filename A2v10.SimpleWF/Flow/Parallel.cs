
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

		public override ExecState ExecuteImmediate(ExecuteContext context)
		{

			return ExecuteNext(context);
		}

		public override void Compile(Compiler compiler)
		{
			compiler.StartActivity(this);

			foreach (var br in Branches)
			{
			}

			Int32 switchAddr = compiler.Emit(OpCode.Switch, null, 0);
			compiler.EndActivity(this);

			// compile children
			var brTable = new Dictionary<String, Int32>();
			foreach (var br in Branches)
			{
				Int32 stepAddr = compiler.CP;
				br.Compile(compiler);
				brTable.Add(br.Ref, stepAddr);
			}
			// lookup table for this stateMachine
			Int32 lookupTableAddress = compiler.CP;
			foreach (var br in brTable)
			{
				compiler.Emit(OpCode.Data, br.Key, br.Value);
			}
			compiler.Emit(OpCode.Data, null, -1);
			compiler.SetAddress(switchAddr, lookupTableAddress);
		}
	}
}
