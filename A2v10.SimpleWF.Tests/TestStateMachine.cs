using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.SimpleWF.Tests
{
	[TestClass]
	[TestCategory("StateMachine")]
	public class TestStateMachine
	{
		[TestMethod]
		public void Simple()
		{
			var f = new StateMachine() { Ref = "Ref0" };
			f.States.Add(new StartState() { Ref = "Ref1", Name="Start", NextState="Final" });
			f.States.Add(new FinalState() { Ref = "Ref2", Name="Final" });

			var wf = new Workflow() { Root = f };

			var objCode = wf.Compile();

			var arg = new DynamicObject();
			arg.Set("x", 3);
			arg.Set("y", 2);
			var res = objCode.Run(arg);
		}

		[TestMethod]
		public void WithEntry()
		{
			var f = new StateMachine() { Ref = "Ref0" };
			f.States.Add(new StartState() { Ref = "Ref1", Name = "Start", NextState = "State1" });
			
			var st = new State() { Ref = "Ref2", Name = "State1", NextState = "Final" };
			st.Entry = new Assign() { Ref = "Entry1", To = "Arg.x", Value = "Arg.x + 1" };
			st.Exit = new Assign() { Ref = "Exit1", To = "Result.x", Value = "Arg.x" };
			f.States.Add(st);

			f.States.Add(new FinalState() { Ref = "Ref3", Name = "Final" });

			var wf = new Workflow() { Root = f };

			var objCode = wf.Compile();

			var arg = new DynamicObject();
			arg.Set("x", 5);
			objCode.Run(arg);
			var res = objCode.Result;
			Assert.AreEqual(6, res.Eval<Int32>("x"));
		}

		[TestMethod]
		public void FinalEntry()
		{
			var f = new StateMachine() { Ref = "Ref0" };
			f.States.Add(new StartState() { Ref = "Ref1", Name = "Start", NextState = "State1" });

			var st = new State() { Ref = "Ref2", Name = "State1", NextState = "Final" };
			f.States.Add(st);

			var fs = new FinalState() { Ref = "Ref3", Name = "Final" };
			fs.Entry = new Assign() { Ref = "Ref4", To = "Result.res", Value = "5" };
			f.States.Add(fs);

			var wf = new Workflow() { Root = f };

			var objCode = wf.Compile();

			var arg = new DynamicObject();
			arg.Set("x", 5);
			arg.Set("t", 10);
			objCode.Run(arg);
			var res = objCode.Result;
			Assert.AreEqual(5, res.Eval<Int32>("res"));
		}

		[TestMethod]
		public void Transitions()
		{
			var f = new StateMachine() { Ref = "Ref0" };
			f.States.Add(new StartState() { Ref = "Ref1", Name = "Start", NextState = "State1" });

			var st = new State() { Ref = "Ref2", Name = "State1", NextState = "Final" };
			var t1 = new Transition()
			{
				Condition = "Arg.t > Arg.x", Action = new Assign() { Ref = "Ref7", To = "Arg.t", Value = "Arg.t - 1" },
				Destination = "State1"

			};
			st.Transitions.Add(t1);
			f.States.Add(st);

			var fs = new FinalState() { Ref = "Ref3", Name = "Final" };
			fs.Entry = new Assign() { Ref = "Ref4", To = "Result.res", Value = "Arg.t" };
			f.States.Add(fs);

			var wf = new Workflow() { Root = f };

			var objCode = wf.Compile();

			var arg = new DynamicObject();
			arg.Set("x", 5);
			arg.Set("t", 12);
			objCode.Run(arg);
			var res = objCode.Result;
			Assert.AreEqual(5, res.Eval<Int32>("res"));
		}
	}
}
