
using System;
using Microsoft.Extensions.Logging;

namespace A2v10.SimpleWF
{
	public class Workflow
	{

		public Activity Root { get; set; }

		public ObjectCode Compile()
		{
			if (Root == null)
				throw new NullReferenceException(nameof(Root));

			var c = new Compiler();

			return c.Compile(Root);
		}
	}
}
