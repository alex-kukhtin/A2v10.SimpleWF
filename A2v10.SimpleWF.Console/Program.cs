using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace A2v10.SimpleWF.Test
{


	public abstract class Compiler<T>
	{

		public abstract void Compile();
	}

	public class AssignCompiler : Compiler<Assign>
	{
		public override void Compile()
		{
			Console.WriteLine("Compile Assign");
		}
	}

	public class SequenceCompiler : Compiler<Sequence>
	{
		public override void Compile()
		{
			Console.WriteLine("Compile Sequecne");
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			var services = new ServiceCollection();
			ConfigreServices(services);

			using var provider = services.BuildServiceProvider();

			var log = provider.GetService<ILogger<Workflow>>();

			List<Activity> acts = new List<Activity>();
			acts.Add(new Assign());
			acts.Add(new Sequence());

			/*
			var f = new Flow() { Ref = "Flow" };
			f.Steps.Add(new Start() { Ref = "Ref0", Next = "Ref1" });
			f.Steps.Add(new Decision() { Ref = "Ref1", Condition = "Arg.x > 0", Then = "Ref2", Else = "Ref3" });
			f.Steps.Add(new Assign() { Ref = "Ref2", Next = "Ref1", To = "Arg.x", Value = "Arg.x - 1" });
			f.Steps.Add(new Assign() { Ref = "Ref3", To = "Result.res", Value = "Arg.x" });

			var wf = new Workflow() { Root = f };

			var objCode = wf.Compile();
			Console.WriteLine(objCode.Dump());

			var prog = new RunContext(objCode);

			var arg = new DynamicObject();
			arg.Set("x", 10);
			var res = prog.Run(arg);
			*/

			// register activity runs
			var f = new StateMachine() { Ref = "Ref0" };
			f.States.Add(new StartState() { Ref = "Ref1", Name = "Start", NextState = "State1" });

			var st = new State() { Ref = "Ref2", Name = "State1", NextState = "Final" };
			st.Entry = new Assign() { Ref = "Entry1", To = "Arg.x", Value = "Arg.x + 1" };
			st.Exit = new Assign() { Ref = "Exit1", To = "Result.x", Value = "Arg.x" };
			f.States.Add(st);

			f.States.Add(new FinalState() { Ref = "Ref3", Name = "Final" });

			var wf = new Workflow() { Root = f };

			//var objCode = wf.Compile();

			var arg = new DynamicObject();
			arg.Set("x", 5);
			// var res = objCode.Run(arg);
		}

		static void ConfigreServices(IServiceCollection services)
		{
			services.AddLogging(configure => configure.AddConsole())
				.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Trace)
				.AddSingleton<Workflow>();
		}

	}
}
