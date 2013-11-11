using System.Collections.Generic;
using System.Linq;

namespace PenPen.Expressions
{
	class Sequence : BaseExpression
	{
		internal IEnumerable<BaseExpression> Expressions { get; private set; }

		internal override string Type
		{
			get
			{
				var types = Expressions.Where(x => !(x is Assertion)).Select(x => x.Type);
				return "Tuple<" + string.Join(", ", types) + ">";
			}
		}

		internal Sequence(IEnumerable<BaseExpression> expressions)
		{
			Expressions = expressions;
		}

		internal override string Generate(int nest)
		{
			var template = nest > 0
				? Symbol.Indent(nest) + "({0}, {1}) => Sequence<{4}>({0}, {1}, {2},{3})"
				: "Sequence<{4}>({0}, {1}, {2},{3})";
			var expressions = string.Join(",", Expressions.Select(x => x.Generate(nest + 1)));
			var types = string.Join(", ", Expressions.Where(x => !(x is Assertion)).Select(x => x.Type));
			var inner = nest > 0 ? "true" : "false";
			return string.Format(template, Symbol.State(nest), Symbol.Result(nest), inner, expressions, types);
		}

		public override string ToString()
		{
			return string.Join(" ", Expressions);
		}
	}
}
