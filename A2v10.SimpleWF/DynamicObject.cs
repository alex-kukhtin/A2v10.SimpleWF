
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace A2v10.SimpleWF
{
	public class DynamicObject
	{
#pragma warning disable IDE1006 // Naming Styles
		private ExpandoObject _object;
		private IDictionary<String, Object> _dictionary => _object;
#pragma warning restore IDE1006 // Naming Styles

		public ExpandoObject Root => _object;

		public static implicit operator DynamicObject(ExpandoObject dobj)
		{
			return new DynamicObject(dobj);
		}

		public static implicit operator ExpandoObject(DynamicObject dobj)
		{
			return dobj.Root;
		}

		public DynamicObject()
		{
			_object = new ExpandoObject();
		}

		public DynamicObject(ExpandoObject expando)
		{
			_object = expando ?? new ExpandoObject();
		}

		public void Set(String name, Object val)
		{
			if (val is DynamicObject dobj)
				val = dobj.Root;
			if (_dictionary.ContainsKey(name))
				_dictionary[name] = val;
			else
				_dictionary.Add(name, val);
		}

		static readonly Regex _arrFind = new Regex(@"(\w+)\[(\d+)\]{1}", RegexOptions.Compiled);

		Object EvalExpression(String expression, Boolean throwIfError = false)
		{
			Object currentContext = _object;
			foreach (var exp in expression.Split('.'))
			{
				if (currentContext == null)
					return null;
				String prop = exp.Trim();
				var d = currentContext as IDictionary<String, Object>;
				if (prop.Contains("["))
				{
					var match = _arrFind.Match(prop);
					prop = match.Groups[1].Value;
					if ((d != null) && d.ContainsKey(prop))
					{
						if (d[prop] is IList<ExpandoObject> listExp)
							currentContext = listExp[Int32.Parse(match.Groups[2].Value)];
						else if (d[prop] is Object[] arrObj)
							currentContext = arrObj[Int32.Parse(match.Groups[2].Value)];
					}
					else
					{
						if (throwIfError)
							throw new ArgumentException($"Error in expression '{expression}'. Property '{prop}' not found");
						return null;
					}
				}
				else
				{
					if ((d != null) && d.ContainsKey(prop))
						currentContext = d[prop];
					else
					{
						if (throwIfError)
							throw new ArgumentException($"Error in expression '{expression}'. Property '{prop}' not found");
						return null;
					}
				}
			}
			return currentContext;
		}

		public T Eval<T>(String expression, T fallback = default, Boolean throwIfError = false)
		{
			if (expression == null)
				return fallback;
			var result = EvalExpression(expression, throwIfError);
			if (result == null)
				return fallback;
			return (T)Convert.ChangeType(result, typeof(T));
		}
	}
}
