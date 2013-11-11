using System;
namespace PenPen.Expressions
{
	class SemanticAction
	{
		internal string Code { get; private set; }

		internal SemanticAction(string code)
		{
			Code = code;
		}

		public override string ToString()
		{
			return Code.Replace(Environment.NewLine, Environment.NewLine + "\t\t\t\t\t\t");
		}
	}
}
