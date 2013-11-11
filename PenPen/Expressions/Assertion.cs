namespace PenPen.Expressions
{
	class Assertion : BaseExpression
	{
		internal bool IsPositive { get; private set; }
		internal BaseExpression Expression { get; private set; }

		internal override string Type
		{
			get { return Expression.Type; }
		}

		internal Assertion(bool isPositive, BaseExpression expression)
		{
			IsPositive = isPositive;
			Expression = expression;
		}

		internal override string Generate(int nest)
		{
			var template = nest > 0
				? Symbol.Indent(nest) + "({0}, {1}) => {2}Assert({0}, {1},{3})"
				: "PositiveAssert({0}, {1},{3})";
			var expression = Expression.Generate(nest + 1);
			var prefix = IsPositive ? "Positive" : "Negative";
			return string.Format(template, Symbol.State(nest), Symbol.Result(nest), prefix, expression);
		}

		public override string ToString()
		{
			return string.Format("{0}{1}", IsPositive ? "&" : "!", Expression);
		}
	}
}
