using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenPen.Expressions
{
	class Grammar
	{
		internal static readonly string prefix = "PenPenSpecified";

		internal static IEnumerable<Symbol> Symbols { get; private set; }

		internal IEnumerable<Config> Configs { get; private set; }

		public Grammar(IEnumerable<Config> configs, IEnumerable<Symbol> symbols)
		{
			Configs = configs;
			Symbols = symbols;
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

		string MultiActionGenerate(Symbol symbol)
		{
			if (symbol.Expression.Action == null)
			{
				if (symbol.Expression is Rule)
					return string.Format("var {0}value = {0}actionResults.Pop().value;", prefix);
				return string.Format("var {0}value = input.Substring({0}state.index, {0}result.index - {0}state.index);", prefix);
			}

			var template = @"Func<{0}> {1}action = ({2}) => {3};
							var {1}value = ({4}){1}action({5});";
			var labels = GetLabel(symbol.Expression);

			if (!labels.Any())
				return string.Format(template, symbol.Type, prefix, "", symbol.Expression.Action, symbol.Type, "");

			var types = string.Join(", ", labels.Select(x => x.Type)) + ", " + symbol.Type;
			var parameters = string.Join(", ", labels.Select(x => x.Type + " " + x.Label));
			var args = string.Join(", ", labels.Select(x => prefix + "actionResults.Single(x => x.label == \"" + x.Label + "\").value"));
			return string.Format(template, types, prefix, parameters, symbol.Expression.Action, symbol.Type, args);
		}

		string MultiParserActionGenerate(Symbol symbol)
		{
			var template = @"{0}result = {1};
						if ({0}result.parsed)
						{{
							{2}
							{0}results.Push(new Result(null, ""{3}"", {0}value));
							return {0}result;
						}}";
			return string.Format(template, prefix, symbol.Expression.Generate(0), MultiActionGenerate(symbol), symbol.Type);
		}

		string MultiParserGenerate(IEnumerable<Symbol> symbols)
		{
			var template =
@"				// {0} = {1}
				{{ ""{0}"", ({2}state, {2}results) =>
					{{
						var {2}actionResults = new Stack<Result>();
						var {3}
						return new State(input, false, {2}state.index, 0);
					}}
				}}";
			var symbol = symbols.First();
			var expressions = string.Join(" / ", symbols.Select(x => x.Expression));
			var parsers = string.Join("\r\n\t\t\t\t\t\t", symbols.Select(x => MultiParserActionGenerate(x)));
			return string.Format(template, symbol.Name, expressions, prefix, parsers);
		}

		internal string Generate()
		{
			var template = @"
		internal Parser()
		{{
			#region Generated Rule Parsers

			parsers = new Dictionary<string, ParseFunc>()
			{{
{0}
			}};

			#endregion
		}}";
			var symbols = Symbols.GroupBy(x => x.Name);
			var parsers = symbols.Select(x => x.Count() == 1 ? x.Single().Generate() : MultiParserGenerate(x));
			return string.Format(template, string.Join(",\r\n", parsers));
		}

		internal string GenerateParse()
		{
			var template = @"<summary>
		/// Parses based on the rules given the input string
		/// </summary>
		/// <param name=""input"">input string</param>
		/// <returns>result data</returns>
		internal {0} Parse(string input)
		{{
			var value = default({0});
			var parsed = TryParse(input, out value);
			if (!parsed)
				throw new Exception(""parse failed"");
			return value;
		}}

		/// <summary>
		/// Parses based on the rules given the input string
		/// </summary>
		/// <param name=""input"">input string</param>
		/// <param name=""value"">result data</param>
		/// <returns>success or failure in the parse</returns>
		internal bool TryParse(string input, out {0} value)
		{{
			this.input = input + ""\0"";
			var results = new Stack<Result>();
			cache = new Dictionary<CacheKey, CacheValue>();
			var parsed = Rule(new State(input, true, 0, 0), results, ""{1}"").parsed;
			value = parsed ? results.Pop().value : default({0});
			return parsed;
		}}
";
			var start = Configs.Single(x => x.Name == "start").Value;
			var symbol = Symbols.First(x => x.Name == start);
			return string.Format(template, symbol.Type, symbol.Name);
		}
	}
}
