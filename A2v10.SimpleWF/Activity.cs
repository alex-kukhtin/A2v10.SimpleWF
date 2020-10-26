
using System;
using System.Linq.Expressions;

namespace A2v10.SimpleWF
{
	public class Activity
	{
		public Activity Parent { get; protected set; }
		public String Ref { get; set; }
		public String Next { get; set; }

		public virtual Boolean IsStart => false;

		public virtual void Compile(Compiler compiler)
		{
			compiler.StartActivity(this);
			compiler.EndActivity(this);
		}

		public virtual Activity FindActivity(String reference)
		{
			return null;
		}

		public virtual ExecState ExecuteImmediate(ExecuteContext context)
		{
			return ExecuteNext(context);
		}

		public virtual ExecState Continue(ExecuteContext context)
		{
			return ExecState.Complete;
		}

		protected ExecState ExecuteNext(ExecuteContext context)
		{
			if (Next != null)
				return Parent.FindActivity(Next).ExecuteImmediate(context);
			return ExecState.Complete;
		}

		public virtual DynamicObject Store()
		{
			return null;
		}

		public virtual void Restore(DynamicObject state)
		{
		}


		public virtual void OnInit(Activity parent)
		{
			Parent = parent;
		}
	}
}
