namespace PenPen.Expressions
{
	class Literal : BaseExpression
	{
		readonly bool isChar;

		internal string Value { get; private set; }

		internal override string Type
		{
			get { return isChar ? "char" : "string"; }
		}

		internal Literal(string value, bool isChar)
		{
			Value = value;
			this.isChar = isChar;
		}

		internal override string Generate(int nest)
		{
			var template = nest > 0
				? Symbol.Indent(nest) + "({0}, {1}) => Literal({0}, {1}, \"{2}\", {3})"
				: "Literal({0}, {1}, \"{2}\", {3})";
			return string.Format(template, Symbol.State(nest), Symbol.Result(nest), Type, this);
		}

		public override string ToString()
		{
			if (isChar)
				return string.Format("\"{0}\"", Value.Replace("\"", "\\\""));
			return string.Format("\"{0}\"", Value);
		}
	}
}
