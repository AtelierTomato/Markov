﻿using AtelierTomato.Markov.Model;

namespace AtelierTomato.Markov.Storage.Sqlite
{
	public class AuthorGroupRow
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public AuthorGroupRow(string ID, string name)
		{
			this.ID = ID;
			Name = name;
		}
		public AuthorGroupRow(AuthorGroup authorGroup)
		{
			ID = authorGroup.ID.ToString();
			Name = authorGroup.Name;
		}
		public AuthorGroup ToAuthorGroup() => new(Guid.Parse(ID), Name);
	}
}