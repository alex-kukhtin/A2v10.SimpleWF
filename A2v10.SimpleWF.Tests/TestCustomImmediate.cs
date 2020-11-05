using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using A2v10.SimpleWF.Mock;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Dynamic;

namespace A2v10.SimpleWF.Tests
{
	[TestClass]
	[TestCategory("CustomImmediate")]
	public class TestCustomImmediate
	{
		[TestMethod]
		public void CustomActivity()
		{
			var f = new Sequence() { Ref = "Ref0" };
			f.Steps.Add(new CallApiMock() { Ref = "Ref1", Url = "MockUrl", Method = "POST", Body = "MockBody" });

			var wf = new Workflow() { Root = f };
			var arg = new DynamicObject();
			wf.RunImmediate(arg);

			var res = wf.ResultImmediate;
			Assert.AreEqual("MockUrl", res.Eval<String>("res.Url"));
			Assert.AreEqual("POST", res.Eval<String>("res.Method"));
			Assert.AreEqual("MockBody", res.Eval<String>("res.Body"));
		}

		[TestMethod]
		public void WaitableCustomActivity()
		{
			var f = new Sequence() { Ref = "Ref0" };
			f.Steps.Add(new WaitableCallApiMock() { Ref = "Ref1", Url = "MockUrl", Method = "POST", Body = "MockBody", Bookmark="Mark1" });
			f.Steps.Add(new Code() { Ref = "Ref2", Script = "Result.res = Reply.apiResult;" });

			var st = new DynamicObject();

			var wf = new Workflow() { Root = f, Storage = st };

			var objCode = wf.Compile();

			var arg = new DynamicObject();

			var state = wf.RunImmediate(arg);
			Assert.AreEqual(ExecState.Idle, state);


			var stJson = JsonConvert.SerializeObject(st.Root);


			//var stored = wf.Store();
			//var str = JsonConvert.SerializeObject(stored);
			//var restored = JsonConvert.DeserializeObject<ExpandoObject>(str);
			//wf.Restore(restored);

			int z = 55;

			var reply = new DynamicObject();
			reply.Set("apiResult", "API Result");
			wf.Continue("Mark1", reply);


			var res = wf.ResultImmediate;

			var resJson = JsonConvert.SerializeObject(res.Root);

			Assert.AreEqual("MockUrl", res.Eval<String>("args.Url"));
			Assert.AreEqual("POST", res.Eval<String>("args.Method"));
			Assert.AreEqual("MockBody", res.Eval<String>("args.Body"));
			Assert.AreEqual("API Result", res.Eval<String>("res"));
		}
	}
}
