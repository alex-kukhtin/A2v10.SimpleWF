
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace A2v10.SimpleWF
{
	public class Stack<T>
	{
		private List<T> _stack = new List<T>();

		public Int32 SP => _sp;
		private Int32 _sp = 0;

		public Int32 Push(T val)
		{
			if (_sp >= _stack.Count)
				_stack.Add(val);
			else
				_stack[_sp] = val;
			_sp += 1;
			return _sp -1;
		}

		public T Pop()
		{
			return _stack[--_sp];
		}

		public void Alloc()
		{
			if (_sp >= _stack.Count)
				_stack.Add(default);
			_sp += 1;
		}

		public void Dealloc()
		{
			_sp -= 1;
		}

		public void Store(T val)
		{
			_stack[_sp - 1] = val;
		}

		public Object Load()
		{
			return _stack[_sp - 1];
		}

		public void Shrink()
		{
			if (_sp < _stack.Count)
				_stack.RemoveRange(_sp, _sp - _stack.Count);
		}

		public String Serialize()
		{
			Shrink();
			return JsonConvert.SerializeObject(_stack);
		}

		public void Deserialize(String code)
		{
			var st = JsonConvert.DeserializeObject<List<T>>(code);
			_stack = st;
			_sp = _stack.Count;
		}

	}
}
