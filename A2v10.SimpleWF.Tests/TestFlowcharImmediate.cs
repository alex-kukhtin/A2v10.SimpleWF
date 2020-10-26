
using Microsoft.VisualStudio.TestTools.UnitTesting;

using A2v10.SimpleWF;
using System;

namespace A2v10.SimpleWF.Tests
{
	[TestClass]
	[TestCategory("Immediate.Flowchart")]
	public class FlowchartImmediateTest
	{
		[TestMethod]
		public void Assign()
		{
			var f = new Flowchart() { Ref = "Ref0" };
			f.Steps.Add(new Start() { Ref = "Ref1", Next = "Ref2" });
			f.Steps.Add(new Assign() { Ref= "Ref2", To = "Result.res", Value = "Arg.x + Arg.y" });

			var wf = new Workflow() { Root = f };

			var arg = new DynamicObject();
			arg.Set("x", 3);
			arg.Set("y", 2);

			wf.RunImmediate(arg);

			var res = wf.ResultImmediate;
			Assert.AreEqual(5, res.Eval<Int32>("res"));
		}

		[TestMethod]
		public void Decision()
		{
			var f = new Flowchart();
			f.Steps.Add(new Start() { Ref = "Ref0", Next = "Ref1" });
			f.Steps.Add(new Decision() {Ref = "Ref1", Condition = "Arg.x > 0", Then = "Ref2", Else = "Ref3" });
			f.Steps.Add(new Assign() { Ref = "Ref2", Next = "Ref1", To = "Arg.x", Value = "Arg.x - 1" });
			f.Steps.Add(new Assign() { Ref = "Ref3", To = "Result.res", Value = "Arg.x" });

			var wf = new Workflow() { Root = f };

			var arg = new DynamicObject();
			arg.Set("x", 10);
			wf.RunImmediate(arg);
			var res = wf.ResultImmediate;

			Assert.AreEqual(0, res.Eval<Int32>("x"));
		}
	}
}
