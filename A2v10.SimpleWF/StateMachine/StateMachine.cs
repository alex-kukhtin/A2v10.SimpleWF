﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A2v10.SimpleWF
{
	public class StateMachine : Activity
	{
		public List<StateBase> States { get; set; } = new List<StateBase>();

		public override void Compile(Compiler compiler)
		{
			// executing
			compiler.StartActivity(this);
			var first = States.FirstOrDefault(x => x.IsStart);
			if (first == null)
				throw new ArgumentOutOfRangeException("There is no 'Start' state");
			var final = States.FirstOrDefault(x => x.IsFinal);
			if (final == null)
				throw new ArgumentOutOfRangeException("There is no 'Final' state");
			// Alloc local in data stack. This is a "CurrentState"
			compiler.Emit(OpCode.Alloc);
			// Set initial state
			compiler.Emit(OpCode.Store, first.Name);
			// loop start
			Int32 loopStart = compiler.Emit(OpCode.Nop);
			compiler.Emit(OpCode.Equal, final.Name); // compare argument with top of data stack
			compiler.EmitOffset(OpCode.BrFalse, +4);
			compiler.Emit(OpCode.Invoke, final.Ref); // Entry for FinalState
			// remove local from data stack
			compiler.Emit(OpCode.Dealloc);
			compiler.EndActivity(this);
			// Swith does not Invoke, but makes Goto!
			compiler.Emit(OpCode.Push, null, loopStart); // for "Ret" from Steps
			Int32 emitAddr = compiler.Emit(OpCode.Switch, null, 0);
			compiler.EmitOffset(OpCode.Goto, -7 /*loop start*/);
			compiler.EndActivity(this);

			// compile children
			var brTable = new Dictionary<String, Int32>();
			foreach (var state in States)
			{
				Int32 stepAddr = compiler.CP;
				state.Compile(compiler);
				brTable.Add(state.Name, stepAddr);
			}
			// lookup table for this stateMachine
			Int32 lookupTableAddress = compiler.CP;
			foreach (var br in brTable)
			{
				compiler.Emit(OpCode.Data, br.Key, br.Value);
			}
			compiler.Emit(OpCode.Data, null, -1);
			compiler.SetAddress(emitAddr, lookupTableAddress);
		}
	}
}
