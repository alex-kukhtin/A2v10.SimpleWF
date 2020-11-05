using Jint;
using Jint.Parser.Ast;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Dynamic;
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
		private readonly DynamicObject _storage;

		public Boolean IsContinue { get; set; }
		public String Bookmark { get; set; }
		public DynamicObject Reply { get; set; }

		public ExecuteContext(Activity root, DynamicObject storage)
		{
			_root = root;
			_storage = storage;
		}

		public void SetVariable(String name, DynamicObject val)
		{
			_script.AddVariable(name, val);
		}

		public DynamicObject GetVariable(String name)
		{
			return _script.GetVariable(name);
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

		public void Store(String refer, String varName, Object value)
		{
			if (_storage == null)
				return;
			if (!_storage.TryGetValue(refer, out Object val))
			{
				_storage.Set(refer, new ExpandoObject());
			}
			DynamicObject dobj = _storage.Get<ExpandoObject>(refer);
			dobj.Set(varName, value);
		}

		public T Restore<T>(String refer, String varName)
		{
			DynamicObject dobj = _storage.Get<ExpandoObject>(refer);
			return dobj.Get<T>(varName);
		}
	}
}
