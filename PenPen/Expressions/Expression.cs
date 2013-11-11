namespace PenPen.Expressions
{
	class Expression : BaseExpression
	{
		internal BaseExpression Value { get; private set; }

		internal override string Type
		{
			get { return Value.Type; }
		}

		internal Expression(BaseExpression value)
		{
			Value = value;
		}

		internal override string Generate(int nest)
		{
			return Value.Generate(nest);
		}

		public override string ToString()
		{
			return string.Format("({0})", Value);
		}
	}
}
