using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.SimpleWF
{
	public class Compiler
	{
		private ObjectCode _code = new ObjectCode();

		public ObjectCode Compile(Activity root)
		{
			Emit(OpCode.Invoke, root.Ref);
			Emit(OpCode.Stop);
			root.Compile(this);
			return _code;
		}

		public Int32 CP => _code.CP;

		public Int32 EmitOffset(OpCode op, Int32 offset)
		{
			return _code.Add(new Instruction(op, _code.CP + offset));
		}

		public Int32 Emit(OpCode op, String name = null, Int32 address = 0)
		{
			return _code.Add(new Instruction(op, name, address));
		}

		public Int32 StartActivity(Activity activity)
		{
			return Emit(OpCode.Start, activity.Ref);
		}

		public Int32 EndActivity(Activity activity)
		{
			if (activity.Next != null)
				return Emit(OpCode.Invoke, activity.Next);
			return Emit(OpCode.Ret);
		}

		public void SetAddress(Int32 instrAddress, Int32 value)
		{
			_code.SetAddress(instrAddress, value);
		}

	}
}
