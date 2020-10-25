
using System;

namespace A2v10.SimpleWF
{
	public class Activity
	{
		public String Ref { get; set; }
		public String Next { get; set; }

		public virtual Boolean IsStart => false;

		public virtual void Compile(Compiler compiler)
		{
			compiler.StartActivity(this);
			compiler.EndActivity(this);
		}
	}
}
