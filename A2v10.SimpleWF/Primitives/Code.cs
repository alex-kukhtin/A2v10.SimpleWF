
using System;
using System.Net;

namespace A2v10.SimpleWF
{
	public class Code : Activity
	{
		public String Script { get; set; }

		public override ExecState ExecuteImmediate(ExecuteContext context)
		{
			if (context.IsContinue)
				return ExecState.Complete;
			context.Execute(Script);
			return ExecuteNext(context);
		}
	}
}
