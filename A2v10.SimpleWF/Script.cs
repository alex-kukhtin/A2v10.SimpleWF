using System;
using System.Dynamic;
using Jint;

namespace A2v10.SimpleWF
{
	public class Script
	{
		Engine _engine = new Engine(opts => {
			opts.Strict(true);
		});

		public void AddVariable(String name, DynamicObject arg)
		{
			_engine.SetValue(name, arg.Root);
		}

		public DynamicObject GetVariable(String name)
		{
			var obj = _engine.GetValue(name).ToObject();
			return new DynamicObject(obj as ExpandoObject);
		}

		public void Execute(String code)
		{
			_engine.Execute(code);
		}

		public T Eval<T>(String expression)
		{
			if (expression == null)
				return default;
			var val = _engine.Execute(expression).GetCompletionValue();
			var vo = val.ToObject();
			return (T)Convert.ChangeType(vo, typeof(T));
		}
	}
}
