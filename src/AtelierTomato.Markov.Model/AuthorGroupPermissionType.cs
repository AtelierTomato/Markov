namespace AtelierTomato.Markov.Model
{
	[Flags]
	public enum AuthorGroupPermissionType
	{
		None = 0,
		SentencesInGroup = 1 << 0,  // 1
		UseGroup = 1 << 1,          // 2
		AddAuthor = 1 << 2,         // 4
		RemoveAuthor = 1 << 3,      // 8
		RenameGroup = 1 << 4,       // 16
		DeleteGroup = 1 << 5        // 32
	}
}
