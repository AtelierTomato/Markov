using AtelierTomato.Markov.Service.ActivityPub.Model;
using FluentAssertions;

namespace AtelierTomato.Markov.Service.ActivityPub.Test
{
	public class ActivityPubSentenceRendererTests
	{
		[Fact]
		public void RenderActivityPubEmojiTest()
		{
			var target = new ActivityPubSentenceRenderer();

			IReadOnlyCollection<ActivityPubEmoji> emojis = [
				new("madou", "_", "_", true),
				new("mikanScreech", "_", "_", true)
			];
			var result = target.Render("i like emojis e:madou: e:mikanScreech: e:greg:", emojis);

			result.Should().Be("i like emojis :madou: :mikanScreech: greg");
		}

		// Below are defaults
		[Theory]
		[InlineData("hello world how are you", "hello world how are you")]
		[InlineData("I 'm fine and how are you", "I'm fine and how are you")]
		[InlineData("I ’m fine and how are you", "I’m fine and how are you")]
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
		public void RenderSimpleTextTest(string input, string output)
		{
			var target = new ActivityPubSentenceRenderer();

			var result = target.Render(input, []);

			result.Should().Be(output);
		}


		[Theory]
		[InlineData("( this sentence is in parentheses )", "(this sentence is in parentheses)")]
		[InlineData("i am god [ citation needed ] and you are stupid { you are DUMB }", "i am god [citation needed] and you are stupid {you are DUMB}")]
		[InlineData("\" who are you quoting ? \"", "\"who are you quoting?\"")]
		[InlineData("god \" jimmy \" jesus here with another minecraft video", "god \"jimmy\" jesus here with another minecraft video")]
		[InlineData("1 \" 2 \" b \" 3 \" 4 5", "1 \"2\" b \"3\" 4 5")]
		public void RenderSurroundingCharactersTest(string input, string output)
		{
			var target = new ActivityPubSentenceRenderer();

			var result = target.Render(input, []);

			result.Should().Be(output);
		}

		[Theory]
		[InlineData("i 've found a million dollars", "i've found a million dollars")]
		[InlineData("what 's up gamers , I 'm out here gaming", "what's up gamers, I'm out here gaming")]
		[InlineData("the students ' council decided you die today", "the students' council decided you die today")]
		[InlineData("and i can tell that doki doki literature club is shitty faux - anime normie garbage", "and i can tell that doki doki literature club is shitty faux-anime normie garbage")]
		[InlineData("i want to eat — drink water", "i want to eat—drink water")]
		public void RenderContractionsTest(string input, string output)
		{
			var target = new ActivityPubSentenceRenderer();

			var result = target.Render(input, []);

			result.Should().Be(output);
		}

		[Theory]
		[InlineData("e:ShihoLook: at the sky", "ShihoLook at the sky")]
		[InlineData("e:ShihoLook: e:ShihoLook: e:ShihoLook:", "ShihoLook ShihoLook ShihoLook")]
		[InlineData("e:apple2:", "apple2")]
		[InlineData("e:__:", "__")]
		public void RenderEmojisTest(string input, string output)
		{
			var target = new ActivityPubSentenceRenderer();

			var result = target.Render(input, []);

			result.Should().Be(output);
		}


		[Theory]
		[InlineData("¿ Dónde está mi gran sombrero ?", "¿Dónde está mi gran sombrero?")]
		[InlineData("¡ No encuentro mi pierna izquierda !", "¡No encuentro mi pierna izquierda!")]
		[InlineData("¡¿ Alguien puede ayudarme por favor ?!", "¡¿Alguien puede ayudarme por favor?!")]
		[InlineData("¡¡¡¿¿¿ Alguien puede ayudarme por favor ???!!!", "¡¡¡¿¿¿Alguien puede ayudarme por favor???!!!")]
		[InlineData("« Je m 'appelle Marinette , une fille comme les autres »", "«Je m'appelle Marinette, une fille comme les autres»")]
		[InlineData("» Je m 'appelle Marinette , une fille comme les autres «", "»Je m'appelle Marinette, une fille comme les autres«")]
		[InlineData("» Je m 'appelle Marinette , une fille comme les autres »", "»Je m'appelle Marinette, une fille comme les autres»")]
		[InlineData("« Je m 'appelle Marinette , une fille comme les autres «", "«Je m'appelle Marinette, une fille comme les autres«")]
		public void RenderForeignTests(string input, string output)
		{
			var target = new ActivityPubSentenceRenderer();

			var result = target.Render(input, []);

			result.Should().Be(output);
		}
	}
}
