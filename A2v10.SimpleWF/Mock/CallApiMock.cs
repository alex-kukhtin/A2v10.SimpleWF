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
	}


	public class WaitableCallApiMock : CustomActivity
	{

		public String Url { get; set; }
		public String Method { get; set; }

		public String Body { get; set; }

		public override bool IsWaitable => true;

		public override void Execute(Script script)
		{
			script.Execute($"Result.val = {{Url:'{Url}', Method: '{Method}', Body:'{Body}' }}");
		}

		public override void Continue(Script script)
		{
			script.Execute($"Result.val.ApiResult = Reply.apiResult; Result.res = Result.val");
		}
	}
}
