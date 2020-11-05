using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.SimpleWF.Mock
{
	public class CallApiMock : CustomActivity
	{

		public String Url { get; set; }
		public String Method { get; set; }

		public String Body { get; set; }

		public override void Execute(Script script)
		{
			script.Execute($"Result.res = {{Url:'{Url}', Method: '{Method}', Body:'{Body}' }}");
		}

		public override ExecState ExecuteImmediate(ExecuteContext context)
		{
			context.Execute($"Result.res = {{Url:'{Url}', Method: '{Method}', Body:'{Body}' }}");
			return ExecuteNext(context);

		}
	}


	public class WaitableCallApiMock : CustomActivity
	{

		public String Url { get; set; }
		public String Method { get; set; }

		public String Body { get; set; }

		public String Bookmark { get; set; }

		public override bool IsWaitable => true;


		public override void Execute(Script script)
		{
			script.Execute($"Result.val = {{Url:'{Url}', Method: '{Method}', Body:'{Body}' }}");
		}

		public override void Continue(Script script)
		{
			throw new NotImplementedException();
			//script.Execute($"Result.val.ApiResult = Reply.apiResult; Result.res = Result.val");
		}

		public override ExecState ExecuteImmediate(ExecuteContext context)
		{
			if (context.IsContinue)
			{
				var storedBookmark = context.Restore<String>(Ref, "Bookmark");
				if (storedBookmark == context.Bookmark)
				{
					context.SetVariable("Reply", context.Reply);
					context.IsContinue = false;
					return ExecState.Complete;
				}
			}
			context.Execute($"Result.args = {{Url:'{Url}', Method: '{Method}', Body:'{Body}' }}");
			context.Store(Ref, "Bookmark", Bookmark);
			return ExecState.Idle;
		}
	}
}
