using Discord;
using FluentAssertions;

namespace AtelierTomato.Markov.Service.Discord.Test
{
	public class DiscordSentenceRendererTests
	{
		[Theory]
		[InlineData("hello world how are you", "hello world how are you")]
		[InlineData("I 'm fine and how are you", "I\\'m fine and how are you")]
		[InlineData("wal*mart anime idol group wow", "wal\\*mart anime idol group wow")]
		[InlineData("the students ' council decided you die today", "the students\\' council decided you die today")]
		[InlineData("this is another short sentence .", "this is another short sentence\\.")]
		[InlineData("Steins;Gate , as well as Megaman.exe , are considered modern classics .", "Steins\\;Gate\\, as well as Megaman\\.exe\\, are considered modern classics\\.")]
		[InlineData("what are you doing to me ???????", "what are you doing to me\\?\\?\\?\\?\\?\\?\\?")]
		[InlineData("are you going to buy a house !?!?!?!?!?", "are you going to buy a house\\!\\?\\!\\?\\!\\?\\!\\?\\!\\?")]
		[InlineData("where can I buy a salmon ?", "where can I buy a salmon\\?")]
		[InlineData("the help command is !help", "the help command is \\!help")]
		[InlineData("i dunno ,,,,, i thought it was pretty funny", "i dunno\\,\\,\\,\\,\\, i thought it was pretty funny")]
		[InlineData("meanwhile me : dying in the heat ; you , however , living in squalor .", "meanwhile me\\: dying in the heat\\; you\\, however\\, living in squalor\\.")]
		[InlineData("the .hack// franchise fucking sucks", "the \\.hack\\/\\/ franchise fucking sucks")]
		[InlineData("i am # dying over here", "i am \\#dying over here")]
		[InlineData("# besties !!!", "\\#besties\\!\\!\\!")]
		[InlineData("> implying", "\\>implying")]
		[InlineData("5 > 2", "5 \\> 2")]
		public void RenderSimpleTextTest(string input, string output)
		{
			var target = new DiscordSentenceRenderer();

			var result = target.Render(input);

			result.Should().Be(output);
		}


		[Theory]
		[InlineData("( this sentence is in parentheses )", "\\(this sentence is in parentheses\\)")]
		[InlineData("i am god [ citation needed ] and you are stupid { you are DUMB }", "i am god \\[citation needed\\] and you are stupid \\{you are DUMB\\}")]
		[InlineData("\" who are you quoting ? \"", "\\\"who are you quoting\\?\\\"")]
		[InlineData("god \" jimmy \" jesus here with another minecraft video", "god \\\"jimmy\\\" jesus here with another minecraft video")]
		public void RenderSurroundingCharactersTest(string input, string output)
		{
			var target = new DiscordSentenceRenderer();

			var result = target.Render(input);

			result.Should().Be(output);
		}

		[Theory]
		[InlineData("i 've found a million dollars", "i\\'ve found a million dollars")]
		[InlineData("what 's up gamers , I 'm out here gaming", "what\\'s up gamers\\, I\\'m out here gaming")]
		[InlineData("the students ' council decided you die today", "the students\\' council decided you die today")]
		[InlineData("and i can tell that doki doki literature club is shitty faux- anime normie garbage", "and i can tell that doki doki literature club is shitty faux\\-anime normie garbage")]
		public void RenderContractionsTest(string input, string output)
		{
			var target = new DiscordSentenceRenderer();

			var result = target.Render(input);

			result.Should().Be(output);
		}

		[Theory]
		[InlineData("e:ShihoLook:", "<:ShihoLook:402558230427074560>")]
		[InlineData("e:ShihoLook: e:applesauce:", "<:ShihoLook:402558230427074560> applesauce")]
		[InlineData("appe:ShihoLook:le", "app<:ShihoLook:402558230427074560>le")]
		[InlineData("e:ShihoLook: e:ShihoLook: e:ShihoLook:", "<:ShihoLook:402558230427074560> <:ShihoLook:402558230427074560> <:ShihoLook:402558230427074560>")]
		[InlineData("e:apple2:", "apple2")]
		[InlineData("e:__:", "\\_\\_")]
		public void RenderEmojisInCurrentTest(string input, string output)
		{
			var emote = new Emote(402558230427074560, "ShihoLook");

			IEnumerable<Emote> currentEmojis = [emote];
			IEnumerable<Emote> allEmojis = [];

			var target = new DiscordSentenceRenderer();

			var result = target.Render(input, currentEmojis, allEmojis);
			result.Should().Be(output);
		}

