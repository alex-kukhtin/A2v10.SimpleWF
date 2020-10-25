using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.SimpleWF
{
	public class Start : Activity
	{
		public override Boolean IsStart => true;

		public override void Compile(Compiler compiler)
		{
			compiler.StartActivity(this);
			compiler.EndActivity(this);
		}
	}
}
