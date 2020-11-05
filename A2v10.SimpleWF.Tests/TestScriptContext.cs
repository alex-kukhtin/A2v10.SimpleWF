using Jint;
using Jint.Native;
using Jint.Native.Global;
using Jint.Runtime.Environments;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.SimpleWF.Tests
{
	[TestClass]
	[TestCategory("ScriptContext")]
	public class TestScriptContext
	{
		[TestMethod]
		public void InnerContext()
		{
			var eng = new Engine();

			/*
			var go = GlobalObject.CreateGlobalObject(eng);
			var ge = LexicalEnvironment.NewObjectEnvironment(eng, go, eng.GlobalEnvironment, false);
			var le = LexicalEnvironment.NewDeclarativeEnvironment(eng, eng.GlobalEnvironment); //, go, go, false);
			*/


			var ec = eng.EnterExecutionContext(null, null, JsValue.Undefined);

			eng.SetValue("x", 5);
			eng.SetValue("y", 3);

			Assert.AreEqual("5", eng.GetValue("x").ToString());
			Assert.AreEqual("3", eng.GetValue("y").ToString());

			eng.LeaveExecutionContext();

			eng.Execute("let z;");
			eng.SetValue("z", 55);
			Assert.AreEqual("5", eng.GetValue("x").ToString());
			Assert.AreEqual("3", eng.GetValue("y").ToString());
			Assert.AreEqual("55", eng.GetValue("z").ToString());

			//Assert.AreEqual("", eng.GetValue("z").ToString());
		}
	}
}
