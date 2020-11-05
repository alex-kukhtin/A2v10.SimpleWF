
using System;
using System.Dynamic;
using Jint.Runtime;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace A2v10.SimpleWF
{
	public class Workflow
	{

		public Activity Root { get; set; }
		public DynamicObject Storage { get; set; }

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
			var executeContext = new ExecuteContext(Root, Storage);
			executeContext.SetVariable("Arg", arg);
			executeContext.SetVariable("Result", new DynamicObject());
			var st = Root.ExecuteImmediate(executeContext);
			_result = executeContext.Result;
			if (Storage != null)
			{
				var storedArg = executeContext.GetVariable("Arg");
				var storedResult = executeContext.GetVariable("Result");
				Storage.Set("Arg", storedArg);
				Storage.Set("Result", storedResult);
			}
			return st;
		}

		public ExecState Continue(String bookmark, DynamicObject reply)
		{
			Root.OnInit(null);
			var executeContext = new ExecuteContext(Root, Storage);
			executeContext.Reply = reply;
			executeContext.Bookmark = bookmark;
			executeContext.IsContinue = true;

			DynamicObject arg = Storage.Get<ExpandoObject>("Arg");
			DynamicObject res = Storage.Get<ExpandoObject>("Result");
			executeContext.SetVariable("Arg", arg);
			executeContext.SetVariable("Result", res);

			var st = Root.ExecuteImmediate(executeContext);
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
