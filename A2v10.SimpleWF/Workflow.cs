
using System;
using Jint.Runtime;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace A2v10.SimpleWF
{
	public class Workflow
	{

		public Activity Root { get; set; }

		public ObjectCode Compile()
		{
			if (Root == null)
				throw new NullReferenceException(nameof(Root));

			var c = new Compiler();

			return c.Compile(Root);
		}


		public DynamicObject ResultImmediate => _result;

		private DynamicObject _result;

		public ExecState RunImmediate(DynamicObject arg)
		{
			Root.OnInit(null);
			var executeContext = new ExecuteContext(Root);
			executeContext.SetVariable("Arg", arg);
			executeContext.SetVariable("Result", new DynamicObject());
			var st = Root.ExecuteImmediate(executeContext);
			_result = executeContext.Result;
			return st;
		}

		public ExecState Continue(String bookmark, DynamicObject reply)
		{
			Root.OnInit(null);
			var executeContext = new ExecuteContext(Root);
			executeContext.SetVariable("Reply", reply);
			var st = Root.Continue(executeContext);
			_result = executeContext.Result;
			return st;
		}

		public DynamicObject Store()
		{
			return Root.Store();
		}

		public void Restore(DynamicObject state)
		{
			Root.Restore(state);
		}
	}
}
