namespace PenPen.Expressions
{
	class Repetition : BaseExpression
	{
		internal BaseExpression Expression { get; private set; }
		internal int Min { get; private set; }
		internal int? Max { get; private set; }

		internal override string Type
		{
			get
			{
				if (Expression.Type == "char" || Expression.Type == "Char")
					return "string";
				return string.Format("IEnumerable<{0}>", Expression.Type);
			}
		}

		internal Repetition(BaseExpression expression, int min, int? max)
		{
			Expression = expression;
			Min = min;
			Max = max;
		}

		internal override string Generate(int nest)
		{
			var template = nest > 0
				? Symbol.Indent(nest) + "({0}, {1}) => Repetition<{5}>({0}, {1}, {2}, {3},{4})"
				: "Repetition<{5}>({0}, {1}, {2}, {3},{4})";
			var max = Max == null ? "null" : Max.ToString();
			var expression = Expression.Generate(nest + 1);
			return string.Format(template, Symbol.State(nest), Symbol.Result(nest), Min, max, expression, Expression.Type);
		}

		public override string ToString()
		{
			if (Min == 0 && Max == null)
				return Expression + "*";
			if (Min == 1 && Max == null)
				return Expression + "+";
			var max = Max == null ? "" : Max.ToString();
			return Expression + string.Format("{{{0}, {1}}}", Min, max);
		}
	}
}
