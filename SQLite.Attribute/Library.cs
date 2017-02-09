namespace SQLite.Attribute
{
    using System;

	[AttributeUsage(AttributeTargets.Class)]
	public class ExportAttribute : System.Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class TableAttribute : System.Attribute
	{
		public string Name { get; set; }

		public TableAttribute(string name)
		{
			Name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class ColumnAttribute : System.Attribute
	{
		public string Name { get; set; }

		public ColumnAttribute(string name)
		{
			Name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class PrimaryKeyAttribute : System.Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class AutoIncrementAttribute : System.Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class IndexedAttribute : System.Attribute
	{
		public string Name { get; set; }
		public int Order { get; set; }
		public virtual bool Unique { get; set; }

		public IndexedAttribute()
		{
		}

		public IndexedAttribute(string name, int order)
		{
			Name = name;
			Order = order;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class IgnoreAttribute : System.Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class UniqueAttribute : IndexedAttribute
	{
		public override bool Unique
		{
			get { return true; }
			set { /* throw?  */ }
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class MaxLengthAttribute : System.Attribute
	{
		public int Value { get; private set; }

		public MaxLengthAttribute(int length)
		{
			Value = length;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class CollationAttribute : System.Attribute
	{
		public string Value { get; private set; }

		public CollationAttribute(string collation)
		{
			Value = collation;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class NotNullAttribute : System.Attribute
	{
	}
}
