using Jint.Parser.Ast;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace A2v10.SimpleWF
{
	public enum OpCode : Byte
	{
		Nop = 0,
		Stop = 1,
		Halt = 2,
		Start = 3,
		Ret = 4,
		Label = 9,
		Push = 10,
		Pop = 11,
		Alloc = 12,
		Dealloc = 13,
		Store = 14,
		Call = 20, // call activity implementation
		Invoke = 21, // invoke activity
		Condition = 22,
		Equal = 23,
		Script = 24,
		Goto = 43,
		BrTrue = 44,
		BrFalse = 45,
		Switch = 46,
		Wait = 50,
		Data = 200
	}

	public enum CodeState
	{
		Running,
		Complete,
		Wait
	}

	public class StoredCode
	{
		public String Stack;
		public String DataStack;
		public Int32 PC;
		public Boolean Flag;
		public DynamicObject Arg;
		public DynamicObject Result;
	}

	public class Instruction
	{
		public OpCode Code;
		public String Argument;
		public Int32 Address;

		public Instruction(OpCode code, Int32 addr)
		{
			Code = code;
			Address = addr;
		}

		public Instruction(OpCode code, String arg, Int32 address = 0)
		{
			Code = code;
			Argument = arg;
			Address = address;
		}

		public String Dump(Int32 addr)
		{
			var args = Address != 0 ? Address.ToString() : Argument;
			return $"{addr}: {Code} [{args}]";
		}
	}

	public class ObjectCode
	{
		private List<Instruction> _instructions = new List<Instruction>();

		private readonly Stack<Int32> _stack = new Stack<Int32>();
		private readonly Stack<String> _dataStack = new Stack<String>();
		private readonly Script _script = new Script();
		private readonly DynamicObject _result = new DynamicObject();
		private String _bookmark;

		Int32 _pc = 0;
		Boolean _flag;

		public Int32 IP => _pc - 1;
		public Boolean Flag => _flag;
		public Int32 CP => _instructions.Count;

		public Int32 Add(Instruction inst)
		{
			_instructions.Add(inst);
			return _instructions.Count - 1;
		}

		public Instruction Read()
		{
			return _instructions[_pc++];
		}

		public Int32 FindActivity(String label)
		{
			return _instructions.FindIndex(x => x.Code == OpCode.Start && x.Argument == label);
		}

		public void Goto(Int32 addr)
		{
			_pc = addr;
		}

		public void SetCondition(Boolean cond)
		{
			_flag = cond;
		}

		public StoredCode Store()
		{
			var sc = new StoredCode();
			sc.Stack = _stack.Serialize();
			sc.DataStack = _dataStack.Serialize();
			sc.PC = _pc;
			sc.Flag = _flag;

			sc.Arg = _script.GetVariable("Arg");
			sc.Result  = _script.GetVariable("Result");

			return sc;
		}

		public void Restore(StoredCode storedCode)
		{
			_stack.Deserialize(storedCode.Stack);
			_dataStack.Deserialize(storedCode.DataStack);
			_pc = storedCode.PC;
			_flag = storedCode.Flag;
			_script.AddVariable("Arg", storedCode.Arg);
			_script.AddVariable("Result", storedCode.Result);
		}


		public void SetAddress(Int32 instrAddress, Int32 value)
		{
			_instructions[instrAddress].Address = value;
		}

		public DynamicObject Result => _script.GetVariable("Result");

		public CodeState Run(DynamicObject arg)
		{
			var result = new DynamicObject();
			_script.AddVariable("Arg", arg);
			_script.AddVariable("Result", result);
			return Continue();
		}

		CodeState Continue()
		{ 
			var inst = Read();
			while (inst != null)
			{
				if (inst.Code == OpCode.Stop)
				{
					return CodeState.Complete;
				} 
				Exec(inst);
				if (inst.Code == OpCode.Wait)
					return CodeState.Wait;
				inst = Read();
			}
			return CodeState.Running;
		}

		public CodeState Resume(String bookmark, DynamicObject reply)
		{
			_script.AddVariable("Reply", reply);
			return Continue();
		}

		CodeState Exec(Instruction inst)
		{
			switch (inst.Code)
			{
				case OpCode.Start:
				case OpCode.Nop:
					break;
				case OpCode.Push:
					_stack.Push(inst.Address);
					Console.WriteLine($"\t\tPush: {inst.Address}, SP={_stack.SP}");
					break;
				case OpCode.Alloc:
					_dataStack.Alloc();
					break;
				case OpCode.Dealloc:
					_dataStack.Dealloc();
					break;
				case OpCode.Store:
					Console.WriteLine($"\t\tstored: {inst.Argument}");
					_dataStack.Store(inst.Argument);
					break;
				case OpCode.Invoke:
					Invoke(inst.Argument);
					break;
				case OpCode.Goto:
					Goto(inst.Address);
					break;
				case OpCode.Ret:
					Int32 addr = _stack.Pop();
					Console.WriteLine($"\t\tRet to {addr}, SP = {_stack.SP}");
					Goto(addr);
					break;
				case OpCode.Script:
					Console.WriteLine($"\t\tScript {inst.Argument}");
					_script.Execute(inst.Argument);
					break;
				case OpCode.Condition:
					SetCondition(Condition(inst.Argument));
					break;
				case OpCode.Equal:
					{
						var data = _dataStack.Load().ToString();
						Console.WriteLine($"\t\tCompare '{data}' with '{inst.Argument}'");
						_flag = data == inst.Argument;
					}
					break;
				case OpCode.BrFalse:
					if (!Flag)
						Goto(inst.Address);
					break;
				case OpCode.BrTrue:
					if (Flag)
						Goto(inst.Address);
					break;
				case OpCode.Switch:
					Switch(inst);
					break;
				case OpCode.Wait:
					_bookmark = inst.Argument;
					return CodeState.Wait;
				default:
					throw new InvalidProgramException($"Unsupported opcode: '{inst.Code}'");
			}
			return CodeState.Running;
		}

		void Invoke(String name)
		{
			var addr = FindActivity(name);
			_stack.Push(IP + 1);
			Console.WriteLine($"\t\tPush: {IP + 1}, SP={_stack.SP}");
			Goto(addr);
		}

		Boolean Condition(String expression)
		{
			return (expression != null) ? _script.Eval<Boolean>(expression) : false;
		}

		void Switch(Instruction instr)
		{
			var dataAddr = instr.Address;
			var argText = _dataStack.Load().ToString();
			for (var i = dataAddr; i<_instructions.Count; i++)
			{
				var curInstr = _instructions[i];
				if (curInstr.Argument == argText)
				{
					// found
					Console.WriteLine($"\t\tSwitch: Label={argText}, goto: {curInstr.Address}");
					Goto(curInstr.Address);
					return;
				}
				if (curInstr.Address == -1)
				{
					// branch not found
					break;
				}
			}
		}


		public String Dump()
		{
			var sb = new StringBuilder();
			for (Int32 i=0; i<_instructions.Count; i++)
			{
				var inst = _instructions[i];
				sb.Append($"{inst.Dump(i)}\n");
			}
			sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}
	}
}
