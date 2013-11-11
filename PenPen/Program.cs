using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PenPen
{
	class Program
	{
		static void Main(string[] args)
		{
			var file = args
				.Where(x => !x.StartsWith("-"))
				.SingleOrDefault();
			var output = args
				.Where(x => x.StartsWith("-o=") || x.StartsWith("--output="))
				.Select(x => x.SkipWhile(y => y != '=').Skip(1))
				.Select(x => string.Join("", x))
				.SingleOrDefault();
			var spacename = args
				.Where(x => x.StartsWith("-n=") || x.StartsWith("--namespace="))
				.Select(x => x.SkipWhile(y => y != '=').Skip(1))
				.Select(x => string.Join("", x))
				.SingleOrDefault();
			var version = Assembly.GetExecutingAssembly().GetName().Version;

			if (file == null)
			{
				Console.WriteLine("PenPen version {0}", version);
				Console.WriteLine("usage: PenPen [options] file.pen");
				Console.WriteLine("  -o, --out=VALUE\t\tpath to the parser file to be generated");
				Console.WriteLine("  -n, --namespace=VALUE\t\tgenerated parser class namespace");
				return;
			}

			var input = File.ReadAllText(file);
			var name = Path.GetFileNameWithoutExtension(file);
			output = output ?? name + ".cs";
			spacename = spacename ?? "PenPen";

			Console.WriteLine("Parsing...");
			var timer = Stopwatch.StartNew();
			var grammar = new Parser().Parse(input);

			using (var stream = File.CreateText(output))
			using (var writer = new Writer(stream, grammar, spacename))
			{
				writer.Write();
				stream.Flush();
			}

			Console.WriteLine("Generated {0} by PenPen {1}", output, version);
			Console.WriteLine("finished in {0:0.000} secs", timer.ElapsedMilliseconds / 1000.0);
			return;
		}
	}
}
