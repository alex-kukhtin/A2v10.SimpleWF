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
			f.Steps.Add(new CallApiMock() { Ref = "Ref1", Url = "MockUrl", Method = "POST", Body="MockBody" });

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
			f.Steps.Add(new WaitableCallApiMock() { Ref = "Ref1", Url = "MockUrl", Method = "POST", Body = "MockBody" });

			var wf = new Workflow() { Root = f };

			var objCode = wf.Compile();

			var arg = new DynamicObject();

			var state = wf.RunImmediate(arg);
			Assert.AreEqual(ExecState.Idle, state);

			var stored = wf.Store();
			var str = JsonConvert.SerializeObject(stored);
			var restored = JsonConvert.DeserializeObject<ExpandoObject>(str);
			wf.Restore(restored);

			int z = 55;

			/*
			var reply = new DynamicObject();
			reply.Set("apiResult", "API Result");

			wf.Continue("", reply);


			var res = objCode.Result;
			Assert.AreEqual("MockUrl", res.Eval<String>("res.Url"));
			Assert.AreEqual("POST", res.Eval<String>("res.Method"));
			Assert.AreEqual("MockBody", res.Eval<String>("res.Body"));
			Assert.AreEqual("API Result", res.Eval<String>("res.ApiResult"));
			*/
		}
	}
}
