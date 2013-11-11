namespace PenPen.Expressions
{
	class Labeled : BaseExpression
	{
		internal string Label { get; private set; }
		internal BaseExpression Expression { get; private set; }

		internal override string Type
		{
			get { return Expression.Type; }
		}

		internal Labeled(string label, BaseExpression expression)
		{
			Label = label;
			Expression = expression;
		}

		internal override string Generate(int nest)
		{
			if (nest > 1)
				throw new System.Exception("validated");

			var template = nest > 0
				? Symbol.Indent(nest) + "({0}, {1}) => Label({0}, {1}, \"{2}\",{3})"
				: "Label({0}, {1}, \"{2}\",{3})";
			var expression = Expression.Generate(nest + 1);
			return string.Format(template, Symbol.State(nest), Symbol.Result(nest), Label, expression);
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", Label, Expression);
		}
	}
}
