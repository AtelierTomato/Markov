using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Model
{
	public class OIDPattern
	{
		public static Regex Generate(IList<string> fields)
		{
			if (fields.Count < 1)
				throw new ArgumentException("OIDPattern failed to construct as less than 1 field was given.", nameof(fields));

			StringBuilder sb = new();
			sb.Append(CultureInfo.InvariantCulture, $@"
^
(?<{fields[0]}>(?:[^:]|\^:)+)
");
			foreach (var field in fields.Skip(1))
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
