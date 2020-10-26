using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using Newtonsoft.Json;

namespace A2v10.SimpleWF
{
	public class CustomActivity : Activity
	{
		public override void Compile(Compiler compiler)
		{
			var type = GetType();
			var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var dobj = new DynamicObject();
			dobj.Set("$type", type.FullName);
			foreach (var p in props)
			{
				dobj.Set(p.Name, p.GetValue(this));
			}
			var ps = JsonConvert.SerializeObject(dobj.Root);
			compiler.StartActivity(this);
			compiler.Emit(OpCode.Execute, ps);
			if (IsWaitable)
			{
				compiler.Emit(OpCode.Wait);
				compiler.Emit(OpCode.Continue, ps);
			}
			compiler.EndActivity(this);
		}

		public virtual Boolean IsWaitable { get; }

		public virtual void Execute(Script script)
		{
		}

		public virtual void Continue(Script script)
		{

		}
	}
}
