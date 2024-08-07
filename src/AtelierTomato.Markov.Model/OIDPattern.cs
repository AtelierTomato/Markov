﻿using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Model
{
	public class OIDPattern
	{
		public static Regex Generate(IList<string> fields)
		{
			if (fields.Count < 2)
				throw new ArgumentException("OIDPattern failed to construct as less than 2 fields were given.", nameof(fields));

			StringBuilder sb = new();
			sb.Append(CultureInfo.InvariantCulture, $@"
^
(?<{fields[0]}>(?:[^:]|\^:)+)
(?<!\^):
(?<{fields[1]}>(?:[^:]|\^:)+)
");
			foreach (var field in fields.Skip(2))
			{
				sb.Append(CultureInfo.InvariantCulture, $@"
(?<!\^):?
(?<{field}>(?:[^:]|\^:)+)?
");
			}
			sb.Append('$');
			return new Regex(sb.ToString(), RegexOptions.IgnorePatternWhitespace);
		}
	}
}
