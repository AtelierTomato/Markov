namespace AtelierTomato.Markov.Model
{
	[Flags]
	public enum AuthorGroupPermissionType
	{
		None = 0,
		Accepted = 1 << 0,          // 1
		SentencesInGroup = 1 << 1,  // 2
		UseGroup = 1 << 2,          // 4
		AddAuthor = 1 << 3,         // 8
		RemoveAuthor = 1 << 4,      // 16
		RenameGroup = 1 << 5,       // 32
		DeleteGroup = 1 << 6        // 64
	}
}
