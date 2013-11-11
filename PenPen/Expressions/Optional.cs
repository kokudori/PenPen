using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenPen.Expressions
{
	class Optional : BaseExpression
	{
		internal BaseExpression Expression { get; private set; }

		internal override string Type
		{
			get { return string.Format("Maybe<{0}>", Expression.Type); }
		}

		public Optional(BaseExpression expression)
		{
			Expression = expression;
		}

		internal override string Generate(int nest)
		{
			var template = nest > 0
				? Symbol.Indent(nest) + "({0}, {1}) => Optional<{2}>({0}, {1}, \"{2}\",{3})"
				: "Optional<{2}>({0}, {1}, \"{2}\",{3})";
			var expression = Expression.Generate(nest + 1);
			return string.Format(template, Symbol.State(nest), Symbol.Result(nest), Expression.Type, expression);
		}

		public override string ToString()
		{
			return Expression + "?";
		}
	}
}