		[Theory]
		[InlineData("e:ShihoLook:", "<:ShihoLook:402558230427074560>")]
		[InlineData("e:ShihoLook: e:applesauce:", "<:ShihoLook:402558230427074560> applesauce")]
		[InlineData("appe:ShihoLook:le", "app<:ShihoLook:402558230427074560>le")]
		[InlineData("e:ShihoLook: e:ShihoLook: e:ShihoLook:", "<:ShihoLook:402558230427074560> <:ShihoLook:402558230427074560> <:ShihoLook:402558230427074560>")]
		[InlineData("e:apple2:", "apple2")]
		[InlineData("e:__:", "\\_\\_")]
		public void RenderEmojisInOtherTest(string input, string output)
		{
			var emote = new Emote(402558230427074560, "ShihoLook");

			IEnumerable<Emote> currentEmojis = [];
			IEnumerable<Emote> allEmojis = [emote];

			var target = new DiscordSentenceRenderer();

			var result = target.Render(input, currentEmojis, allEmojis);
			result.Should().Be(output);
		}
		[Theory]
		[InlineData("e:1234____:", "<:1234____:402558230427074560>")]
		[InlineData("e:1234____: e:applesauce:", "<:1234____:402558230427074560> applesauce")]
		[InlineData("appe:1234____:le", "app<:1234____:402558230427074560>le")]
		[InlineData("e:1234____: e:1234____: e:1234____:", "<:1234____:402558230427074560> <:1234____:402558230427074560> <:1234____:402558230427074560>")]
		public void RenderEmojisNumeric(string input, string output)
		{
			var emote = new Emote(402558230427074560, "1234____");

			IEnumerable<Emote> currentEmojis = [];
			IEnumerable<Emote> allEmojis = [emote];

			var target = new DiscordSentenceRenderer();

			var result = target.Render(input, currentEmojis, allEmojis);
			result.Should().Be(output);
		}
		[Theory]
		[InlineData("e:ShihoLook:", "<a:ShihoLook:402558230427074560>")]
		[InlineData("e:ShihoLook: e:applesauce:", "<a:ShihoLook:402558230427074560> applesauce")]
		[InlineData("appe:ShihoLook:le", "app<a:ShihoLook:402558230427074560>le")]
		[InlineData("e:ShihoLook: e:ShihoLook: e:ShihoLook:", "<a:ShihoLook:402558230427074560> <a:ShihoLook:402558230427074560> <a:ShihoLook:402558230427074560>")]
		public void RenderEmojisInAnimated(string input, string output)
		{
			var emote = new Emote(402558230427074560, "ShihoLook", true);

			IEnumerable<Emote> currentEmojis = [];
			IEnumerable<Emote> allEmojis = [emote];

			var target = new DiscordSentenceRenderer();

			var result = target.Render(input, currentEmojis, allEmojis);
			result.Should().Be(output);
		}

		[Theory]
		[InlineData("¿ Dónde está mi gran sombrero ?", "\\¿D\\ónde est\\á mi gran sombrero\\?")]
		[InlineData("¡ No encuentro mi pierna izquierda !", "\\¡No encuentro mi pierna izquierda\\!")]
		[InlineData("¡¿ Alguien puede ayudarme por favor ?!", "\\¡\\¿Alguien puede ayudarme por favor\\?\\!")]
		[InlineData("¡¡¡¿¿¿ Alguien puede ayudarme por favor ???!!!", "\\¡\\¡\\¡\\¿\\¿\\¿Alguien puede ayudarme por favor\\?\\?\\?\\!\\!\\!")]
		public void RenderForeignTests(string input, string output)
		{
			var target = new DiscordSentenceRenderer();

			var result = target.Render(input);

			result.Should().Be(output);
		}
	}
}
