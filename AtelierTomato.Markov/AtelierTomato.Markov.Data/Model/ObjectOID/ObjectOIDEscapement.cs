﻿using System.Text.RegularExpressions;

namespace AtelierTomato.Markov.Data.Model.ObjectOID
{
	public static class ObjectOIDEscapement
	{
		public static string Escape(string value) => new Regex(@"[\^:]").Replace(value, m => "^" + m.Value);
		public static string Unescape(string value) => new Regex(@"\^([\^:])").Replace(value, m => m.Groups[1].Value);
	}
}
