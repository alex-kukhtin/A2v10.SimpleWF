using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.SimpleWF
{
	public class FinalState : StateBase
	{
		public override Boolean IsFinal => true;

		public Activity Entry { get; set; }

		public override void Compile(Compiler compiler)
		{
			// executing
			compiler.StartActivity(this);
			if (Entry != null)
				compiler.Emit(OpCode.Invoke, Entry.Ref);
			compiler.EndActivity(this);

			// compile children
			if (Entry != null)
				Entry.Compile(compiler);
		}
	}
}
