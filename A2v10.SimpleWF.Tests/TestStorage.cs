using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.SimpleWF.Tests
{
	[TestClass]
	[TestCategory("Storage")]
	public class TestStorage
	{

		[TestMethod]
		public void SaveRestore()
		{
			var f = new StateMachine() { Ref = "Ref0" };
			f.States.Add(new StartState() { Ref = "Ref1", Name = "Start", NextState = "State1" });

			var st = new State() { Ref = "Ref2", Name = "State1", NextState = "Final" };
			st.Entry = new Bookmark() { Ref = "Ref21", Name = "Bookmark" };
			st.Exit = new Assign() { Ref = "Ref22", To = "Arg.x", Value = "Arg.x + Arg.x" };
			f.States.Add(st);

			var fs = new FinalState() { Ref = "Ref3", Name = "Final" };
			fs.Entry = new Assign() { Ref = "Ref4", To = "Result.res", Value = "Arg.x" };
			f.States.Add(fs);

			var wf = new Workflow() { Root = f };

			var objCode = wf.Compile();

			var arg = new DynamicObject();
			arg.Set("x", 5);

			var state= objCode.Run(arg);
			Assert.AreEqual(CodeState.Wait, state);

			var res = objCode.Result;
			Assert.AreEqual(0, res.Eval<Int32>("res"));


			var stored = objCode.Store();
			var wf2 = new Workflow() { Root = f };
			var objCode2 = wf.Compile();

			objCode2.Restore(stored);

			objCode2.Resume("Bookmark", new DynamicObject());
			res = objCode2.Result;
			Assert.AreEqual(10, res.Eval<Int32>("res"));
		}
	}
}
