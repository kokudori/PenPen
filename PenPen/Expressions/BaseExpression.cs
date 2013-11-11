namespace PenPen.Expressions
{
	abstract class BaseExpression
	{
		internal virtual SemanticAction Action { get; set; }

		internal abstract string Type { get; }

		internal abstract string Generate(int nest);
	}
}
