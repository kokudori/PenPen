using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

namespace Calculator
{
	[TestClass]
	public class Calculator
	{
		Parser parser;

		[TestInitialize]
		public void Initialize()
		{
			parser = new Parser();
		}

		[TestMethod]
		public void Primitive()
		{
			parser.Parse("0").Single().Is(0);
			parser.Parse("-0").Single().Is(-0);
			parser.Parse("1").Single().Is(1);
			parser.Parse("-1").Single().Is(-1);
			parser.Parse("100").Single().Is(100);
			parser.Parse("-100").Single().Is(-100);
		}

		[TestMethod]
		public void Addition()
		{
			parser.Parse("1 + 2").Single().Is(3);
			parser.Parse("100 + 200").Single().Is(300);
			parser.Parse("1 + -2").Single().Is(-1);
			parser.Parse("-1 + 2").Single().Is(1);
			parser.Parse("-1 + -2").Single().Is(-3);
		}

		[TestMethod]
		public void Subtraction()
		{
			parser.Parse("2 - 1").Single().Is(1);
			parser.Parse("200 - 100").Single().Is(100);
			parser.Parse("1 - 2").Single().Is(-1);
			parser.Parse("100 - 200").Single().Is(-100);
			parser.Parse("1 - -2").Single().Is(3);
			parser.Parse("-1 - 2").Single().Is(-3);
			parser.Parse("-1 - -2").Single().Is(1);
		}

		[TestMethod]
		public void Multiplication()
		{
			parser.Parse("1 * 2").Single().Is(2);
			parser.Parse("100 * 200").Single().Is(20000);
			parser.Parse("1 * -2").Single().Is(-2);
			parser.Parse("-1 * 2").Single().Is(-2);
			parser.Parse("-1 * -2").Single().Is(2);
		}

		[TestMethod]
		public void Division()
		{
			parser.Parse("2 / 1").Single().Is(2);
			parser.Parse("200 / 100").Single().Is(2);
			parser.Parse("1 / 2").Single().Is(0);
			parser.Parse("100 / 200").Single().Is(0);
			parser.Parse("1 / -2").Single().Is(-0);
			parser.Parse("-1 / 2").Single().Is(-0);
			parser.Parse("-1 / -2").Single().Is(0);
		}

		[TestMethod]
		public void Priority()
		{
			parser.Parse("1 + 2 * 3").Single().Is(7);
			parser.Parse("1 * 2 + 3").Single().Is(5);
			parser.Parse("2 / 2 * 4").Single().Is(4);
			parser.Parse("2 / 2 + 4").Single().Is(5);
		}

		[TestMethod]
		public void Expressions()
		{
			var input =
@"
1 + 2
1 + 2 * 3
1 * 2 + 3
1 + 2 * 3 / 4
";
			var result = parser.Parse(input);
			result.Length.Is(4);
			result[0].Is(3);
			result[1].Is(7);
			result[2].Is(5);
			result[3].Is(2);
		}
	}
}
