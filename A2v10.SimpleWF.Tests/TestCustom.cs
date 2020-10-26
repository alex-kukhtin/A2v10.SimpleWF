using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using A2v10.SimpleWF.Mock;
using System.Collections.Generic;
using System.Text;

namespace A2v10.SimpleWF.Tests
{
	[TestClass]
	[TestCategory("Custom")]
	public class TestCustom
	{
		[TestMethod]
		public void CustomActivity()
		{
			var f = new Sequence() { Ref = "Ref0" };
			f.Steps.Add(new CallApiMock() { Ref = "Ref1", Url = "MockUrl", Method = "POST", Body="MockBody" });

			var wf = new Workflow() { Root = f };

			var objCode = wf.Compile();

			var arg = new DynamicObject();

			objCode.Run(arg);
			var res = objCode.Result;
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

			objCode.Run(arg);

			var reply = new DynamicObject();
			reply.Set("apiResult", "API Result");
			objCode.Resume("", reply);

			var res = objCode.Result;
			Assert.AreEqual("MockUrl", res.Eval<String>("res.Url"));
			Assert.AreEqual("POST", res.Eval<String>("res.Method"));
			Assert.AreEqual("MockBody", res.Eval<String>("res.Body"));
			Assert.AreEqual("API Result", res.Eval<String>("res.ApiResult"));
		}
	}
}
