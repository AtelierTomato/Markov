using System.Text.RegularExpressions;
using AtelierTomato.Calculator;
using AtelierTomato.Dice;
using AtelierTomato.Markov.Core.Cooldown;
using AtelierTomato.Markov.Model;
using AtelierTomato.Markov.Service.Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AtelierTomato.Markov.Bot.Discord.Core.CommandModules
{
	public class CalculatorModule : ModuleBase<SocketCommandContext>
	{
		private readonly ILogger<CalculatorModule> logger;
		private readonly DiscordBotOptions options;
		private readonly Cooldown cooldown;
		private readonly ExpressionParser expressionParser;
		private readonly ExpressionExecutor expressionExecutor;
		private readonly DiceExpressionParser diceExpressionParser;
		private readonly DiceExpressionExecutor diceExpressionExecutor;
		private readonly HelpContentBuilder helpContentBuilder;
		private readonly DiscordObjectOIDBuilder objectOIDBuilder;
		public CalculatorModule(ILogger<CalculatorModule> logger, IOptions<DiscordBotOptions> options, Cooldown cooldown, ExpressionParser expressionParser, ExpressionExecutor expressionExecutor, DiceExpressionParser diceExpressionParser, DiceExpressionExecutor diceExpressionExecutor, HelpContentBuilder helpContentBuilder, DiscordObjectOIDBuilder objectOIDBuilder)
		{
			this.logger = logger;
			this.options = options.Value;
			this.cooldown = cooldown;
			this.expressionParser = expressionParser;
			this.expressionExecutor = expressionExecutor;
			this.diceExpressionParser = diceExpressionParser;
			this.diceExpressionExecutor = diceExpressionExecutor;
			this.helpContentBuilder = helpContentBuilder;
			this.objectOIDBuilder = objectOIDBuilder;
		}

		[Command("calculator")]
		[Alias("calculate", "calc", "c")]
		public async Task Calculate([Remainder] string input)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			// remove escaped characters in order to allow markdown horror circumvention
			var unescapedInput = input.Replace("\\", string.Empty);

			string message;
			try
			{
				var parsedCalculation = expressionParser.Parse(unescapedInput);
				var calculationResult = expressionExecutor.Calculate(parsedCalculation);
				message = $"The result of `{parsedCalculation}` is: `{calculationResult:0.########}`";
			}
			catch (ParseException ex)
			{
				message = $"Your math `{input}` was wrong: {ex.Message}";
			}
			catch (ExecuteException ex)
			{
				message = $"Your math `{input}` was wrong: {ex.Message}";
			}

			await base.ReplyAsync(message);
		}

		[Command("calculator")]
		[Alias("calculate", "calc", "c")]
		public async Task Calculate()
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.Calculate).Build());
		}

		[Command("roll")]
		[Alias("dice", "die")]
		public async Task Roll([Remainder] string input)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			// remove escaped characters in order to allow markdown horror circumvention
			var unescapedInput = input.Replace("\\", string.Empty);
			string fixedInput = FixDiceInput(unescapedInput);

			string message;
			try
			{
				var parsedDice = diceExpressionParser.Parse(fixedInput);
				var diceResults = diceExpressionExecutor.Execute(parsedDice);
				var hasMathRegex = new Regex(@"[+\-*×∙/÷^]|\d+(\s+)?\(");
				if (hasMathRegex.Match(fixedInput).Success && !string.IsNullOrEmpty(diceResults.ExecutionLog))
				{
					if (diceResults.ExecutionLog.Contains(Environment.NewLine))
					{
						message = $"`{parsedDice}`: {diceResults.NumericResult:0.########}{Environment.NewLine}- {diceResults.ExecutionLog.Replace(Environment.NewLine, Environment.NewLine + "- ")}";
					}
					else
					{
						message = $"`{parsedDice}`: {diceResults.NumericResult:0.########} - {diceResults.ExecutionLog}";
					}
				}
				else if (string.IsNullOrEmpty(diceResults.ExecutionLog))
				{
					message = $"`{parsedDice}`: {diceResults.NumericResult:0.########}";
				}
				else
				{
					message = diceResults.ExecutionLog;
				}

				if (message.Length >= 2000)
				{
					message = $"`{parsedDice}`: {diceResults.NumericResult:0.########} - execution log is too long to show.";
				}
			}
			catch (ParseException ex)
			{
				message = $"your math `{fixedInput}` was wrong: {ex.Message}";
			}
			catch (ExecuteException ex)
			{
				message = $"your math `{input}` was wrong: {ex.Message}";
			}
			catch (ArgumentException ex)
			{
				message = $"your math `{input}` was wrong: {ex.Message}";
			}
			catch (Exception ex)
			{
				message = $"oh no, something went wrong with the math: {ex.Message}";
				this.logger.LogWarning(ex, "There was an error during calculation of a roll.");
			}

			await base.ReplyAsync(message);
		}

		[Command("roll")]
		[Alias("dice", "die")]
		public async Task Roll()
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.Roll).Build());
		}

		[Command("rollrepeat")]
		[Alias("rollrep", "rr", "dierepeat", "dierep", "dicerepeat", "dicerep", "dr")]
		public async Task RollRepeat(int iterations, [Remainder] string input)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}

			if (iterations < 1)
			{
				await base.ReplyAsync("You can't roll 0 dice!");
				return;
			}

			if (iterations > 50)
			{
				await base.ReplyAsync("Only up to 50 repetitions are allowed!");
				return;
			}

			// remove escaped characters in order to allow markdown horror circumvention
			var unescapedInput = input.Replace("\\", string.Empty);
			string fixedInput = FixDiceInput(unescapedInput);

			string message;

			try
			{
				var parsedDice = diceExpressionParser.Parse(fixedInput);
				var hasMathRegex = new Regex(@"[+\-*×∙/÷^]|\d+(\s+)?\(");
				message = String.Join(Environment.NewLine, Enumerable.Range(1, iterations).Select(_ =>
				{
					var diceResults = diceExpressionExecutor.Execute(parsedDice);
					string partMessage;
					if (hasMathRegex.Match(fixedInput).Success && !string.IsNullOrEmpty(diceResults.ExecutionLog))
					{
						if (diceResults.ExecutionLog.Contains(Environment.NewLine))
						{
							partMessage = $"`{parsedDice}`: {diceResults.NumericResult:0.########}{Environment.NewLine}- {diceResults.ExecutionLog.Replace(Environment.NewLine, Environment.NewLine + "- ")}";
						}
						else
						{
							partMessage = $"`{parsedDice}`: {diceResults.NumericResult:0.########} - {diceResults.ExecutionLog}";
						}
					}
					else if (string.IsNullOrEmpty(diceResults.ExecutionLog))
					{
						partMessage = $"`{parsedDice}`: {diceResults.NumericResult:0.########}{Environment.NewLine}";
					}
					else
					{
						partMessage = $"{diceResults.ExecutionLog}";
					}

					if (partMessage.Length >= 2000 / iterations)
					{
						partMessage = $"`{parsedDice}`: {diceResults.NumericResult:0.########} - execution log is too long to show.";
					}

					return partMessage;
				}));
				if (message.Length > 2000)
					throw new ExecuteException("output is too long to fit into a message!");
			}
			catch (ParseException ex)
			{
				message = $"your math `{fixedInput}` was wrong: {ex.Message}";
			}
			catch (ExecuteException ex)
			{
				message = $"your math `{input}` was wrong: {ex.Message}";
			}
			catch (ArgumentException ex)
			{
				message = $"your math `{input}` was wrong: {ex.Message}";
			}
			catch (Exception ex)
			{
				message = $"oh no, something went wrong with the math: {ex.Message}";
				this.logger.LogWarning(ex, "There was an error during calculation of a roll.");
			}

			await base.ReplyAsync(message);
		}

		[Command("rollrepeat")]
		[Alias("rollrep", "rr", "dierepeat", "dierep", "dicerepeat", "dicerep", "dr")]
		public async Task RollRepeat([Remainder] string? _ = null)
		{
			var authorOID = new AuthorOID(ServiceType.Discord, options.DiscordInstance, Context.User.Id.ToString());
			var location = await objectOIDBuilder.Build(Context.Guild, Context.Channel, options.DiscordInstance);
			if (!cooldown.HandleCooldown(authorOID, location, CooldownType.Default))
			{
				await ReplyAsync(message: "slow down!!");
				return;
			}
			await ReplyAsync(embed: helpContentBuilder.BuildForSubject(HelpSubject.RollRepeat).Build());
		}

		private static string FixDiceInput(string input)
		{
			//allow countless simple rolls by forcing a 1 for count
			var basicRollRegex = new Regex(@"(^| )d\d+($| )");
			if (basicRollRegex.Match(input).Success)
			{
				return input.Replace("d", "1d");
			}
			else
			{
				return input;
			}
		}
	}
}
