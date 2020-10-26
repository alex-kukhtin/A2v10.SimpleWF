using Jint;
using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.SimpleWF
{ 
	public enum ExecState
	{
		Idle,
		Complete
	}

	public class ExecuteContext
	{
		private readonly Script _script = new Script();
		private readonly Activity _root;

		public ExecuteContext(Activity root)
		{
			_root = root; 
		}

		public void SetVariable(String name, DynamicObject val)
		{
			_script.AddVariable(name, val);
		}

		public DynamicObject Result => _script.GetVariable("Result");

		public void Execute(String statement)
		{
			_script.Execute(statement);
		}

		public Boolean Evaluate(String expression)
		{
			return (expression != null) ? _script.Eval<Boolean>(expression) : false;
		}
	}
}
