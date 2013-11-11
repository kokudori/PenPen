using System;
using System.Collections.Generic;
using System.Linq;

namespace PenPen.Expressions
{
	class Symbol
	{
		internal string Name { get; private set; }
		internal string Type { get; private set; }
		internal string Display { get; private set; }
		internal BaseExpression Expression { get; private set; }

		internal static string Result(int nest)
		{
			if (nest == 0)
				return Grammar.prefix + "actionResults";
			return string.Join("", Enumerable.Repeat("r", nest));
		}

		internal static string State(int nest)
		{
			if (nest == 0)
				return Grammar.prefix + "state";
			return string.Join("", Enumerable.Repeat("s", nest));
		}

		internal static string Indent(int nest)
		{
			if (nest == 0)
				return "\r\n\t\t\t\t\t\t";
			return "\r\n\t\t\t\t\t\t" + string.Join("", Enumerable.Repeat("\t", nest));
		}

		internal Symbol(string name, string type, string display, BaseExpression expression)
		{
			Name = name;
			Type = type;
			Display = display;
			Expression = expression;
		}

		Labeled[] GetLabel(BaseExpression expression)
		{
			if (expression is Labeled)
				return new[] { expression as Labeled };
			if (expression is Sequence)
				return (expression as Sequence).Expressions
					.SelectMany(GetLabel).Where(x => x != null).ToArray();
			if (expression is Choice)
				return (expression as Choice).Expressions
					.SelectMany(GetLabel).Where(x => x != null).ToArray();
			return new Labeled[] { };
		}

		string GenerateAction()
		{
			if (Expression.Action == null)
				return string.Format("var {0}value = input.Substring({0}state.index, {0}result.index - {0}state.index);", Grammar.prefix);

			var template = @"Func<{0}> {1}action = ({2}) => {3};
							var {1}value = ({4}){1}action({5});";
			var labels = GetLabel(Expression);

			if (!labels.Any())
				return string.Format(template, Type, Grammar.prefix, "", Expression.Action, Type, "");

			var types = string.Join(", ", labels.Select(x => x.Type)) + ", " + Type;
			var parameters = string.Join(", ", labels.Select(x => x.Type + " " + x.Label));
			var args = string.Join(", ", labels.Select(x => Grammar.prefix + "actionResults.Single(x => x.label == \"" + x.Label + "\").value"));
			return string.Format(template, types, Grammar.prefix, parameters, Expression.Action, Type, args);
		}

		internal string Generate()
		{
			var template =
@"				// {0} = {1}
				{{ ""{0}"", ({2}state, {2}results) =>
					{{
						var {2}actionResults = new Stack<Result>();
						var {2}result = {3};
						if ({2}result.parsed)
						{{
							{4}
							{2}results.Push(new Result(null, ""{5}"", {2}value));
						}}
						return {2}result;
					}}
				}}";
			return string.Format(template, Name, Expression, Grammar.prefix, Expression.Generate(0), GenerateAction(), Type);
		}
	}
}
