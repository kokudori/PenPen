using System;
using System.Collections.Generic;
using System.Text;

namespace PenPen.Expressions
{
	class Range : BaseExpression
	{
		internal string Chars { get; private set; }
		internal string Ranges { get; private set; }
		internal string Categories { get; private set; }
		internal string CategoryLabel { get; private set; }
		internal bool Inverted { get; private set; }

		internal override string Type
		{
			get { return "char"; }
		}

		internal Range(string text)
		{
			var chars = new StringBuilder();
			var ranges = new StringBuilder();
			var categories = new List<string>();

			CategoryLabel = "";
			Inverted = text[0] == '^';

			var i = Inverted ? 1 : 0;
			while (i < text.Length)
			{
				if (i + 3 < text.Length && text[i] == '\\' && text[i + 1] == 'c')
				{
					categories.Add(Category(text.Substring(i + 2, 2)));
					CategoryLabel += "\\\\c" + text.Substring(i + 2, 2);
					i += 4;
				}
				else if (i + 2 < text.Length && text[i + 1] == '-')	// note that - has no effect at the start or the end of the text
				{
					ranges.Append(text.Substring(i, 1) + text[i + 2]);
					i += 3;
				}
				else
				{
					chars.Append(text[i++]);
				}
			}

			Chars = chars.ToString();
			Ranges = ranges.ToString();
			if (categories.Count > 0)
				Categories = "new UnicodeCategory[]{" + string.Join(", ", categories.ToArray()) + "}";
			else
				Categories = "null";
		}

		internal override string Generate(int nest)
		{
			var inverted = Inverted ? "true" : "false";
			var chars = EscapeAll(Chars).Replace("\\]", "]").Replace("\"", "\\\"");
			var ranges = Escape(Ranges).Replace("\"", "\\\"");

			var template = nest > 0
				? Symbol.Indent(nest) + "({0}, {1}) => Range({0}, {1}, {2}, \"{3}\", \"{4}\", {5}, \"{6}\")"
				: "Range({0}, {1}, {2}, \"{3}\", \"{4}\", {5}, \"{6}\")";
			return string.Format(template, Symbol.State(nest), Symbol.Result(nest), inverted, chars, ranges, Categories, this);
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			if (Chars.Length == 0 && Ranges == "\x0001\xFFFF")
			{
				builder.Append('.');
			}
			else
			{
				builder.Append('[');
				if (Inverted)
					builder.Append('^');
				if (Chars.Length > 0)
					builder.Append(Escape(Chars));
				builder.Append(CategoryLabel);
				for (int i = 0; i < Ranges.Length; i += 2)
				{
					builder.Append(Escape(Ranges.Substring(i, 1)));
					builder.Append('-');
					builder.Append(Escape(Ranges.Substring(i + 1, 1)));
				}
				builder.Append(']');
			}

			return builder.ToString().Replace("\"", "\\\"");
		}

		string Escape(string s)
		{
			return EscapeAll(s).Replace("]", "\\]");
		}

		string EscapeAll(string s)
		{
			var builder = new System.Text.StringBuilder(s.Length);

			foreach (char ch in s)
			{
				if (ch == '\n')
					builder.Append("\\n");

				else if (ch == '\r')
					builder.Append("\\r");

				else if (ch == '\t')
					builder.Append("\\t");

				else if (ch < ' ' || ch > '\x7F')
					builder.AppendFormat("\\x{0:X4}", (int)ch);

				else
					builder.Append(ch);
			}

			return builder.ToString();
		}

		string Category(string category)
		{
			switch (category)
			{
				case "Lu":
					return "UnicodeCategory.UppercaseLetter";
				case "Ll":
					return "UnicodeCategory.LowercaseLetter";
				case "Lt":
					return "UnicodeCategory.TitlecaseLetter";
				case "Lm":
					return "UnicodeCategory.ModifierLetter";
				case "Lo":
					return "UnicodeCategory.OtherLetter";
				case "Mn":
					return "UnicodeCategory.NonSpacingMark";
				case "Mc":
					return "UnicodeCategory.SpacingCombiningMark";
				case "Me":
					return "UnicodeCategory.EnclosingMark";
				case "Nd":
					return "UnicodeCategory.DecimalDigitNumber";
				case "Nl":
					return "UnicodeCategory.LetterNumber";
				case "No":
					return "UnicodeCategory.OtherNumber";
				case "Zs":
					return "UnicodeCategory.SpaceSeparator";
				case "Zl":
					return "UnicodeCategory.LineSeparator";
				case "Zp":
					return "UnicodeCategory.ParagraphSeparator";
				case "Cc":
					return "UnicodeCategory.Control";
				case "Cf":
					return "UnicodeCategory.Format";
				case "Cs":
					return "UnicodeCategory.Surrogate";
				case "Co":
					return "UnicodeCategory.PrivateUse";
				case "Pc":
					return "UnicodeCategory.ConnectorPunctuation";
				case "Pd":
					return "UnicodeCategory.DashPunctuation";
				case "Ps":
					return "UnicodeCategory.OpenPunctuation";
				case "Pe":
					return "UnicodeCategory.ClosePunctuation";
				case "Pi":
					return "UnicodeCategory.InitialQuotePunctuation";
				case "Pf":
					return "UnicodeCategory.FinalQuotePunctuation";
				case "Po":
					return "UnicodeCategory.OtherPunctuation";
				case "Sm":
					return "UnicodeCategory.MathSymbol";
				case "Sc":
					return "UnicodeCategory.CurrencySymbol";
				case "Sk":
					return "UnicodeCategory.ModifierSymbol";
				case "So":
					return "UnicodeCategory.OtherSymbol";
				case "Cn":
					return "UnicodeCategory.OtherNotAssigned";
				default:
					throw new ArgumentException(category + " is not a valid Unicode character category");
			}
		}
	}
}
