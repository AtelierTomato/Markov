using FluentAssertions;

namespace AtelierTomato.Markov.Renderer.Test
{
	public class SentenceRendererTests
	{
		[Theory]
		[InlineData("hello world how are you", "hello world how are you")]
		[InlineData("I 'm fine and how are you", "I'm fine and how are you")]
		[InlineData("wal*mart anime idol group wow", "wal*mart anime idol group wow")]
		[InlineData("the students ' council decided you die today", "the students' council decided you die today")]
		[InlineData("this is another short sentence .", "this is another short sentence.")]
		[InlineData("Steins;Gate , as well as Megaman.exe , are considered modern classics .", "Steins;Gate, as well as Megaman.exe, are considered modern classics.")]
		[InlineData("what are you doing to me ???????", "what are you doing to me???????")]
		[InlineData("are you going to buy a house !?!?!?!?!?", "are you going to buy a house!?!?!?!?!?")]
		[InlineData("where can I buy a salmon ?", "where can I buy a salmon?")]
		[InlineData("the help command is !help", "the help command is !help")]
		[InlineData("i dunno ,,,,, i thought it was pretty funny", "i dunno,,,,, i thought it was pretty funny")]
		[InlineData("meanwhile me : dying in the heat ; you , however , living in squalor .", "meanwhile me: dying in the heat; you, however, living in squalor.")]
		[InlineData("the .hack// franchise fucking sucks", "the .hack// franchise fucking sucks")]
		[InlineData("i am # dying over here", "i am #dying over here")]
		[InlineData("# besties !!!", "#besties!!!")]
		[InlineData("> implying", ">implying")]
		[InlineData("5 > 2", "5 > 2")]
		public void RenderSimpleText(string input, string output)
		{
			var target = new SentenceRenderer();

			var result = target.Render(input);

			result.Should().Be(output);
		}
	}
}