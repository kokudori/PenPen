﻿/*
 * PenPen PEG Parser v0.0.1
 * http://github.com/kokudori/PenPen
 * 
 * Copyright (c) 2013 Kokudori <@Kokudori on Twitter>
 * Released under the MIT License
 * http://opensource.org/licenses/mit-license.php
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using PenPen.Expressions;

// {0}
	class Parser
	{
		delegate State ParseFunc(State state, Stack<Result> results);
		string input;
		Dictionary<CacheKey, CacheValue> cache;
		Dictionary<string, ParseFunc> parsers;

		/// {1}{2}

		#region Built-in Parsers

		State Rule(State state, Stack<Result> results, string rule)
		{
			var key = new CacheKey(rule, state.index);
			CacheValue value;
			if (cache.TryGetValue(key, out value))
			{
				if (value.state.parsed)
					results.Push(value.result);
				return value.state;
			}
			else
			{
				var result = parsers[rule](state, results);
				cache.Add(key, new CacheValue(result, result.parsed ? results.Peek() : null));
				return result;
			}
		}

		State Choice(State state, Stack<Result> results, bool isImplicit, params ParseFunc[] parses)
		{
			var start = state.index;
			foreach (var parse in parses)
			{
				var result = parse(state, results);
				if (result.parsed)
				{
					if (isImplicit)
					{
						var value = results.Pop();
						if (value.type == "char" && value.value is char)
							results.Push(new Result(value.label, "string", value.value.ToString()));
						else
							results.Push(value);
					}
					return result;
				}
				state = new State(input, false, start, 0);
			}
			return state;
		}

		State Sequence(State state, Stack<Result> results, params ParseFunc[] parses)
		{
			var start = state.index;
			var stack = new Stack<Result>();
			foreach (var parse in parses)
			{
				state = parse(state, results);
				if (!state.parsed)
					return new State(input, false, start, 0);
				var result = results.Pop();
				if (!(result is Assert))
					stack.Push(result);
			}
			foreach (var result in stack)
			{
				results.Push(result);
			}
			return new State(input, true, state.index, state.index - start);
		}

		State Sequence<T>(State state, Stack<Result> results, bool inner, params ParseFunc[] parses)
		{
			var result = Sequence(state, results, parses);
			if (!result.parsed)
				return new State(input, false, state.index, 0);
			if (inner)
			{
				var types = string.Format("{{0}}", typeof(T).Name);
				var value = results.Pop().value;
				results.Push(new Result(null, types, value));
			}
			return result;
		}

		State Sequence<T1, T2>(State state, Stack<Result> results, bool inner, params ParseFunc[] parses)
		{
			var result = Sequence(state, results, parses);
			if (!result.parsed)
				return new State(input, false, state.index, 0);
			if (inner)
			{
				var types = string.Format("Tuple<{{0}}, {{1}}>", typeof(T1).Name, typeof(T2).Name);
				var value = new Tuple<T1, T2>(results.Pop().value, results.Pop().value);
				results.Push(new Result(null, types, value));
			}
			return result;
		}

		State Sequence<T1, T2, T3>(State state, Stack<Result> results, bool inner, params ParseFunc[] parses)
		{
			var result = Sequence(state, results, parses);
			if (!result.parsed)
				return new State(input, false, state.index, 0);
			if (inner)
			{
				var types = string.Format("Tuple<{{0}}, {{1}}, {{2}}>", typeof(T1).Name, typeof(T2).Name, typeof(T3).Name);
				var value = new Tuple<T1, T2, T3>(results.Pop().value, results.Pop().value, results.Pop().value);
				results.Push(new Result(null, types, value));
			}
			return result;
		}

		State Sequence<T1, T2, T3, T4>(State state, Stack<Result> results, bool inner, params ParseFunc[] parses)
		{
			var result = Sequence(state, results, parses);
			if (!result.parsed)
				return new State(input, false, state.index, 0);
			if (inner)
			{
				var types = string.Format("Tuple<{{0}}, {{1}}, {{2}}, {{3}}>", typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name);
				var value = new Tuple<T1, T2, T3, T4>(results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value);
				results.Push(new Result(null, types, value));
			}
			return result;
		}

		State Sequence<T1, T2, T3, T4, T5>(State state, Stack<Result> results, bool inner, params ParseFunc[] parses)
		{
			var result = Sequence(state, results, parses);
			if (!result.parsed)
				return new State(input, false, state.index, 0);
			if (inner)
			{
				var types = string.Format("Tuple<{{0}}, {{1}}, {{2}}, {{3}}, {{4}}>", typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5));
				var value = new Tuple<T1, T2, T3, T4, T5>(results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value);
				results.Push(new Result(null, types, value));
			}
			return result;
		}

		State Sequence<T1, T2, T3, T4, T5, T6>(State state, Stack<Result> results, bool inner, params ParseFunc[] parses)
		{
			var result = Sequence(state, results, parses);
			if (!result.parsed)
				return new State(input, false, state.index, 0);
			if (inner)
			{
				var types = string.Format("Tuple<{{0}}, {{1}}, {{2}}, {{3}}, {{4}}, {{5}}>", typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5), typeof(T6));
				var value = new Tuple<T1, T2, T3, T4, T5, T6>(results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value);
				results.Push(new Result(null, types, value));
			}
			return result;
		}

		State Sequence<T1, T2, T3, T4, T5, T6, T7>(State state, Stack<Result> results, bool inner, params ParseFunc[] parses)
		{
			var result = Sequence(state, results, parses);
			if (!result.parsed)
				return new State(input, false, state.index, 0);
			if (inner)
			{
				var types = string.Format("Tuple<{{0}}, {{1}}, {{2}}, {{3}}, {{4}}, {{5}}, {{6}}>", typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5), typeof(T6).Name, typeof(T7));
				var value = new Tuple<T1, T2, T3, T4, T5, T6, T7>(results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value);
				results.Push(new Result(null, types, value));
			}
			return result;
		}

		State Sequence<T1, T2, T3, T4, T5, T6, T7, T8>(State state, Stack<Result> results, bool inner, params ParseFunc[] parses)
		{
			var result = Sequence(state, results, parses);
			if (!result.parsed)
				return new State(input, false, state.index, 0);
			if (inner)
			{
				var types = string.Format("Tuple<{{0}}, {{1}}, {{2}}, {{3}}, {{4}}, {{5}}, {{6}}, {{7}}>", typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5), typeof(T6).Name, typeof(T7), typeof(T8));
				var value = new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, new Tuple<T8>(results.Pop().value));
				results.Push(new Result(null, types, value));
			}
			return result;
		}

		State Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9>(State state, Stack<Result> results, bool inner, params ParseFunc[] parses)
		{
			var result = Sequence(state, results, parses);
			if (!result.parsed)
				return new State(input, false, state.index, 0);
			if (inner)
			{
				var types = string.Format("Tuple<{{0}}, {{1}}, {{2}}, {{3}}, {{4}}, {{5}}, {{6}}, {{7}}, {{8}}>", typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5), typeof(T6).Name, typeof(T7), typeof(T8), typeof(T9));
				var value = new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>(results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, new Tuple<T8, T9>(results.Pop().value, results.Pop().value));
				results.Push(new Result(null, types, value));
			}
			return result;
		}

		State Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(State state, Stack<Result> results, bool inner, params ParseFunc[] parses)
		{
			var result = Sequence(state, results, parses);
			if (!result.parsed)
				return new State(input, false, state.index, 0);
			if (inner)
			{
				var types = string.Format("Tuple<{{0}}, {{1}}, {{2}}, {{3}}, {{4}}, {{5}}, {{6}}, {{7}}, {{8}}, {{9}}>", typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5), typeof(T6).Name, typeof(T7), typeof(T8), typeof(T9), typeof(T10));
				var value = new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>(results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, new Tuple<T8, T9, T10>(results.Pop().value, results.Pop().value, results.Pop().value));
				results.Push(new Result(null, types, value));
			}
			return result;
		}

		State Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(State state, Stack<Result> results, bool inner, params ParseFunc[] parses)
		{
			var result = Sequence(state, results, parses);
			if (!result.parsed)
				return new State(input, false, state.index, 0);
			if (inner)
			{
				var types = string.Format("Tuple<{{0}}, {{1}}, {{2}}, {{3}}, {{4}}, {{5}}, {{6}}, {{7}}, {{8}}, {{9}}, {{10}}>", typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5), typeof(T6).Name, typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11));
				var value = new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>(results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, new Tuple<T8, T9, T10, T11>(results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value));
				results.Push(new Result(null, types, value));
			}
			return result;
		}

		State Sequence<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(State state, Stack<Result> results, bool inner, params ParseFunc[] parses)
		{
			var result = Sequence(state, results, parses);
			if (!result.parsed)
				return new State(input, false, state.index, 0);
			if (inner)
			{
				var types = string.Format("Tuple<{{0}}, {{1}}, {{2}}, {{3}}, {{4}}, {{5}}, {{6}}, {{7}}, {{8}}, {{9}}, {{10}}, {{11}}>", typeof(T1).Name, typeof(T2).Name, typeof(T3).Name, typeof(T4).Name, typeof(T5), typeof(T6).Name, typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12));
				var value = new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>(results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, new Tuple<T8, T9, T10, T11, T12>(results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value, results.Pop().value));
				results.Push(new Result(null, types, value));
			}
			return result;
		}

		State Literal(State state, Stack<Result> results, string type, string literal)
		{
			if (literal.Length > input.Skip(state.index).Count())
				return new State(input, false, state.index, 0);
			var text = input.Substring(state.index, literal.Length);
			if (text != literal)
				return new State(input, false, state.index, 0);

			if (type == "string")
				results.Push(new Result(null, type, text));
			else if (type == "char")
				results.Push(new Result(null, type, text.Single()));
			return new State(input, true, state.index + literal.Length, literal.Length);
		}

		State Optional<T>(State state, Stack<Result> results, string type, ParseFunc parse)
		{
			var count = results.Count;
			var result = parse(state, results);
			var index = result.parsed ? result.index : state.index;
			var length = result.parsed ? result.index - state.index : 0;
			var value = result.parsed ? Maybe<T>.Just(results.Pop().value) : Maybe<T>.Nothing();

			results.Push(new Result(null, type, value));
			return new State(input, true, index, length);
		}

		State Repetition<T>(State state, Stack<Result> results, int min, int? max, ParseFunc parse)
		{
			var list = new List<T>();

			var start = state;
			max = max ?? int.MaxValue;
			var count = 0;
			while (count <= max)
			{
				var result = parse(state, results);
				if (result.parsed && result.index > state.index)
				{
					state = result;
					list.Add(results.Pop().value);
					count++;
				}
				else
				{
					state = new State(input, true, state.index, state.index - start.index);
					break;
				}
			}
			if (count < min || count > max)
				return new State(input, false, start.index, 0);

			if (typeof(T).Name == "Char")
				results.Push(new Result(null, "string", string.Join("", list.Select(x => x.ToString()))));
			else
				results.Push(new Result(null, "IEnumerable<" + typeof(T).Name + ">", list));
			return state;
		}

		State Range(State state, Stack<Result> results, bool inverted, string chars, string ranges, UnicodeCategory[] categories, string label)
		{
			var ch = input[state.index];
			var matched = chars.IndexOf(ch) >= 0;

			for (int i = 0; i < ranges.Length && !matched; i += 2)
			{
				matched = ranges[i] <= ch && ch <= ranges[i + 1];
			}
			for (int i = 0; categories != null && i < categories.Length && !matched; ++i)
			{
				matched = char.GetUnicodeCategory(ch) == categories[i];
			}

			if (inverted)
				matched = !matched && ch != '\x0';

			if (matched)
			{
				results.Push(new Result(null, "char", input.Substring(state.index, 1).Single()));
				return new State(input, true, state.index + 1, 1);
			}

			return new State(input, false, state.index, 0);
		}

		State PositiveAssert(State state, Stack<Result> results, ParseFunc parse)
		{
			var result = parse(state, results);
			if (result.parsed)
				results.Push(new Assert(results.Pop()));
			return new State(input, result.parsed, state.index, 0);
		}

		State NegativeAssert(State state, Stack<Result> results, ParseFunc parse)
		{
			var result = parse(state, results);
			if (result.parsed)
				results.Pop();
			else
				results.Push(new Assert(new Result(null, null, null)));
			return new State(input, !result.parsed, state.index, 0);
		}

		State Label(State state, Stack<Result> results, string label, ParseFunc parse)
		{
			var result = parse(state, results);
			if (!result.parsed)
				return new State(input, false, state.index, 0);

			var value = results.Pop();
			results.Push(new Result(label, value.type, value.value));
			return result;
		}

		#endregion

		#region Internal Informations

		private sealed class State
		{
			internal readonly string input;
			internal readonly bool parsed;
			internal readonly int index;
			internal readonly int length;

			internal State(string input, bool parsed, int index, int length)
			{
				this.input = input;
				this.parsed = parsed;
				this.index = index;
				this.length = length;
			}

			int line()
			{
				var line = 1;
				var i = 0;
				while (i <= index)
				{
					var ch = input[i++];
					if (ch == '\r' && input[i] == '\n')
					{
						++i;
						++line;
					}
					else if (ch == '\r' || ch == '\n')
					{
						++line;
					}
				}
				return line;
			}

			int colum()
			{
				var i = index;
				while (i > 0 && input[i - 1] != '\n' && input[i - 1] != '\r')
				{
					--i;
				}
				return index - i + 1;
			}

			public override string ToString()
			{
				var successed = parsed ? "success" : "failed";
				return string.Format("{{0}}: {{1}}Line, {{2}}Colum", successed, line(), colum());
			}
		}

		private class Result
		{
			internal readonly string label;
			internal readonly string type;
			internal readonly dynamic value;

			internal Result(string label, string type, dynamic value)
			{
				this.label = label;
				this.type = type;
				this.value = value;
			}
		}

		private sealed class Assert : Result
		{
			public Assert(Result result)
				: base(result.label, result.type, result.value as object)
			{

			}
		}

		private struct CacheKey : IEquatable<CacheKey>
		{
			string rule;
			int index;

			internal CacheKey(string rule, int index)
			{
				this.rule = rule;
				this.index = index;
			}

			public bool Equals(CacheKey other)
			{
				if (this.rule != other.rule)
					return false;
				if (this.index != other.index)
					return false;
				return true;
			}

			public static bool operator ==(CacheKey left, CacheKey right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(CacheKey left, CacheKey right)
			{
				return !(left == right);
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;
				if (GetType() != obj.GetType())
					return false;
				return this == (CacheKey)obj;
			}

			public override int GetHashCode()
			{
				var hash = 0;
				unchecked
				{
					hash += rule.GetHashCode();
					hash += index.GetHashCode();
				}
				return hash;
			}
		}

		private struct CacheValue
		{
			internal readonly State state;
			internal readonly Result result;

			internal CacheValue(State state, Result result)
			{
				this.state = state;
				this.result = result;
			}
		}

		#endregion

		#region Utilities

		class Maybe<T>
		{
			readonly T value;

			internal readonly bool isJust;
			internal readonly bool isNothing;

			Maybe(T value, bool isJust)
			{
				this.value = value;
				this.isJust = isJust;
				isNothing = !isJust;
			}

			internal T Get()
			{
				if (isNothing)
					throw new Exception("nothing");
				return value;
			}

			internal T Get(T other)
			{
				return isJust ? value : other;
			}

			internal static Maybe<T> Just(T value)
			{
				return new Maybe<T>(value, true);
			}

			internal static Maybe<T> Nothing()
			{
				return new Maybe<T>(default(T), false);
			}
		}

		#endregion
	}
// {3}