﻿
using System;
using System.Collections.Generic;

namespace A2v10.SimpleWF
{
	public class State : StateBase
	{
		public Activity Entry { get; set; }
		public Activity Exit { get; set; }

		public List<Transition> Transitions { get; set; } = new List<Transition>();

		public override void Compile(Compiler compiler)
		{
			// executing
			compiler.StartActivity(this);
			if (Entry != null)
				compiler.Emit(OpCode.Invoke, Entry.Ref);

			compiler.Emit(OpCode.Store, NextState);

			compiler.EmitOffset(OpCode.Goto, +4); // to transitions

			Int32 endAddress = compiler.Emit(OpCode.Nop);
			if (Exit != null)
				compiler.Emit(OpCode.Invoke, Exit.Ref);
			else
				compiler.Emit(OpCode.Nop);

			compiler.EndActivity(this);

			for (int i=0; i<Transitions.Count; i++)
			{
				// order is important!
				var trans = Transitions[i];
				trans.CompileAction(compiler);
			}
			compiler.Emit(OpCode.Goto, null, endAddress); // break


			// compile children
			if (Entry != null)
				Entry.Compile(compiler);
			if (Exit != null)
				Exit.Compile(compiler);
			foreach (var tr in Transitions)
				tr.Compile(compiler);
		}

		public override ExecState ExecuteState(ExecuteContext context, out String nextState)
		{
			if (Entry != null)
				Entry.ExecuteImmediate(context);
			for (var i = 0; i<Transitions.Count; i++)
			{
				var tr = Transitions[i];
				if (tr.Trigger != null)
					tr.Trigger.ExecuteImmediate(context);
				if (context.Evaluate(tr.Condition)) {
					tr.Action.ExecuteImmediate(context);
					nextState = tr.Destination;
					return EndExecute(context);
				}
				
			}
			nextState = NextState;
			return EndExecute(context);
		}

		ExecState EndExecute(ExecuteContext context)
		{
			if (Exit != null)
				return Exit.ExecuteImmediate(context);
			return ExecState.Complete;
		}
	}
}
