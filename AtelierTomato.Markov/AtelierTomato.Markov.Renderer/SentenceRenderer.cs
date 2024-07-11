
namespace AtelierTomato.Markov.Renderer
{
	public class SentenceRenderer
	{
		public string Render(string text)
		{
			text = RenderDetachCharacters(text);
			text = RenderEmojis(text);
			return text;
		}

		private string RenderDetachCharacters(string text)
		{
			return text;
		}

		private string RenderEmojis(string text)
		{
			return text;
		}
	}
}
