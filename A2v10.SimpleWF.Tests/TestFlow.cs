using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.SimpleWF.Tests
{
	[TestClass]
	[TestCategory("Flow")]
	public class TestFlow
	{
		[TestMethod]
		public void Sequence()
		{
			var f = new Sequence() { Ref = "Ref0" };
			f.Steps.Add(new Assign() { Ref = "Ref1", To = "Arg.r", Value="Arg.x + Arg.y"});
			f.Steps.Add(new Assign() { Ref = "Ref2", To = "Result.res", Value = "Arg.r" });

			var wf = new Workflow() { Root = f };

			var objCode = wf.Compile();

			var arg = new DynamicObject();
			arg.Set("x", 3);
			arg.Set("y", 2);
			objCode.Run(arg);
			var res = objCode.Result;
			Assert.AreEqual(5, res.Eval<Int32>("res"));
		}
	}
}
