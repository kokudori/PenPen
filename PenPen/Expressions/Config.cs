namespace PenPen.Expressions
{
	class Config
	{
		internal string Name { get; private set; }
		internal string Value { get; private set; }

		internal Config(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}
}
