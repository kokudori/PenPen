using System.Collections.Generic;
using System.Linq;

namespace PenPen.Expressions
{
	class Choice : BaseExpression
	{
		internal IEnumerable<BaseExpression> Expressions { get; private set; }

		internal override string Type
		{
			get
			{
				var types = Expressions.Select(x => x.Type);
				if (types.All(x => x == types.First()))
					return types.First();
				if (types.All(x => x == "string" || x == "char"))
					return "string";
				throw new System.Exception("valid");
			}
		}

		bool IsImplicit()
		{
			var types = Expressions.Select(x => x.Type);
			var all = types.All(x => x == types.First());
			var any = types.All(x => x == "string" || x == "char");
			return !all && any;
		}

		internal Choice(IEnumerable<BaseExpression> expressions)
		{
			Expressions = expressions;
		}

		internal override string Generate(int nest)
		{
			var template = nest > 0
				? Symbol.Indent(nest) + "({0}, {1}) => Choice({0}, {1}, {2},{3})"
				: "Choice({0}, {1} , {2},{3})";
			var isImplicit = IsImplicit() ? "true" : "false";
			var expressions = string.Join(",", Expressions.Select(x => x.Generate(nest + 1)));
			return string.Format(template, Symbol.State(nest), Symbol.Result(nest), isImplicit, expressions);
		}

		public override string ToString()
		{
			return string.Join(" / ", Expressions);
		}
	}
}
