using System.Linq;

namespace PenPen.Expressions
{
	class Rule : BaseExpression
	{
		internal string Name { get; private set; }

		internal override string Type
		{
			get { return Grammar.Symbols.Where(x => x.Name == Name).First().Type; }
		}

		internal Rule(string name)
		{
			Name = name;
		}

		internal override string Generate(int nest)
		{
			var template = nest > 0
				? Symbol.Indent(nest) + "({0}, {1}) => Rule({0}, {1}, \"{2}\")"
				: "Rule({0}, {1}, \"{2}\")";
			return string.Format(template, Symbol.State(nest), Symbol.Result(nest), Name);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
