
using System;
using System.Net;

namespace A2v10.SimpleWF
{
	public class Bookmark : Activity
	{
		public String Name { get; set; }

		public override void Compile(Compiler compiler)
		{
			compiler.StartActivity(this);
			compiler.Emit(OpCode.Wait, Name);
			compiler.EndActivity(this);
		}
	}
}
