
using System;

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
			compiler.EmitInvoke(Entry);
			compiler.EndActivity(this);

			// compile children
			Entry?.Compile(compiler);
		}
	}
}
